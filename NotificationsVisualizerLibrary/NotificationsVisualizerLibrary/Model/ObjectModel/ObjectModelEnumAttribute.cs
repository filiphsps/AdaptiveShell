using System;

namespace NotificationsVisualizerLibrary.Model
{
    internal class ObjectModelEnumAttribute : ObjectModelBaseAttribute
    {
        public String Name { get; set; }

        public ObjectModelEnumAttribute(String name)
        {
            this.Name = name;
        }

        public ObjectModelEnumAttribute(String name, NotificationType context) : base(context)
        {
            this.Name = name;
        }
    }
}
