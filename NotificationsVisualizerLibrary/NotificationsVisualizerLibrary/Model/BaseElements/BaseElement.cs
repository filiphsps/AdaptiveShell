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
        public bool UsesDataBinding { get; private set; }

        internal static ErrorPositionInfo GetErrorPositionInfo(XObject node)
        {
            return XmlTemplateParser.GetErrorPositionInfo(node);
        }

        internal bool TryParseEnum<TEnum>(string text, out TEnum answer) where TEnum : struct, IConvertible
        {
            var supportedEnums = GetSupportedEnums<TEnum>();

            if (text != null)
            {
                foreach (var e in supportedEnums)
                {
                    string xmlValue = GetXmlValueFromEnum((Enum)e);
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

        private static string GetXmlValueFromEnum(Enum enumValue)
        {
            XmlValueAttribute xmlValueAttribute = enumValue.GetType().GetTypeInfo().GetDeclaredField(enumValue.ToString()).GetCustomAttribute<XmlValueAttribute>();

            if (xmlValueAttribute != null)
                return xmlValueAttribute.XmlValue;

            return enumValue.ToString();
        }

        internal bool TryParseEnum<TEnum>(ParseResult result, AttributesHelper attributes, string attributeName, out TEnum answer, bool caseSensitive = true) where TEnum : struct, IConvertible
        {
            XAttribute attr = attributes.PopAttribute(attributeName, caseSensitive);

            // If it has the attribute
            if (attr != null)
            {
                // And the attribute has a value
                if (attr.Value != null)
                {
                    if (TryParseEnum(attr.Value, out answer))
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

        private static bool TryGetUnknownEnum<TEnum>(out TEnum answer)
        {
            // If there's an Unknown enum value, use that
            foreach (var e in Enum.GetValues(typeof(TEnum)))
            {
                string xmlValue = GetXmlValueFromEnum((Enum)e);
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
            public string DataName { get; private set; }
            public Action<string> OnSet { get; private set; }
            private ParseError _currentError;
            private ParseResult _result;

            public ConfiguredBinding(ParseResult result, string dataName, Action<string> onSet)
            {
                DataName = dataName;
                OnSet = onSet;
                _result = result;
            }

            public void Invoke(string value)
            {
                ParseError newError;
                try
                {
                    // If data binding value wasn't present
                    if (value == null)
                    {
                        // We fall back to displaying {value} or whatever dev provided as plain text
                        OnSet("{" + DataName + "}");
                    }
                    else
                    {
                        OnSet(value);
                    }
                    newError = null;
                }
                catch (ParseErrorException ex)
                {
                    newError = ex.Error;
                }

                if (_currentError != null && newError == null)
                {
                    _result.Errors.Remove(_currentError);
                    _currentError = null;
                    return;
                }

                if (newError != null)
                {
                    if (_currentError != null && newError.Equals(_currentError))
                    {
                        return;
                    }

                    else if (_currentError != null)
                    {
                        _result.Errors.Remove(_currentError);
                    }

                    _result.Add(newError);
                    _currentError = newError;
                }
            }
        }

        private List<ConfiguredBinding> _bindings = new List<ConfiguredBinding>();

        internal void SetBinding(ParseResult result, string dataName, Action<string> onSet)
        {
            _bindings.Add(new ConfiguredBinding(result, dataName, onSet));
        }

        internal class ParseErrorException : Exception
        {
            public ParseError Error { get; private set; }
            public ParseErrorException(ParseError error)
            {
                Error = error;
            }
        }

        /// <summary>
        /// Returns false if attribute not found. If attribute is using data binding, configures data binding. Otherwise, passes the attribute value to the provided method.
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="attributeName"></param>
        /// <param name="processValue"></param>
        internal bool TryPopAttributeValueWithBinding(ParseResult result, AttributesHelper attributes, string attributeName, out string bindingName, Action<string> processValue)
        {
            string attrValue = attributes.PopAttributeValue(attributeName);

            return TryProcessBindableValue(result, attrValue, out bindingName, processValue);
        }

        internal bool TryProcessBindableValue(ParseResult result, string value, out string bindingName, Action<string> processValue)
        {
            bindingName = null;

            if (value == null)
            {
                return false;
            }

            if (IsDataBinding(value))
            {
                string dataName = GetDataBindingName(value);
                SetBinding(result, dataName, processValue);
                UsesDataBinding = true;
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
            foreach (var binding in _bindings)
            {
                string value;
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
            ApplyDataBindingToThisInstance(dataBinding);
        }

        internal static bool IsDataBinding(string attributeValue)
        {
            return attributeValue.StartsWith("{") && attributeValue.EndsWith("}");
        }

        /// <summary>
        /// Assumes that it is already confirmed to be a data binding name
        /// </summary>
        /// <param name="attributeValue"></param>
        /// <returns></returns>
        internal static string GetDataBindingName(string attributeValue)
        {
            return attributeValue.Substring(1, attributeValue.Length - 2);
        }

        internal static bool TryParse(ParseResult result, AttributesHelper attributes, string attributeName, out int answer)
        {
            return XmlTemplateParser.TryParse(result, attributes, attributeName, out answer);
        }

        internal static bool TryParse(ParseResult result, AttributesHelper attributes, string attributeName, out double answer)
        {
            return XmlTemplateParser.TryParse(result, attributes, attributeName, out answer);
        }

        internal static bool TryParse(ParseResult result, AttributesHelper attributes, string attributeName, out bool answer)
        {
            return XmlTemplateParser.TryParse(result, attributes, attributeName, out answer);
        }

        protected virtual Array GetSupportedEnums<TEnum>() where TEnum : struct, IConvertible
        {
            return Enum.GetValues(typeof(TEnum));
        }

        protected void HandleRemainingAttributes(AttributesHelper attributes, ParseResult result)
        {
            XmlTemplateParser.AddWarningsForAttributesNotSupportedByVisualizer(result, attributes, GetAttributesNotSupportedByVisualizer().ToArray());

            XmlTemplateParser.AddWarningsForUnknownAttributes(result, attributes);
        }

        protected abstract IEnumerable<string> GetAttributesNotSupportedByVisualizer();
    }
}
