namespace Nebula.Resources {
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;

    public class StringSubCache<T> {
        private Dictionary<T, string> cachedStrings = new Dictionary<T, string>();

        public string String(T key, string globalKey) {
            if (this.cachedStrings.ContainsKey(key))
                return this.cachedStrings[key];
            else {
                string s = BaseStringCache.Get(globalKey);
                if (s != null) {
                    this.cachedStrings.Add(key, s);
                    return s;
                }
                return globalKey;
            }
        }
    }

}