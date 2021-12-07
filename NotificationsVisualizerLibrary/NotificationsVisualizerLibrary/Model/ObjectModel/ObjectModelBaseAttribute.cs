using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationsVisualizerLibrary.Model
{
    internal abstract class ObjectModelBaseAttribute : Attribute
    {
        public NotificationType? Context { get; private set; }

        public ObjectModelBaseAttribute() { }

        public ObjectModelBaseAttribute(NotificationType context)
        {
            Context = context;
        }
    }
}
