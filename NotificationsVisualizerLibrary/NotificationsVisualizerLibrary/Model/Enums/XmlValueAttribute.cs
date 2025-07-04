using System;

namespace NotificationsVisualizerLibrary.Model.Enums
{
    internal class XmlValueAttribute : Attribute
    {
        public String XmlValue { get; private set; }

        public XmlValueAttribute(String xmlValue)
        {
            this.XmlValue = xmlValue;
        }
    }
}
