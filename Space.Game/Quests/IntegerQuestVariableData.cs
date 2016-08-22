using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Quests {
    public class IntegerQuestVariableData : QuestVariableData {

        private int m_Value;

        public IntegerQuestVariableData(string type, string name, int value)
            : base(type, name) {
            m_Value = value;
        }

        public int value {
            get {
                return m_Value;
            }
        }
    }
}
