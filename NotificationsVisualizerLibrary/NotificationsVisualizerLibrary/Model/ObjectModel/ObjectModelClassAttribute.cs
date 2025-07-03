using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
