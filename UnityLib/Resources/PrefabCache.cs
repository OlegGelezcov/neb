namespace Nebula.Resources {
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    public static class PrefabCache {
        private static Dictionary<string, GameObject> cache;

        static PrefabCache() {
            cache = new Dictionary<string, GameObject>();
        }
        public static GameObject Get(string path) {
            if (cache.ContainsKey(path))
                return cache[path];
            else {
                //Debug.Log("RESOURCE PATH: " + path);
                GameObject obj = UnityEngine.Resources.Load<GameObject>(path);
                if (obj)
                    cache.Add(path, obj);
                return obj;
            }
        }
    }
}