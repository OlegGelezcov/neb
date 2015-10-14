using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using System.Collections;

namespace Nebula.Client {
    public class ClientSearchGroupsResult : IInfoParser {

        private Dictionary<string, ClientSearchGroup> groups = new Dictionary<string, ClientSearchGroup>();

        public ClientSearchGroupsResult(){

        }

        public ClientSearchGroupsResult(Hashtable info) {
            this.ParseInfo(info);
        }

        public void ParseInfo(Hashtable info) {
            if (this.groups == null) {
                this.groups = new Dictionary<string, ClientSearchGroup>();
            }
            this.groups.Clear();
            foreach (DictionaryEntry entry in info) {
                this.groups.Add(entry.Key.ToString(), new ClientSearchGroup(entry.Key.ToString(), entry.Value as Hashtable));
            }
        }

        public Dictionary<string, ClientSearchGroup> Groups() {
            return this.groups;
        }

        public void Clear() {
            groups.Clear();
        }
    }
}
