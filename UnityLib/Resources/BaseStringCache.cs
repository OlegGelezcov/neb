using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Resources {
    public static class BaseStringCache {
        private static Dictionary<string, string> cache;

        static BaseStringCache() {
            cache = new Dictionary<string, string>();
        }

        public static string Get(string key) {
            if (cache.ContainsKey(key)) {
                return cache[key];
            } else {
                cache[key] = DataResources.Instance.String(key).Trim();
                return cache[key];
            }
        }

    }
}
