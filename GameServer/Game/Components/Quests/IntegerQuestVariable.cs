using Common;
using ServerClientCommon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Game.Components.Quests {
    public class IntegerQuestVariable : QuestVariable {

        private int m_Value;

        public IntegerQuestVariable(string name, string type, int val ) 
            : base(name, type) {
            m_Value = val;
        }

        public IntegerQuestVariable(Hashtable hash)  {
            ParseInfo(hash);
        }
        
        public override Hashtable GetInfo() {
            Hashtable hash = base.GetInfo();
            hash.Add((int)SPC.Value, value);
            return hash;
        }

        public override void ParseInfo(Hashtable hash) {
            base.ParseInfo(hash);
            m_Value = hash.GetValue<int>((int)SPC.Value, 0);
        }

        public int value {
            get {
                return m_Value;
            }
        }

        public void SetValue(int val) {
            m_Value = val;
        }
    }
}
