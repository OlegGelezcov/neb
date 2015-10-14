namespace Nebula.Resources {
    using System.Collections.Generic;
    using UnityEngine;

    public class PrefabSubCache<T> {
        private Dictionary<T, GameObject> cache;

        public PrefabSubCache() {
            this.cache = new Dictionary<T, GameObject>();
        }

        public GameObject Prefab(T key, string fullPath) {
            if (!this.cache.ContainsKey(key)) {
                GameObject prefab = PrefabCache.Get(fullPath);
                if (prefab) {
                    this.cache.Add(key, prefab);
                }
            }
            if (this.cache.ContainsKey(key))
                return this.cache[key];
            return null;
        }
    }
}