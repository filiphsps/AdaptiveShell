using System;
using System.Collections.Generic;

namespace NotificationsVisualizerLibrary
{
    public sealed class PreviewNotificationData
    {
        public IEnumerable<KeyValuePair<String, String>> Values { get; private set; }

        public UInt32 Version { get; private set; }

        public PreviewNotificationData(IEnumerable<KeyValuePair<String, String>> values, UInt32 version)
        {
            this.Values = values;
            this.Version = version;
        }

        public PreviewNotificationData()
        {
            this.Values = new KeyValuePair<String, String>[0];
            this.Version = 0;
        }
    }
}
