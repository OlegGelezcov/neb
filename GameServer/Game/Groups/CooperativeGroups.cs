//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using ExitGames.Threading;
//using System.Threading;
//using Space.Server;
//using System.Collections;

//namespace Space.Game.Groups {
//    public class CooperativeGroups : IDisposable {

//        private Dictionary<string, CooperativeGroup> groupCache;
//        private readonly int maxLockMilliseconds;
//        private readonly ReaderWriterLockSlim readerWriterLock;

//        public const int MAX_GROUP_MEMBERS = 5;

//        public CooperativeGroups() {
//            this.maxLockMilliseconds = Settings.MaxLockWaitTimeMilliseconds;
//            this.groupCache = new Dictionary<string, CooperativeGroup>();
//            this.readerWriterLock = new ReaderWriterLockSlim();
//        }


//        public CooperativeGroup GetGroup(string id) {
//            CooperativeGroup result;

//            using (ReadLock.TryEnter(this.readerWriterLock, this.maxLockMilliseconds)) {
//                if (this.groupCache.TryGetValue(id, out result)) {
//                    return result;
//                }
//            }
//            return null;
//        }

//        public Hashtable RequestOpenedGroups() {
//            List<CooperativeGroup> foundedGroups = new List<CooperativeGroup>();
//            using (ReadLock.TryEnter(this.readerWriterLock, this.maxLockMilliseconds)) {
//                foreach (var pGroup in this.groupCache) {
//                    if (pGroup.Value.Opened() && pGroup.Value.MemberCount() < MAX_GROUP_MEMBERS) {
//                        foundedGroups.Add(pGroup.Value);
//                    }
//                }

//                Hashtable result = new Hashtable();
//                foreach (var fg in foundedGroups) {
//                    result.Add(fg.Id(), fg.RequestSearchInfo());
//                }
//                return result;
//            }
//        }


//        public bool AddGroup(CooperativeGroup group) {
//            CooperativeGroup tryGroup;

//            using (WriteLock.TryEnter(this.readerWriterLock, this.maxLockMilliseconds)) {
//                if (!this.groupCache.TryGetValue(group.Id(), out tryGroup)) {
//                    this.groupCache.Add(group.Id(), group);
//                    return true;
//                }
//            }
//            return false;
//        }



//        public bool RemoveGroup(string id) {
//            bool result = false;
//            var group = GetGroup(id);

//            using (WriteLock.TryEnter(this.readerWriterLock, this.maxLockMilliseconds)) {
//                if (group != null) {
//                    result = this.groupCache.Remove(id);
//                }
//            }
//            //if(group != null)
//            //    group.Dispose();
//            return result;
//        }

//        public int MemberCount(string id) {
//            var group = GetGroup(id);

//            if (group != null) {
//                return group.MemberCount();
//            }
//            return 0;
//        }

//        ~CooperativeGroups() {
//            this.Dispose(false);
//        }

//        protected virtual void Dispose(bool disposing) {
//            if (disposing) {
//                this.readerWriterLock.Dispose();
//                foreach (CooperativeGroup g in this.groupCache.Values) {
//                    g.Dispose();
//                }
//                this.groupCache.Clear();
//            }
//        }

//        public void Dispose() {
//            this.Dispose(true);
//            GC.SuppressFinalize(this);
//        }
//    }
//}
