using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Common
{
    public class SafeDictionary<TKey, TValue>
    {
        private readonly object _padlock = new object();
        private readonly Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();

        public TValue this[TKey key]
        {
            get
            {
                lock(_padlock)
                {
                    return dictionary[key];
                }
            }
            set
            {
                lock(_padlock)
                {
                    dictionary[key] = value;
                }
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            lock(_padlock)
            {
                return dictionary.TryGetValue(key, out value);
            }
        }

        public bool ContainsKey(TKey key)
        {
            lock(_padlock)
            {
                return dictionary.ContainsKey(key);
            }
        }

        public void Add(TKey key, TValue value)
        {
            lock (_padlock)
            {
                dictionary.Add(key, value);
            }
        }
    }
}
