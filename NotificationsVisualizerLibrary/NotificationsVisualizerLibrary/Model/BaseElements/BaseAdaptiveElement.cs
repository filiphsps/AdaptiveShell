using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using NotificationsVisualizerLibrary.Parsers;
using System.Collections;

namespace NotificationsVisualizerLibrary.Model.BaseElements
{
    internal abstract class BaseAdaptiveElement : BaseElement
    {
        internal const string ATTR_ARGUMENTS = "arguments";
        internal const string ATTR_ACTIVATIONTYPE = "activationType";
        internal const string ATTR_STATE = "state";

        public BaseAdaptiveElement(NotificationType context, FeatureSet supportedFeatures)
        {
            Context = context;
            SupportedFeatures = supportedFeatures;
        }

        public NotificationType Context { get; protected set; }

        public FeatureSet SupportedFeatures { get; private set; }

        internal void ParseActivatableElementAttributes(XElement node, AttributesHelper attributes, ParseResult result)
        {
            IActivatableElement el = this as IActivatableElement;

            // activationType is optional
            ActivationType type;
            if (TryParseEnum(result, attributes, ATTR_ACTIVATIONTYPE, out type))
                el.ActivationType = type;


            // arguments is required
            XAttribute attrArguments = attributes.PopAttribute(ATTR_ARGUMENTS);
            if (attrArguments == null)
            {
                // If we're in an activation type that requires attributes
                if (el.ActivationType != ActivationType.None)
                {
                    result.AddErrorButRenderAllowed("arguments attribute is required.", XmlTemplateParser.GetErrorPositionInfo(node));
                    throw new IncompleteElementException();
                }
            }

            else
                el.Arguments = attrArguments.Value;
        }

        public virtual ObjectModelObject ConvertToObject()
        {
            var classAttr = GetObjectModelClassAttribute();
            if (classAttr == null)
            {
                return null;
            }

            var answer = new ObjectModelObject(classAttr.Name);

            foreach (var propInfo in this.GetType().GetProperties())
            {
                var propAttr = GetObjectModelPropertyAttribute(propInfo);
                if (propAttr != null)
                {
                    object propVal = propInfo.GetValue(this);
                    if (!object.Equals(propAttr.DefaultValue, propVal))
                    {
                        IObjectModelValue value = CreateObject(propVal);

                        if (value != null)
                        {
                            answer.Add(propAttr.Name, value);
                        }
                    }
                }
                else
                {
                    var bindingPropAttr = GetObjectModelBindingPropertyAttribute(propInfo);
                    if (bindingPropAttr != null)
                    {
                        string bindingName = propInfo.GetValue(this) as string;
                        if (bindingName != null)
                        {
                            answer.AddBinding(bindingPropAttr, bindingName);
                        }
                    }
                }
            }

            return answer;
        }

        private IObjectModelValue CreateObject(object propVal)
        {
            if (propVal is BaseAdaptiveElement)
            {
                return (propVal as BaseAdaptiveElement).ConvertToObject();
            }
            else if (propVal is IEnumerable && !(propVal is string) && !(propVal is IObjectModelValue))
            {
                var listInit = new ObjectModelListInitialization();
                foreach (var o in (propVal as IEnumerable))
                {
                    listInit.Add(CreateObject(o));
                }
                return listInit;
            }
            else if (propVal is Enum)
            {
                var enumAttr = GetForContext(propVal.GetType().GetTypeInfo().GetCustomAttributes<ObjectModelEnumAttribute>());
                if (enumAttr != null)
                {
                    return new ObjectModelEnum(enumAttr.Name, propVal.ToString());
                }
            }

            return ObjectModelObject.Create(propVal);
        }

        private ObjectModelClassAttribute GetObjectModelClassAttribute()
        {
            return GetForContext(this.GetType().GetTypeInfo().GetCustomAttributes<ObjectModelClassAttribute>());
        }

        private ObjectModelPropertyAttribute GetObjectModelPropertyAttribute(PropertyInfo propInfo)
        {
            return GetForContext(propInfo.GetCustomAttributes<ObjectModelPropertyAttribute>());
        }

        private ObjectModelBindingPropertyAttribute GetObjectModelBindingPropertyAttribute(PropertyInfo propInfo)
        {
            return GetForContext(propInfo.GetCustomAttributes<ObjectModelBindingPropertyAttribute>());
        }

        private T GetForContext<T>(IEnumerable<T> attributes) where T : ObjectModelBaseAttribute
        {
            var matching = attributes.FirstOrDefault(i => i.Context != null && i.Context.Value == Context);
            if (matching != null)
            {
                return matching;
            }
            return attributes.FirstOrDefault(i => i.Context == null);
        }
    }
}
