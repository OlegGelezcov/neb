using Common;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;
using ServerClientCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Client.Pets {
    public class PetActiveSkill : IInfoParser{
        public int id { get; private set; }
        public bool activated { get; private set; }

        public PetActiveSkill(Hashtable hash) {
            ParseInfo(hash);
        }

        public void ParseInfo(Hashtable info) {
            id = info.GetValueInt((int)SPC.Id);
            activated = info.GetValueBool((int)SPC.Active);
        }
    }
}
