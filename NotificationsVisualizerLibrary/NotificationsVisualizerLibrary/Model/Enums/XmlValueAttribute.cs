using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationsVisualizerLibrary.Model.Enums
{
    internal class XmlValueAttribute : Attribute
    {
        public string XmlValue { get; private set; }

        public XmlValueAttribute(string xmlValue)
        {
            XmlValue = xmlValue;
        }
    }
}
