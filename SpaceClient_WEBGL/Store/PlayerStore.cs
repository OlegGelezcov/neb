using Common;
using System.Collections.Generic;
using ServerClientCommon;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;

namespace Nebula.Client.Store {
    public class PlayerStore : IInfoParser {
        public int credits { get; private set; } = 0;
        public Dictionary<string, StoreItem> storeItems { get; private set; } = new Dictionary<string, StoreItem>();
        public int slotsUSed { get; private set; } = 0;
        public int maxSlots { get; private set; } = 0;

        public int pvpPoints { get; private set; } = 0;

        public void SetPvpPoints(int count) {
            pvpPoints = count;
        }

        public void ParseInfo(Hashtable info) {
            credits = info.GetValueInt((int)SPC.Credits);
            slotsUSed = info.GetValueInt((int)SPC.SlotsUsed);
            maxSlots = info.GetValueInt((int)SPC.MaxSlots);
            pvpPoints = info.GetValueInt((int)SPC.PvpPoints);

            if (storeItems == null) {
                storeItems = new Dictionary<string, StoreItem>();
            }
            storeItems.Clear();

            Hashtable itemHash = info.GetValueHash((int)SPC.Items);
            foreach (System.Collections.DictionaryEntry entry in itemHash) {
                string id = entry.Key.ToString();
                StoreItem item = new StoreItem(entry.Value as Hashtable);
                storeItems.Add(id, item);
            }
        }

        public PlayerStore() { }

        public PlayerStore(Hashtable hash) {
            ParseInfo(hash);
        }

        public void Clear() {
            credits = 0;
            pvpPoints = 0;
            slotsUSed = 0;
            maxSlots = 0;
            if (storeItems != null) {
                storeItems.Clear();
            }
        }
    }
}
