using System.Collections.Generic;
using UnityEngine;

namespace Nebula.Resources {
    public static class BaseTextureCache {
        private static Dictionary<string, Texture2D> cache;

        static BaseTextureCache() {
            cache = new Dictionary<string, Texture2D>();
        }

        public static Texture2D Get(string path) {
            if (cache.ContainsKey(path)) {
                return cache[path];
            } else {
                Texture2D tex = UnityEngine.Resources.Load<Texture2D>(path);
                if (tex != null)
                    cache.Add(path, tex);
                return tex;
            }
        }
    }
}
