using System;
using System.Xml.Serialization;

namespace NotificationsVisualizerLibrary.Manifest
{
    public sealed class Application
    {
        [XmlAttribute]
        public String Id { get; set; }

        [XmlElement(ElementName = "VisualElements")]
        public VisualElements M1VisualElements { get; set; }


        [XmlElement(Namespace = "http://schemas.microsoft.com/appx/2014/manifest", ElementName = "VisualElements")]
        public VisualElements M3VisualElements { get; set; }

        [XmlElement(Namespace = "http://schemas.microsoft.com/appx/2013/manifest", ElementName = "VisualElements")]
        public VisualElements M2VisualElements { get; set; }

        public VisualElements GetVisualElements()
        {
            return this.M3VisualElements ?? this.M2VisualElements ?? this.M1VisualElements;
        }
    }
}
