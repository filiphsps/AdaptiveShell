using System;

namespace NotificationsVisualizerLibrary.Model
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    internal class ObjectModelClassAttribute : ObjectModelBaseAttribute
    {
        public String Name { get; private set; }

        public ObjectModelClassAttribute(String name)
        {
            this.Name = name;
        }

        public ObjectModelClassAttribute(String name, NotificationType context) : base(context)
        {
            this.Name = name;
        }
    }
}
