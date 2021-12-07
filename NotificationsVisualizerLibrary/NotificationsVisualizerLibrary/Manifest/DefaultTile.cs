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
        public string Wide310x150Logo { get; set; }

        [XmlAttribute]
        public string Square71x71Logo { get; set; }

        [XmlAttribute]
        public string Square310x310Logo { get; set; }

        [XmlAttribute]
        public string Tall150x310Logo { get; set; }
    }
}
