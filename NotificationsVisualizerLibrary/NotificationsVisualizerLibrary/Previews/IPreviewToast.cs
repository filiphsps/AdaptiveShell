using NotificationsVisualizerLibrary.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;

namespace NotificationsVisualizerLibrary
{
    public interface IPreviewToast
    {
        ParseResult Initialize(XmlDocument content);
        ParseResult Initialize(XmlDocument content, PreviewNotificationData data);
        void Update(PreviewNotificationData data);
        PreviewToastProperties Properties { get; set; }
        int OSBuildNumber { get; set; }
    }
}
