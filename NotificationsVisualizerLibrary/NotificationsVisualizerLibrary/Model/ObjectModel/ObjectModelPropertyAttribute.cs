using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationsVisualizerLibrary.Model
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    internal class ObjectModelPropertyAttribute : ObjectModelBaseAttribute
    {
        public string Name { get; private set; }
        public object DefaultValue { get; private set; }

        public ObjectModelPropertyAttribute(string name)
        {
            Name = name;
        }

        public ObjectModelPropertyAttribute(string name, object defaultValue)
        {
            Name = name;
            DefaultValue = defaultValue;
        }

        public ObjectModelPropertyAttribute(string name, object defaultValue, NotificationType context) : base(context)
        {
            Name = name;
            DefaultValue = defaultValue;
        }
    }
}
