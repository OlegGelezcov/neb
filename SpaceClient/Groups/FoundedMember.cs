using Common;
using ServerClientCommon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Client.Groups {
    public class FoundedMember : IInfoParser {
        public string characterID { get; private set; }
        public Workshop workshop { get; private set; }
        public int exp { get; private set; }

        public FoundedMember(string characterID, Hashtable info) {
            this.characterID = characterID;
            ParseInfo(info);
        }

        public void ParseInfo(Hashtable info) {
            workshop = (Workshop)(byte)info.GetValue<int>((int)SPC.Workshop, (int)(byte)Workshop.Arlen);
            exp = info.GetValue<int>((int)SPC.Exp, 0);
        }
    }
}
