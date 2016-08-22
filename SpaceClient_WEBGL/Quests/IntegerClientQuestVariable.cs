using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;
using ServerClientCommon;

namespace Nebula.Client.Quests {
    public class IntegerClientQuestVariable : ClientQuestVariable {

        private int m_Value;

        public IntegerClientQuestVariable(Hashtable hash) {
            ParseInfo(hash);
        }

        public override void ParseInfo(Hashtable info) {
            base.ParseInfo(info);
            m_Value = info.GetValueInt((int)SPC.Value);
        }

        public int value {
            get {
                return m_Value;
            }
        }
    }
}
