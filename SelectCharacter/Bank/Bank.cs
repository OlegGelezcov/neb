using Common;
using Space.Game.Inventory;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Nebula.Inventory;
using GameMath;
using ServerClientCommon;

namespace SelectCharacter.Bank {
    public class Bank : IInfo {

        public const int DEFAULT_MAX_SLOTS = 50;


        public int maxSlots { get; private set; }

        public ConcurrentDictionary<string, ServerInventoryItem> items { get; private set; }

        public Bank() {
            maxSlots = DEFAULT_MAX_SLOTS;
            items = new ConcurrentDictionary<string, ServerInventoryItem>();
        }

        public int slotsUsed {
            get {
                return items.Count;
            }
        }

        public int freeSlots {
            get {
                return Mathf.ClampLess(maxSlots - slotsUsed, 0);
            }
        }

        public void AddMaxSlot(int count) {
            maxSlots += count;
        }

        public bool AddItem(Hashtable item) {
            int count = 0;
            var obj = InventoryUtils.Create(item, out count);
            return AddItem(obj, count);
        }

        public bool AddItem(IInventoryObject obj, int count) {
            if (obj != null && count > 0) {
                ServerInventoryItem servItem = new ServerInventoryItem(obj, count);
                if (HasSpaceForItems(servItem)) {
                    if (items.ContainsKey(servItem.Object.Id)) {
                        ServerInventoryItem existingItem = null;
                        if (items.TryGetValue(servItem.Object.Id, out existingItem)) {
                            existingItem.Add(count);
                            return true;
                        }
                    } else {
                        items.TryAdd(servItem.Object.Id, servItem);
                        return true;
                    }
                }
            }
            return false;
        }

        public bool RemoveItem(string id, int count) {
            if(items.ContainsKey(id)) {
                ServerInventoryItem it;
                if(items.TryGetValue(id, out it)) {
                    it.Remove(count);
                    if(!it.Has) {
                        ServerInventoryItem removedItem;
                        items.TryRemove(id, out removedItem);
                    }
                    return true;
                }
            }
            return false;
        }

        public int GetItemCount(string id) {
            ServerInventoryItem it;
            if(items.TryGetValue(id, out it)) {
                return it.Count;
            }
            return 0;
        }

        public int SlotsForItem(ServerInventoryItem it) {
            if(items.ContainsKey(it.Object.Id)) {
                return 0;
            } else {
                return 1;
            }
        }

        public int SlotsForItems(params ServerInventoryItem[] testItems) {
            int total = 0;
            foreach(var ti in testItems) {
                total += SlotsForItem(ti);
            }
            return total;
        }

        public bool HasSpaceForItems(params ServerInventoryItem[] testItems) {
            int need = SlotsForItems(testItems);
            return (freeSlots >= need);
        }

        public bool HasSpaceForItems(params string[] ids) {
            int total = 0;
            foreach(var id in ids) {
                if(!items.ContainsKey(id)) {
                    total++;
                }
            }
            return (freeSlots >= total);
        }

        public ServerInventoryItem GetItem(string id) {
            ServerInventoryItem result;
            if(items.TryGetValue(id, out result)) {
                return result;
            }
            return null;
        }

        public Hashtable GetInfo() {
            Hashtable itemHash = new Hashtable();
            foreach(var it in items ) {
                itemHash.Add(it.Key, it.Value.GetInfo());
            }

            return new Hashtable {
                {(int)SPC.MaxSlots, maxSlots },
                {(int)SPC.Items, itemHash }
            };
        }



        public void ParseInfo(Hashtable info) {
            maxSlots = info.GetValue<int>((int)SPC.MaxSlots, 0);
            Hashtable itemHash = info.GetValue<Hashtable>((int)SPC.Items, new Hashtable());
            if(items == null ) {
                items = new ConcurrentDictionary<string, ServerInventoryItem>();
            }

            if(itemHash != null ) {
                foreach(DictionaryEntry entry in itemHash) {
                    Hashtable itemInfo = entry.Value as Hashtable;
                    if(itemInfo != null ) {
                        AddItem(itemInfo);
                    }
                }
            }
        }
    }
}
