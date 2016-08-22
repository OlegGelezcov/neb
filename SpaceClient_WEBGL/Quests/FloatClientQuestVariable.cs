using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;
using ServerClientCommon;

namespace Nebula.Client.Quests {
    public class FloatClientQuestVariable : ClientQuestVariable {
        private float m_Value;

        public FloatClientQuestVariable(Hashtable hash) {
            ParseInfo(hash);
        }

        public override void ParseInfo(Hashtable info) {
            base.ParseInfo(info);
            m_Value = info.GetValueFloat((int)SPC.Value);
        }

        public float value {
            get {
                return m_Value;
            }
        }
    }
}
