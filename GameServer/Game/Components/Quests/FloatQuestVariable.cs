/*
using Common;
using ServerClientCommon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Game.Components.Quests {
    public class FloatQuestVariable : QuestVariable {
        private float m_Value;

        public FloatQuestVariable(string name, string type, float val) : base(name, type) {
            m_Value = val;
        }

        public FloatQuestVariable(Hashtable hash) {
            ParseInfo(hash);
        }

        public override Hashtable GetInfo() {
            Hashtable hash = base.GetInfo();
            hash.Add((int)SPC.Value, value);
            return hash;
        }

        public override void ParseInfo(Hashtable hash) {
            base.ParseInfo(hash);
            m_Value = hash.GetValue<float>((int)SPC.Value, 0.0f);
        }

        public float value {
            get {
                return m_Value;
            }
        }

        public void SetValue(float val) {
            m_Value = val;
        }
    }
}*/
