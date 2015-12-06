using Common;
using ServerClientCommon;
using ExitGames.Client.Photon;
using System.Collections.Generic;
using Nebula.Client.Utils;

namespace Nebula.Client.Notifications {
    public class CharacterNotifications : IInfoParser {

        public string characterID { get; private set; }

        public Dictionary<string, Notification> notifications { get; private set; }
        public Dictionary<string, Notification> newNotifications { get; private set; }

        public CharacterNotifications() {
            characterID = string.Empty;
            notifications = new Dictionary<string, Notification>();
        }

        public CharacterNotifications(Hashtable info) { ParseInfo(info); }



        public void ParseInfo(Hashtable info) {
            if (notifications == null) {
                notifications = new Dictionary<string, Notification>();
            }

            Dictionary<string, Notification> oldNotifications = null;
            if (notifications.Count > 0) {
                oldNotifications = new Dictionary<string, Notification>();
                foreach (var pk in notifications) {
                    oldNotifications.Add(pk.Key, pk.Value);
                }
            }

            notifications.Clear();

            characterID = info.Value<string>((int)SPC.CharacterId);
            Hashtable notificationHash = info.GetValueHash((int)SPC.Notifications);


            foreach (System.Collections.DictionaryEntry entry in notificationHash) {
                string key = entry.Key.ToString();
                Hashtable nHash = entry.Value as Hashtable;
                notifications.Add(key, new Notification(nHash));
            }

            CollectNewNotifications(oldNotifications);
        }

        private void CollectNewNotifications(Dictionary<string, Notification> oldNotifications) {
            if (newNotifications == null) {
                newNotifications = new Dictionary<string, Notification>();
            }
            newNotifications.Clear();
            foreach (var pk in notifications) {
                if (oldNotifications == null || (!oldNotifications.ContainsKey(pk.Key))) {
                    newNotifications.Add(pk.Key, pk.Value);
                }
            }
        }

        public void Clear() {
            characterID = string.Empty;
            if (notifications != null) {
                notifications.Clear();
            }
        }

        public int notHandledCount {
            get {
                int cnt = 0;
                if (notifications == null) { return cnt; }
                foreach (var n in notifications) {
                    if (!n.Value.handled) {
                        cnt++;
                    }
                }
                return cnt;
            }
        }
    }
}
