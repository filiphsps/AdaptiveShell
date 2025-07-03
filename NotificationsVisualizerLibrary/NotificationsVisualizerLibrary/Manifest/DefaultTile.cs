using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NotificationsVisualizerLibrary.Manifest
{
    public sealed class DefaultTile
    {
        [XmlAttribute]
        public String Wide310x150Logo { get; set; }

        [XmlAttribute]
        public String Square71x71Logo { get; set; }

        [XmlAttribute]
        public String Square310x310Logo { get; set; }

        [XmlAttribute]
        public String Tall150x310Logo { get; set; }
    }
}
