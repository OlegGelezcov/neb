using Common;
using System.Collections;
using System.Collections.Generic;

namespace Nebula.Client.PvpStore {
    public class PvpStore : IInfoParser {

        private readonly List<PvpStoreItem> mStoreItems = new List<PvpStoreItem>();

        public PvpStore() { }

        public PvpStore(Hashtable info) {
            ParseInfo(info);
        }

        public void ParseInfo(Hashtable info) {
            mStoreItems.Clear();

            foreach(DictionaryEntry entry in info ) {
                string type = (string)entry.Key;
                Hashtable hash = entry.Value as Hashtable;
                if(hash != null ) {
                    mStoreItems.Add(new PvpStoreItem(type, hash));
                }
            }
        }

        public PvpStoreItem GetItem(string type) {
            foreach(var it in mStoreItems) {
                if(it.type.ToLower() == type.ToLower() ) {
                    return it;
                }
            }
            return null;
        }

        public List<PvpStoreItem> items {
            get {
                return mStoreItems;
            }
        }
    }
}
