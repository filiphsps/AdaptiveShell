using AdaptiveShell.LiveTiles.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveShell.LiveTiles.Models.ObjectModel {
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    internal class ObjectModelPropertyAttribute : ObjectModelBaseAttribute {
        public String Name { get; private set; }
        public Object DefaultValue { get; private set; }

        public ObjectModelPropertyAttribute(String name) {
            this.Name = name;
        }

        public ObjectModelPropertyAttribute(String name, Object defaultValue) {
            this.Name = name;
            this.DefaultValue = defaultValue;
        }

        public ObjectModelPropertyAttribute(String name, Object defaultValue, XmlTemplateParser.NotificationType context) : base(context) {
            this.Name = name;
            this.DefaultValue = defaultValue;
        }
    }
}
