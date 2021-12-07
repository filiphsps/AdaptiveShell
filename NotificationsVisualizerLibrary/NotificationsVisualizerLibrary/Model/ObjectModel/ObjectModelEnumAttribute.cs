using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationsVisualizerLibrary.Model
{
    internal class ObjectModelEnumAttribute : ObjectModelBaseAttribute
    {
        public string Name { get; set; }

        public ObjectModelEnumAttribute(string name)
        {
            Name = name;
        }

        public ObjectModelEnumAttribute(string name, NotificationType context) : base(context)
        {
            Name = name;
        }
    }
}
