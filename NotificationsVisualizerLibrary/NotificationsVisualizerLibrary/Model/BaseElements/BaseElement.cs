using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using NotificationsVisualizerLibrary.Model.Enums;
using NotificationsVisualizerLibrary.Parsers;
using System.Reflection;

namespace NotificationsVisualizerLibrary.Model.BaseElements
{
    internal abstract class BaseElement : BindableBase
    {
        /// <summary>
        /// Returns true if this element uses data binding
        /// </summary>
        public Boolean UsesDataBinding { get; private set; }

        internal static ErrorPositionInfo GetErrorPositionInfo(XObject node)
        {
            return XmlTemplateParser.GetErrorPositionInfo(node);
        }

        internal Boolean TryParseEnum<TEnum>(String text, out TEnum answer) where TEnum : struct, IConvertible
        {
            var supportedEnums = this.GetSupportedEnums<TEnum>();

            if (text != null)
            {
                foreach (var e in supportedEnums)
                {
                    String xmlValue = GetXmlValueFromEnum((Enum)e);
                    if (text.Equals(xmlValue, StringComparison.CurrentCultureIgnoreCase))
                    {
                        answer = (TEnum)e;
                        return true;
                    }
                }
            }

            

            answer = default(TEnum);
            return false;
        }

        private static String GetXmlValueFromEnum(Enum enumValue)
        {
            XmlValueAttribute xmlValueAttribute = enumValue.GetType().GetTypeInfo().GetDeclaredField(enumValue.ToString()).GetCustomAttribute<XmlValueAttribute>();

            if (xmlValueAttribute != null)
                return xmlValueAttribute.XmlValue;

            return enumValue.ToString();
        }

        internal Boolean TryParseEnum<TEnum>(ParseResult result, AttributesHelper attributes, String attributeName, out TEnum answer, Boolean caseSensitive = true) where TEnum : struct, IConvertible
        {
            XAttribute attr = attributes.PopAttribute(attributeName, caseSensitive);

            // If it has the attribute
            if (attr != null)
            {
                // And the attribute has a value
                if (attr.Value != null)
                {
                    if (this.TryParseEnum(attr.Value, out answer))
                        return true;
                    
                    // Couldn't find matching enum
                    result.AddWarning($@"Unknown value ""{attr.Value}"" on {attributeName} attribute", GetErrorPositionInfo(attr));

                    if (TryGetUnknownEnum(out answer))
                        return true;
                }

                // Attribute doesn't have a value
                else
                    result.AddWarning($@"Attribute {attributeName} has no value specified", GetErrorPositionInfo(attr));
            }

            // Attribute isn't found, no warning
            answer = default(TEnum);
            return false;
        }

        private static Boolean TryGetUnknownEnum<TEnum>(out TEnum answer)
        {
            // If there's an Unknown enum value, use that
            foreach (var e in Enum.GetValues(typeof(TEnum)))
            {
                String xmlValue = GetXmlValueFromEnum((Enum)e);
                if (xmlValue.Equals("unknown", StringComparison.CurrentCultureIgnoreCase))
                {
                    answer = (TEnum)e;
                    return true;
                }
            }

            answer = default(TEnum);
            return false;
        }

        private class ConfiguredBinding
        {
            public String DataName { get; private set; }
            public Action<String> OnSet { get; private set; }
            private ParseError _currentError;
            private ParseResult _result;

            public ConfiguredBinding(ParseResult result, String dataName, Action<String> onSet)
            {
                this.DataName = dataName;
                this.OnSet = onSet;
                this._result = result;
            }

            public void Invoke(String value)
            {
                ParseError newError;
                try
                {
                    // If data binding value wasn't present
                    if (value == null)
                    {
                        // We fall back to displaying {value} or whatever dev provided as plain text
                        this.OnSet("{" + this.DataName + "}");
                    }
                    else
                    {
                        this.OnSet(value);
                    }
                    newError = null;
                }
                catch (ParseErrorException ex)
                {
                    newError = ex.Error;
                }

                if (this._currentError != null && newError == null)
                {
                    this._result.Errors.Remove(this._currentError);
                    this._currentError = null;
                    return;
                }

                if (newError != null)
                {
                    if (this._currentError != null && newError.Equals(this._currentError))
                    {
                        return;
                    }

                    else if (this._currentError != null)
                    {
                        this._result.Errors.Remove(this._currentError);
                    }

                    this._result.Add(newError);
                    this._currentError = newError;
                }
            }
        }

