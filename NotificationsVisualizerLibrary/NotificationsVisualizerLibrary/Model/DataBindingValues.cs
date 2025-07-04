using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NotificationsVisualizerLibrary.Model
{
    internal partial class DataBindingValues : IEnumerable<KeyValuePair<String, String>>
    {
        private Dictionary<String, String> _values;
        public UInt32 Version { get; private set; }

        public DataBindingValues(PreviewNotificationData data)
        {
            this._values = new Dictionary<String, String>();
            foreach (var pair in data.Values)
            {
                this._values[pair.Key] = pair.Value;
            }
            this.Version = data.Version;
        }

        public Boolean TryGetValue(String key, out String value)
        {
            return this._values.TryGetValue(key, out value);
        }

        public void Update(IEnumerable<KeyValuePair<String, String>> values)
        {
            foreach (var pair in values)
            {
                this._values[pair.Key] = pair.Value;
            }

            var newKeys = values.Select(i => i.Key).ToArray();
            var oldKeys = this._values.Keys.ToArray();
            foreach (var oldKey in oldKeys)
            {
                // If key no longer exists
                if (!newKeys.Contains(oldKey))
                {
                    // Remove the value
                    this._values.Remove(oldKey);
                }
            }
        }

        public IEnumerator<KeyValuePair<String, String>> GetEnumerator()
        {
            return this._values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
