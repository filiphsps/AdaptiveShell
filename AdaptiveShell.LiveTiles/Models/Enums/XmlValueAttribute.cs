using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveShell.LiveTiles.Models.Enums {
    internal class XmlValueAttribute : Attribute {
        public String XmlValue { get; private set; }

        public XmlValueAttribute(String xmlValue) {
            this.XmlValue = xmlValue;
        }
    }
}
