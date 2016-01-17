using Common;
using ServerClientCommon;
using System.Collections;
using System;

namespace Nebula.Pets {
    public class PetActiveSkill : IInfoParser {
        public int id { get; set; }
        public bool activated { get; set; }

        public Hashtable GetInfo() {
            return new Hashtable {
                {(int)SPC.Id, id },
                {(int)SPC.Active, activated }
            };
        }


        public void ParseInfo(Hashtable info) {
            id = info.GetValue<int>((int)SPC.Id, 0);
            activated = info.GetValue<bool>((int)SPC.Active, true);
        }
    }
}
