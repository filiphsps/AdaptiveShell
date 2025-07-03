using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
