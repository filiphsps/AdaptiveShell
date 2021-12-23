using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveShell.LiveTiles {
    public sealed class NotificationData {
        public IEnumerable<KeyValuePair<String, String>> Values { get; private set; }

        public UInt32 Version { get; private set; }

        public NotificationData(IEnumerable<KeyValuePair<String, String>> values, UInt32 version) {
            this.Values = values;
            this.Version = version;
        }

        public NotificationData() {
            this.Values = new KeyValuePair<String, String>[0];
            this.Version = 0;
        }
    }
}
