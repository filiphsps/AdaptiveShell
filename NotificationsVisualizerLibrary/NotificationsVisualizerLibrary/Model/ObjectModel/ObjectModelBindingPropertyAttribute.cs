using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationsVisualizerLibrary.Model
{
    internal class ObjectModelBindingPropertyAttribute : ObjectModelBaseAttribute
    {
        public String PropertyName { get; set; }
        public String BindableTypeName { get; set; }

        public ObjectModelBindingPropertyAttribute(String propertyName, String bindableTypeName)
        {
            this.PropertyName = propertyName;
            this.BindableTypeName = bindableTypeName;
        }

        public ObjectModelBindingPropertyAttribute(String propertyName, String bindableTypeName, NotificationType context) : base(context)
        {
            this.PropertyName = propertyName;
            this.BindableTypeName = bindableTypeName;
        }
    }
}
