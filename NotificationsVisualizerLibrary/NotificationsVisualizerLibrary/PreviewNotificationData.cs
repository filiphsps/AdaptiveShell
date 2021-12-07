using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationsVisualizerLibrary
{
    public sealed class PreviewNotificationData
    {
        public IEnumerable<KeyValuePair<string, string>> Values { get; private set; }

        public uint Version { get; private set; }

        public PreviewNotificationData(IEnumerable<KeyValuePair<string, string>> values, uint version)
        {
            Values = values;
            Version = version;
        }

        public PreviewNotificationData()
        {
            Values = new KeyValuePair<string, string>[0];
            Version = 0;
        }
    }
}
