//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Common;
//using System.Collections;
//using ServerClientCommon;

//namespace Nebula.Client {
//    public class ClientCooperativeGroup : IInfoParser {

//        private Dictionary<string, ClientCooperativeGroupMember> members;
//        private bool opened;
//        private string id;



//        public ClientCooperativeGroup() {
//            this.members = new Dictionary<string, ClientCooperativeGroupMember>();
//        }


//        public void ParseInfo(Hashtable info) {
//            this.members.Clear();

//            Hashtable members = info.GetValue<Hashtable>((int)SPC.Members, new Hashtable());

//            foreach (DictionaryEntry entry in members) {
//                this.members.Add(entry.Key.ToString(), new ClientCooperativeGroupMember(entry.Value as Hashtable));
//            }

//            this.opened = info.GetValue<bool>((int)SPC.Opened, true);
//            this.id = info.GetValue<string>((int)SPC.Id, string.Empty);
//        }

//        public bool HasGroup() {
//            if (this.members == null) {
//                return false;
//            }
//            if (this.members.Count <= 1) {
//                return false;
//            }
//            return true;
//        }

//        public List<ClientCooperativeGroupMember> Members() {
//            return this.members.OrderBy(d => d.Key).Select(d => d.Value).ToList();
//        }

//        public Dictionary<string, ClientCooperativeGroupMember> MembersDict() {
//            return this.members;
//        }


//        /// <summary>
//        /// Get member object for character id
//        /// </summary>
//        public bool TryGetMember(string characterId, out ClientCooperativeGroupMember member) {
//            return this.members.TryGetValue(characterId, out member);
//        }

//        public bool IsLeader(string characterId) {
//            ClientCooperativeGroupMember member = null;
//            if (TryGetMember(characterId, out member)) {
//                return member.IsLeader();
//            }
//            return false;
//        }

//        public int MemberCount() {
//            return this.members.Count;
//        }

//        public bool Opened() {
//            return this.opened;
//        }

//        public string Id() {
//            return this.id;
//        }
//    }
//}
