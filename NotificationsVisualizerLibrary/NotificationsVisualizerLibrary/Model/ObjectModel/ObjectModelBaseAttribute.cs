using System;

namespace NotificationsVisualizerLibrary.Model
{
    internal abstract class ObjectModelBaseAttribute : Attribute
    {
        public NotificationType? Context { get; private set; }

        public ObjectModelBaseAttribute() { }

        public ObjectModelBaseAttribute(NotificationType context)
        {
            this.Context = context;
        }
    }
}
