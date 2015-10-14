using MongoDB.Driver;
using NebulaCommon;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectCharacter.Notifications {
    public class NotificationCache {

        private ConcurrentDictionary<string, DbObjectWrapper<CharacterNotifications>> cache = new ConcurrentDictionary<string, DbObjectWrapper<CharacterNotifications>>();

        public bool TryGetNotification(string characterId, out CharacterNotifications notification) {
            DbObjectWrapper<CharacterNotifications> wrapper = null;
            if(cache.TryGetValue(characterId, out wrapper)) {
                notification = wrapper.Data;
                return true;
            }
            notification = null;
            return false;
        }

        public bool TryGetWrapper(string characterId, out DbObjectWrapper<CharacterNotifications> notification) {
            return cache.TryGetValue(characterId, out notification);
        }

        public void SetNotification(CharacterNotifications notification) {
            bool success = true;

            if(cache.ContainsKey(notification.characterId)) {
                DbObjectWrapper<CharacterNotifications> oldNotification = null;
                if(!cache.TryRemove(notification.characterId, out oldNotification)) {
                    success = false;
                }
            }

            if(success) {
                cache.TryAdd(notification.characterId, new DbObjectWrapper<CharacterNotifications> { Changed = true, Data = notification });
            }
        }

        public void RemoveNotification(string characterId ) {
            DbObjectWrapper<CharacterNotifications> notification;
            cache.TryRemove(characterId, out notification);
        }

        public void SetChanged(string characterId, bool changed ) {
            DbObjectWrapper<CharacterNotifications> notification = null;
            if(cache.TryGetValue(characterId, out notification)) {
                notification.Changed = changed;
            }
        }

        public void DumpChangesToDatabase(MongoCollection<CharacterNotifications> collection) {
            foreach(var notification in  cache) {
                if(notification.Value.Changed) {
                    collection.Save(notification.Value.Data);
                    notification.Value.Changed = false;
                }
            }
        }

        public void Clear() {
            cache.Clear();
        }

        public void ForceSave(string characterId, MongoCollection<CharacterNotifications> collection) {
            DbObjectWrapper<CharacterNotifications> notification = null;
            if(cache.TryGetValue(characterId, out notification)) {
                collection.Save(notification.Data);
                notification.Changed = false;
            }
        }
    }

    
}
