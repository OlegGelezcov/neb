using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using ServerClientCommon;

namespace NebulaCommon.Group {
    public class GroupMember : IInfo {
        public string login { get; set; }
        public string gameRefID { get; set; }
        public string characterID { get; set; }
        public bool isLeader { get; set; }
        public string worldID { get; set; }
        public int workshop { get; set; }
        public int exp { get; set; }

        public Hashtable GetInfo() {
            return new Hashtable {
                { (int)SPC.Login, login },
                { (int)SPC.GameRefId, gameRefID },
                { (int)SPC.CharacterId, characterID },
                { (int)SPC.IsLeader, isLeader },
                { (int)SPC.WorldId, worldID },
                { (int)SPC.Workshop, workshop },
                { (int)SPC.Exp, exp }
            };
        }

        public void ParseInfo(Hashtable info) {
            login = info.Value<string>((int)SPC.Login);
            gameRefID = info.Value<string>((int)SPC.GameRefId);
            characterID = info.Value<string>((int)SPC.CharacterId);
            isLeader = info.Value<bool>((int)SPC.IsLeader);
            worldID = info.Value<string>((int)SPC.WorldId);
            workshop = info.GetValue<int>((int)SPC.Workshop, (int)(byte)Workshop.Arlen);
            exp = info.GetValue<int>((int)SPC.Exp, 0);
        }
    }
}
