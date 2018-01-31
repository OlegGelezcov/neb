/*
using Common;
using ServerClientCommon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Game.Components.Quests {
    public class BooleanQuestVariable : QuestVariable {
        private bool m_Value;

        public BooleanQuestVariable(string name, string type, bool val)
            : base(name, type) {
            m_Value = val;
        }

        public BooleanQuestVariable(Hashtable hash) {
            ParseInfo(hash);
        }

        public override Hashtable GetInfo() {
            Hashtable hash = base.GetInfo();
            hash.Add((int)SPC.Value, value);
            return hash;
        }

        public override void ParseInfo(Hashtable hash) {
            base.ParseInfo(hash);
            m_Value = hash.GetValue<bool>((int)SPC.Value, false);
        }

        public bool value {
            get {
                return m_Value;
            }
        }

        public void SetValue(bool val) {
            m_Value = val;
        }
    }
}
*/