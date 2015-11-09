using Common;
using System.Collections.Concurrent;
using System.Linq;
using System.Xml.Linq;
using System;
using System.Collections;

namespace SelectCharacter.Resources {
    public class PvpStoreItemCollection : IInfoSource {

        private readonly ConcurrentDictionary<string, PvpStoreItem> mStoreItems = new ConcurrentDictionary<string, PvpStoreItem>();

        public void Load(string path) {
            XDocument document = XDocument.Load(path);
            mStoreItems.Clear();

            var dmpList = document.Element("store").Elements("product").Select(e => {
                PvpStoreItem item = new PvpStoreItem(e);
                mStoreItems.TryAdd(item.type.ToLower(), item);
                return item.type.ToLower();
            }).ToList();
        }


        public PvpStoreItem GetItem(string type) {
            PvpStoreItem result;
            if(mStoreItems.TryGetValue(type.ToLower(), out result )) {
                return result;
            }
            return null;
        }

        private Hashtable mCachedInfo = null;

        public Hashtable GetInfo() {
            if (mCachedInfo == null) {
                mCachedInfo = new Hashtable();
                foreach (var pItem in mStoreItems) {
                    mCachedInfo.Add(pItem.Key, pItem.Value.GetInfo());
                }
            }
            return mCachedInfo;
        }

        public int count {
            get {
                return mStoreItems.Count;
            }
        }
    }
}
