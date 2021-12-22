using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AdaptiveShell.LiveTiles.Manifest {
    public sealed class Applications {
        [XmlElement(ElementName = "Application")]
        public IList<Application> Application { get; set; }
    }
}
