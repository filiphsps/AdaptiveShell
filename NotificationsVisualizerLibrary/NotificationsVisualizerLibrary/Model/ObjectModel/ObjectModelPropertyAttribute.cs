using System;

namespace NotificationsVisualizerLibrary.Model
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    internal class ObjectModelPropertyAttribute : ObjectModelBaseAttribute
    {
        public String Name { get; private set; }
        public Object DefaultValue { get; private set; }

        public ObjectModelPropertyAttribute(String name)
        {
            this.Name = name;
        }

        public ObjectModelPropertyAttribute(String name, Object defaultValue)
        {
            this.Name = name;
            this.DefaultValue = defaultValue;
        }

        public ObjectModelPropertyAttribute(String name, Object defaultValue, NotificationType context) : base(context)
        {
            this.Name = name;
            this.DefaultValue = defaultValue;
        }
    }
}
