using Common;
using MongoDB.Bson;
using ServerClientCommon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectCharacter.Store {
    public class PlayerStore : IInfoSource {

        public const int MAX_SLOTS = 300;

        public ObjectId Id { get; set; }

        private readonly object syncRoot = new object();

        public string login { get; set; } = string.Empty;
        public string gameRefID { get; set; } = string.Empty;
        public string characterID { get; set; } = string.Empty;
        public int credits { get; set; } = 0;

        //Hold pvp points of player
        public int pvpPoints { get; set; } = 0;

        public Dictionary<string, PlayerStoreItem> storeItems { get; set; } = new Dictionary<string, PlayerStoreItem>();

        private bool mChaged = false;
        public bool IsChanged() {
            return mChaged;
        }

        public void ResetChanged() {
            lock(syncRoot) {
                mChaged = false;
            }
        }

        public bool hasFreeSlots {
            get {
                lock(syncRoot) {
                    return (storeItems.Count < MAX_SLOTS);
                }
            }
        }

        //public bool ContainsItem(string storeItemID) {
        //    lock(syncRoot) {
        //        return storeItems.ContainsKey(storeItemID);
        //    }
        //}

            public void SetNewPrice(string storeItemID, int newPrice) {
            lock(syncRoot) {
                if(storeItems.ContainsKey(storeItemID)) {
                    storeItems[storeItemID].price = newPrice;
                    mChaged = true;
                }
            }
        }


        public PlayerStoreItem PutAtStore(Hashtable objectInfo, int count, int price) {
            PlayerStoreItem storeItem = new PlayerStoreItem {
                storeItemID = Guid.NewGuid().ToString(),
                count = count,
                objectInfo = objectInfo,
                time = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds,
                price = price
            };
            lock(syncRoot) {
                storeItems.Add(storeItem.storeItemID, storeItem);
                SelectCharacterApplication.Instance.Stores.SendStoreUpdate(this);
                mChaged = true;
                return storeItem;
            }
        }


        public bool RemoveFromStore(string storeItemID ) {
            lock(syncRoot) {
                bool result = storeItems.Remove(storeItemID);
                SelectCharacterApplication.Instance.Stores.SendStoreUpdate(this);
                mChaged = true;
                return result;
            }
        }

        public bool ContainsStoreItem(string storeItemID ) {
            lock(syncRoot) {
                return storeItems.ContainsKey(storeItemID);
            }
        }

        public bool AddPvpPoints(int count) {
            lock(syncRoot ) {
                pvpPoints += count;
                mChaged = true;
                SelectCharacterApplication.Instance.Stores.SendPvpPointsUpdate(this);
                return true;
            }
        }

        public bool RemovePvpPoints(int count) {
            lock(syncRoot ) {
                if(pvpPoints >= count ) {
                    pvpPoints -= count;
                    mChaged = true;
                    SelectCharacterApplication.Instance.Stores.SendPvpPointsUpdate(this);
                    return true;
                }
                return false;
            }
        }

        public void AddCredits(int creds) {
            lock(syncRoot) {
                credits += creds;
                mChaged = true;
                SelectCharacterApplication.Instance.Stores.SendStoreUpdate(this);
            }
        }

        public bool RemoveCredits(int creds) {
            lock(syncRoot) {
                if(credits >= creds) {
                    credits -= creds;
                    mChaged = true;
                    SelectCharacterApplication.Instance.Stores.SendStoreUpdate(this);
                    return true;
                }
                return false;
            }
        }

        public Hashtable GetInfo() {
            lock(syncRoot) {
                Hashtable hash = new Hashtable {
                { (int)SPC.Credits, credits },
                { (int)SPC.SlotsUsed, storeItems.Count },
                { (int)SPC.MaxSlots, MAX_SLOTS },
                { (int)SPC.PvpPoints, pvpPoints }
            };

                Hashtable storeItemsHash = new Hashtable();
                foreach (var itemPair in storeItems) {
                    storeItemsHash.Add(itemPair.Key, itemPair.Value.GetInfo());
                }

                hash.Add((int)SPC.Items, storeItemsHash);
                return hash;
            }
        }
    }
}
