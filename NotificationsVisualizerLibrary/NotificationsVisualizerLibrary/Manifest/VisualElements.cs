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
        private String _square30X30Logo;
        private String _square44X44Logo;

        [XmlAttribute]
        public String DisplayName { get; set; }

        [XmlAttribute]
        public String Square150x150Logo { get; set; }

        [XmlAttribute]
        public String Square30x30Logo
        {
            get { return this._square30X30Logo; }
            set
            {
                this._square30X30Logo = value;
                this._square44X44Logo = value;
            }
        }

        [XmlAttribute]
        public String Square44x44Logo
        {
            get { return this._square44X44Logo; }
            set
            {
                this._square44X44Logo = value;
                this._square30X30Logo = value;
            }
        }

        [XmlAttribute]
        public Color BackgroundColor { get; set; } = Colors.Blue;

        [XmlElement(Namespace = "http://schemas.microsoft.com/appx/2014/manifest")]
        public DefaultTile DefaultTile { get; set; }
    }
}
