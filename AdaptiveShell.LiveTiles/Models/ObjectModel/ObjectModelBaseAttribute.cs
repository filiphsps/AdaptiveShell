using AdaptiveShell.LiveTiles.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveShell.LiveTiles.Models.ObjectModel {
    public abstract class ObjectModelBaseAttribute : Attribute {
        public XmlTemplateParser.NotificationType? Context { get; private set; }

        public ObjectModelBaseAttribute() { }

        public ObjectModelBaseAttribute(XmlTemplateParser.NotificationType context) {
            this.Context = context;
        }
    }
}
