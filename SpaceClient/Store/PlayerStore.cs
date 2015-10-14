using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using ServerClientCommon;

namespace Nebula.Client.Store {
    public class PlayerStore : IInfoParser {
        public int credits { get; private set; } = 0;
        public Dictionary<string, StoreItem> storeItems { get; private set; } = new Dictionary<string, StoreItem>();
        public int slotsUSed { get; private set; } = 0;
        public int maxSlots { get; private set; } = 0;

        public void ParseInfo(Hashtable info) {
            credits =   info.Value<int>((int)SPC.Credits,   0);
            slotsUSed = info.Value<int>((int)SPC.SlotsUsed, 0);
            maxSlots =  info.Value<int>((int)SPC.MaxSlots,  0);

            if(storeItems == null ) {
                storeItems = new Dictionary<string, StoreItem>();
            }
            storeItems.Clear();

            Hashtable itemHash = info.Value<Hashtable>((int)SPC.Items, new Hashtable());
            foreach(DictionaryEntry entry in itemHash) {
                string id = entry.Key.ToString();
                StoreItem item = new StoreItem(entry.Value as Hashtable);
                storeItems.Add(id, item);
            }
        }

        public PlayerStore() { }

        public PlayerStore(Hashtable hash) {
            ParseInfo(hash);
        }
    }
}
