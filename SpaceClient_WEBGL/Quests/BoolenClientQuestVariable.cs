using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;
using ServerClientCommon;

namespace Nebula.Client.Quests {
    public class BoolenClientQuestVariable : ClientQuestVariable  {
        private bool m_Value;

        public BoolenClientQuestVariable(Hashtable hash) {
            ParseInfo(hash);
        }

        public override void ParseInfo(Hashtable info) {
            base.ParseInfo(info);
            m_Value = info.GetValueBool((int)SPC.Value);
        }

        public bool value {
            get {
                return m_Value;
            }
        }
    }
}
