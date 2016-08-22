using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Quests {
    public class FloatQuestVariableData : QuestVariableData {
        private float m_Value;

        public FloatQuestVariableData(string type, string name, float value)
            : base(type, name) {
            m_Value = value;
        }

        public float value {
            get {
                return m_Value;
            }
        }
    }
}
