using Common;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using ServerClientCommon;

namespace SelectCharacter.Notifications {
    public class CharacterNotifications : IInfoSource {
        public ObjectId Id { get; set; }
        public string characterId { get; set; }
        public Dictionary<string, Notification> notifications { get; set; }

        private readonly object syncRoot = new object();

        public bool Contains(Notification n) {
            lock(syncRoot) {
                foreach(var pNotification in notifications) {
                    if(pNotification.Value.uniqueID == n.uniqueID) {
                        return true;
                    }
                }
                return false;
            }
        }

        public void Add(Notification notification) {
            lock(syncRoot) {
                notifications[notification.id] = notification;
            }
        }

        public void Remove(string nID) {
            lock(syncRoot) {
                if(notifications.ContainsKey(nID)) {
                    notifications.Remove(nID);
                }
            }
        }

        public Notification Get(string nID) {
            lock (syncRoot) {
                Notification result = null;
                notifications.TryGetValue(nID, out result);
                return result;
            }
        }

        public Hashtable GetInfo() {
            Hashtable result = new Hashtable {
                {(int)SPC.CharacterId, characterId},
            };
            Hashtable nHash = new Hashtable();
            lock (syncRoot) {
                
                foreach(var pair in notifications) {
                    nHash.Add(pair.Key, pair.Value.GetInfo());
                }
            }
            result.Add((int)SPC.Notifications, nHash);
            return result;
        }

    }
}