        private List<ConfiguredBinding> _bindings = new List<ConfiguredBinding>();

        internal void SetBinding(ParseResult result, String dataName, Action<String> onSet)
        {
            this._bindings.Add(new ConfiguredBinding(result, dataName, onSet));
        }

        internal class ParseErrorException : Exception
        {
            public ParseError Error { get; private set; }
            public ParseErrorException(ParseError error)
            {
                this.Error = error;
            }
        }

        /// <summary>
        /// Returns false if attribute not found. If attribute is using data binding, configures data binding. Otherwise, passes the attribute value to the provided method.
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="attributeName"></param>
        /// <param name="processValue"></param>
        internal Boolean TryPopAttributeValueWithBinding(ParseResult result, AttributesHelper attributes, String attributeName, out String bindingName, Action<String> processValue)
        {
            String attrValue = attributes.PopAttributeValue(attributeName);

            return this.TryProcessBindableValue(result, attrValue, out bindingName, processValue);
        }

        internal Boolean TryProcessBindableValue(ParseResult result, String value, out String bindingName, Action<String> processValue)
        {
            bindingName = null;

            if (value == null)
            {
                return false;
            }

            if (IsDataBinding(value))
            {
                String dataName = GetDataBindingName(value);
                this.SetBinding(result, dataName, processValue);
                this.UsesDataBinding = true;
                bindingName = dataName;
            }
            else
            {
                try
                {
                    processValue(value);
                }
                catch (ParseErrorException ex)
                {
                    result.Add(ex.Error);
                }
            }

            return true;
        }

        protected void ApplyDataBindingToThisInstance(DataBindingValues dataBinding)
        {
            foreach (var binding in this._bindings)
            {
                String value;
                if (dataBinding.TryGetValue(binding.DataName, out value))
                {
                    binding.Invoke(value);
                }
                else
                {
                    binding.Invoke(null);
                }
            }
        }

        internal virtual void ApplyDataBinding(DataBindingValues dataBinding)
        {
            this.ApplyDataBindingToThisInstance(dataBinding);
        }

        internal static Boolean IsDataBinding(String attributeValue)
        {
            return attributeValue.StartsWith("{") && attributeValue.EndsWith("}");
        }

        /// <summary>
        /// Assumes that it is already confirmed to be a data binding name
        /// </summary>
        /// <param name="attributeValue"></param>
        /// <returns></returns>
        internal static String GetDataBindingName(String attributeValue)
        {
            return attributeValue.Substring(1, attributeValue.Length - 2);
        }

        internal static Boolean TryParse(ParseResult result, AttributesHelper attributes, String attributeName, out Int32 answer)
        {
            return XmlTemplateParser.TryParse(result, attributes, attributeName, out answer);
        }

        internal static Boolean TryParse(ParseResult result, AttributesHelper attributes, String attributeName, out Double answer)
        {
            return XmlTemplateParser.TryParse(result, attributes, attributeName, out answer);
        }

        internal static Boolean TryParse(ParseResult result, AttributesHelper attributes, String attributeName, out Boolean answer)
        {
            return XmlTemplateParser.TryParse(result, attributes, attributeName, out answer);
        }

        protected virtual Array GetSupportedEnums<TEnum>() where TEnum : struct, IConvertible
        {
            return Enum.GetValues(typeof(TEnum));
        }

        protected void HandleRemainingAttributes(AttributesHelper attributes, ParseResult result)
        {
            XmlTemplateParser.AddWarningsForAttributesNotSupportedByVisualizer(result, attributes, this.GetAttributesNotSupportedByVisualizer().ToArray());

            XmlTemplateParser.AddWarningsForUnknownAttributes(result, attributes);
        }

        protected abstract IEnumerable<String> GetAttributesNotSupportedByVisualizer();
    }
}
