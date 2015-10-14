using Common;
using ServerClientCommon;
using System.Collections;
using System.Collections.Generic;

namespace Nebula.Client.Notifications {
    public class CharacterNotifications : IInfoParser {

        public string characterID { get; private set; }

        public Dictionary<string, Notification> notifications { get; private set; }

        public CharacterNotifications() {
            characterID = string.Empty;
            notifications = new Dictionary<string, Notification>();
        }

        public CharacterNotifications(Hashtable info) { ParseInfo(info); }

        public void ParseInfo(Hashtable info) {
            if(notifications == null) {
                notifications = new Dictionary<string, Notification>();
            }
            notifications.Clear();

            characterID                     = info.Value<string>((int)SPC.CharacterId);
            Hashtable notificationHash      = info.Value<Hashtable>((int)SPC.Notifications);


            foreach(DictionaryEntry entry in notificationHash) {
                string key = entry.Key.ToString();
                Hashtable nHash = entry.Value as Hashtable;
                notifications.Add(key, new Notification(nHash));
            }
        }

        public void Clear() {
            characterID = string.Empty;
            if(notifications != null ) {
                notifications.Clear();
            }
        }

        public int notHandledCount {
            get {
                int cnt = 0;
                if(notifications == null ) { return cnt;  }
                foreach(var n in notifications) {
                    if(!n.Value.handled) {
                        cnt++;
                    }
                }
                return cnt;
            }
        }
    }
}
