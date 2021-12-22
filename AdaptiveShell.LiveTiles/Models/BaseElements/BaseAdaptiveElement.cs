using AdaptiveShell.LiveTiles.Models.ObjectModel;
using AdaptiveShell.LiveTiles.Parsers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AdaptiveShell.LiveTiles.Models.BaseElements {
    public abstract class BaseAdaptiveElement : BaseElement {
        internal const String ATTR_ARGUMENTS = "arguments";
        internal const String ATTR_STATE = "state";

        public BaseAdaptiveElement(XmlTemplateParser.NotificationType context) {
            this.Context = context;
        }

        public XmlTemplateParser.NotificationType Context { get; protected set; }

        public virtual ObjectModelObject ConvertToObject() {
            ObjectModelClassAttribute classAttr = this.GetObjectModelClassAttribute();
            if (classAttr == null) {
                return null;
            }

            var answer = new ObjectModelObject(classAttr.Name);

            foreach (PropertyInfo propInfo in this.GetType().GetProperties()) {
                ObjectModelPropertyAttribute propAttr = this.GetObjectModelPropertyAttribute(propInfo);
                if (propAttr != null) {
                    Object propVal = propInfo.GetValue(this);
                    if (!Object.Equals(propAttr.DefaultValue, propVal)) {
                        IObjectModelValue value = this.CreateObject(propVal);

                        if (value != null) {
                            answer.Add(propAttr.Name, value);
                        }
                    }
                } else {
                    ObjectModelBindingPropertyAttribute bindingPropAttr = this.GetObjectModelBindingPropertyAttribute(propInfo);
                    if (bindingPropAttr != null) {
                        String bindingName = propInfo.GetValue(this) as String;
                        if (bindingName != null) {
                            answer.AddBinding(bindingPropAttr, bindingName);
                        }
                    }
                }
            }

            return answer;
        }

        private IObjectModelValue CreateObject(Object propVal) {
            if (propVal is BaseAdaptiveElement) {
                return (propVal as BaseAdaptiveElement).ConvertToObject();
            } else if (propVal is IEnumerable && !(propVal is String) && !(propVal is IObjectModelValue)) {
                var listInit = new ObjectModelListInitialization();
                foreach (Object o in (propVal as IEnumerable)) {
                    listInit.Add(this.CreateObject(o));
                }
                return listInit;
            } else if (propVal is Enum) {
                ObjectModelEnumAttribute enumAttr = this.GetForContext(propVal.GetType().GetTypeInfo().GetCustomAttributes<ObjectModelEnumAttribute>());
                if (enumAttr != null) {
                    return new ObjectModelEnum(enumAttr.Name, propVal.ToString());
                }
            }

            return ObjectModelObject.Create(propVal);
        }

        private ObjectModelClassAttribute GetObjectModelClassAttribute() {
            return this.GetForContext(this.GetType().GetTypeInfo().GetCustomAttributes<ObjectModelClassAttribute>());
        }

        private ObjectModelPropertyAttribute GetObjectModelPropertyAttribute(PropertyInfo propInfo) {
            return this.GetForContext(propInfo.GetCustomAttributes<ObjectModelPropertyAttribute>());
        }

        private ObjectModelBindingPropertyAttribute GetObjectModelBindingPropertyAttribute(PropertyInfo propInfo) {
            return this.GetForContext(propInfo.GetCustomAttributes<ObjectModelBindingPropertyAttribute>());
        }

        private T GetForContext<T>(IEnumerable<T> attributes) where T : ObjectModelBaseAttribute {
            T matching = attributes.FirstOrDefault(i => i.Context != null && i.Context.Value == this.Context);
            if (matching != null) {
                return matching;
            }
            return attributes.FirstOrDefault(i => i.Context == null);
        }
    }
}
