using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using ServerClientCommon;

namespace Nebula.Client {
    public class GroupMember : IInfoParser {
        public string login { get; private set; }
        public string gameRefID { get; private set; }
        public string characterID { get; private set; }
        public bool isLeader { get; private set; }
        public string worldID { get; private set; }

        public void ParseInfo(Hashtable info) {
            login = info.Value<string>((int)SPC.Login);
            gameRefID = info.Value<string>((int)SPC.GameRefId);
            characterID = info.Value<string>((int)SPC.CharacterId);
            isLeader = info.Value<bool>((int)SPC.IsLeader);
            worldID = info.Value<string>((int)SPC.WorldId);
        }

        public GroupMember() { }
        public GroupMember(Hashtable info ) { ParseInfo(info); }

        public override string ToString() {
            return string.Format("{0}, leader = {1}, world = {2}", login, isLeader, worldID);
        }
    }
}
