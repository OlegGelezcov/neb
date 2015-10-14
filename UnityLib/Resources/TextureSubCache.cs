namespace Nebula.Resources {

    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;

    //Optimaized texture cache used classes for caching often used textures and don't request global TextureCache many times
    public class TextureSubCache<T> {
        private Dictionary<T, Texture2D> cachedTextures = new Dictionary<T, Texture2D>();

        public Texture2D GetTexture(T key, string globalCacheTexturePath) {
            if (this.cachedTextures.ContainsKey(key)) {
                return this.cachedTextures[key];
            } else {
                Texture2D tex = BaseTextureCache.Get(globalCacheTexturePath);
                if (tex != null) {
                    this.cachedTextures.Add(key, tex);
                }
                return tex;
            }
        }
    }

}