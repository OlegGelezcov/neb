﻿using Common;
using ServerClientCommon;
using ExitGames.Client.Photon;
using System.Collections.Generic;
using Nebula.Client.Utils;

namespace Nebula.Client {
    public class ClientSearchGroup : IInfoParser {

        private Dictionary<string, ClientSearchGroupMember> members = new Dictionary<string, ClientSearchGroupMember>();
        private string id;

        public ClientSearchGroup(string id, Hashtable info) {
            this.id = id;
            ParseInfo(info);
        }

        public void ParseInfo(Hashtable info) {

            if (this.members == null) {
                this.members = new Dictionary<string, ClientSearchGroupMember>();
            }
            this.members.Clear();

            Hashtable membersHash = info.GetValueHash((int)SPC.Members);
            foreach (System.Collections.DictionaryEntry entry in membersHash) {
                this.members.Add(entry.Key.ToString(), new ClientSearchGroupMember(entry.Value as Hashtable));
            }
        }

        public Dictionary<string, ClientSearchGroupMember> Members() {
            return this.members;
        }

        public string Id() {
            return this.id;
        }
    }
}
