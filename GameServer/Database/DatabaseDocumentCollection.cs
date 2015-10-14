using ExitGames.Logging;
using NebulaCommon;
using System.Collections.Generic;

namespace Space.Database {
    public class DatabaseDocumentCollection<T> : Dictionary<string, DbObjectWrapper<IDatabaseDocument<T>>> where T : class {

        private readonly object syncObject = new object();
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public bool TryGetObject(string gameRefId, string characterId, out IDatabaseDocument<T> result ) {
            result = null;

            lock(syncObject) {
                DbObjectWrapper<IDatabaseDocument<T>> wrapper;
                if(TryGetValue(gameRefId, out wrapper)) {
                    if(wrapper.Data != null && wrapper.Data.CharacterId == characterId) {
                        result = wrapper.Data;
                        return true;
                    } else {
                        result = null;
                        Remove(gameRefId);
                    }
                }
            }

            return false;
        }

        public bool TryGetWrapper(string gameRefId, string characterId, out DbObjectWrapper<IDatabaseDocument<T>> result) {
            result = null;

            lock (syncObject) {
                DbObjectWrapper<IDatabaseDocument<T>> wrapper;
                if (TryGetValue(gameRefId, out wrapper)) {
                    if (wrapper.Data != null && wrapper.Data.CharacterId == characterId) {
                        result = wrapper;
                        return true;
                    } else {
                        result = null;
                        Remove(gameRefId);
                    }
                }
            }

            return false;
        }

        public void Set(string gameRefId, IDatabaseDocument<T> document, bool changed) {
            lock(syncObject) {
                if(ContainsKey(gameRefId)) {
                    Remove(gameRefId);
                }
                Add(gameRefId, new DbObjectWrapper<IDatabaseDocument<T>> { Changed = changed, Data = document });
            }
        }

        /// <param name="gameRefID"></param>
        public bool RemoveObject(string gameRefID) {
            lock(syncObject) {
                return Remove(gameRefID);
            }
        }
    }
}
