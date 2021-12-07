using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationsVisualizerLibrary.Model
{
    internal class ObjectModelBindingPropertyAttribute : ObjectModelBaseAttribute
    {
        public string PropertyName { get; set; }
        public string BindableTypeName { get; set; }

        public ObjectModelBindingPropertyAttribute(string propertyName, string bindableTypeName)
        {
            PropertyName = propertyName;
            BindableTypeName = bindableTypeName;
        }

        public ObjectModelBindingPropertyAttribute(string propertyName, string bindableTypeName, NotificationType context) : base(context)
        {
            PropertyName = propertyName;
            BindableTypeName = bindableTypeName;
        }
    }
}
