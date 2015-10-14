using System.Collections.Generic;
using UnityEngine;

namespace Nebula.Resources {
    public class ObjectSubCache<T, U> where T : Object {

        private Dictionary<U, T> cachedObjects = new Dictionary<U, T>();

        public T GetObject(U key, string globalCacheObjectPath) {
            if (this.cachedObjects.ContainsKey(key)) {
                return this.cachedObjects[key];
            } else {
                T obj = ObjectCache<T>.GetObject(globalCacheObjectPath);
                if (((object)obj) != null) {
                    this.cachedObjects.Add(key, obj);
                }
                return obj;
            }
        }

        public void Preload(U key, string globalCacheObjectPath) {
            var obj = this.GetObject(key, globalCacheObjectPath);
            if (obj != default(T)) {

            }
        }
    }
}
