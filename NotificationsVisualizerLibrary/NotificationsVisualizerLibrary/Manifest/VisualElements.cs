using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.UI;

namespace NotificationsVisualizerLibrary.Manifest
{
    public sealed class VisualElements
    {
        private string _square30X30Logo;
        private string _square44X44Logo;

        [XmlAttribute]
        public string DisplayName { get; set; }

        [XmlAttribute]
        public string Square150x150Logo { get; set; }

        [XmlAttribute]
        public string Square30x30Logo
        {
            get { return _square30X30Logo; }
            set
            {
                _square30X30Logo = value;
                _square44X44Logo = value;
            }
        }

        [XmlAttribute]
        public string Square44x44Logo
        {
            get { return _square44X44Logo; }
            set
            {
                _square44X44Logo = value;
                _square30X30Logo = value;
            }
        }

        [XmlAttribute]
        public Color BackgroundColor { get; set; } = Colors.Blue;

        [XmlElement(Namespace = "http://schemas.microsoft.com/appx/2014/manifest")]
        public DefaultTile DefaultTile { get; set; }
    }
}
