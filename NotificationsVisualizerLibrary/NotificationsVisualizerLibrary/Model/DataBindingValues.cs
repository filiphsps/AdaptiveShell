using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationsVisualizerLibrary.Model
{
    internal class DataBindingValues : IEnumerable<KeyValuePair<string, string>>
    {
        private Dictionary<string, string> _values;
        public uint Version { get; private set; }

        public DataBindingValues(PreviewNotificationData data)
        {
            _values = new Dictionary<string, string>();
            foreach (var pair in data.Values)
            {
                _values[pair.Key] = pair.Value;
            }
            Version = data.Version;
        }

        public bool TryGetValue(string key, out string value)
        {
            return _values.TryGetValue(key, out value);
        }

        public void Update(IEnumerable<KeyValuePair<string, string>> values)
        {
            foreach (var pair in values)
            {
                _values[pair.Key] = pair.Value;
            }

            var newKeys = values.Select(i => i.Key).ToArray();
            var oldKeys = _values.Keys.ToArray();
            foreach (var oldKey in oldKeys)
            {
                // If key no longer exists
                if (!newKeys.Contains(oldKey))
                {
                    // Remove the value
                    _values.Remove(oldKey);
                }
            }
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
