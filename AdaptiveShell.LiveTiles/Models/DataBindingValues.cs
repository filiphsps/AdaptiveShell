using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveShell.LiveTiles.Models {
    public class DataBindingValues : IEnumerable<KeyValuePair<String, String>> {
        private Dictionary<String, String> _values;
        public UInt32 Version { get; private set; }

        public DataBindingValues(NotificationData data) {
            this._values = new Dictionary<String, String>();
            foreach (KeyValuePair<String, String> pair in data.Values) {
                this._values[pair.Key] = pair.Value;
            }

            this.Version = data.Version;
        }

        public Boolean TryGetValue(String key, out String value) {
            return this._values.TryGetValue(key, out value);
        }

        public void Update(IEnumerable<KeyValuePair<String, String>> values) {
            foreach (KeyValuePair<String, String> pair in values) {
                this._values[pair.Key] = pair.Value;
            }

            String[] newKeys = values.Select(i => i.Key).ToArray();
            String[] oldKeys = this._values.Keys.ToArray();
            foreach (String oldKey in oldKeys) {
                // If key no longer exists
                if (!newKeys.Contains(oldKey)) {
                    // Remove the value
                    this._values.Remove(oldKey);
                }
            }
        }

        public IEnumerator<KeyValuePair<String, String>> GetEnumerator() {
            return this._values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
    }
}
