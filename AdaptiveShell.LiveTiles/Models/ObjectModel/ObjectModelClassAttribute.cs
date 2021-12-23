using AdaptiveShell.LiveTiles.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveShell.LiveTiles.Models.ObjectModel {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    internal class ObjectModelClassAttribute : ObjectModelBaseAttribute {
        public String Name { get; private set; }

        public ObjectModelClassAttribute(String name) {
            this.Name = name;
        }

        public ObjectModelClassAttribute(String name, XmlTemplateParser.NotificationType context) : base(context) {
            this.Name = name;
        }
    }
}
