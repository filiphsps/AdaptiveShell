using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NotificationsVisualizerLibrary.Manifest
{
    public sealed class Application
    {
        [XmlAttribute]
        public string Id { get; set; }

        [XmlElement(ElementName = "VisualElements")]
        public VisualElements M1VisualElements { get; set; }


        [XmlElement(Namespace = "http://schemas.microsoft.com/appx/2014/manifest", ElementName = "VisualElements")]
        public VisualElements M3VisualElements { get; set; }

        [XmlElement(Namespace = "http://schemas.microsoft.com/appx/2013/manifest", ElementName = "VisualElements")]
        public VisualElements M2VisualElements { get; set; }

        public VisualElements GetVisualElements()
        {
            return M3VisualElements ?? M2VisualElements ?? M1VisualElements;
        }
    }
}
