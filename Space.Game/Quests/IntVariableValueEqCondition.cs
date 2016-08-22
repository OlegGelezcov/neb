using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Quests {
    public class IntVariableValueEqCondition : QuestCondition {

        private string m_VariableName;
        private int m_Value;

        public IntVariableValueEqCondition(string name, int value) {
            m_VariableName = name;
            m_Value = value;
        }

        public override bool CheckCondition(IQuestConditionTarget target, object data = null) {
            if(quest != null ) {
                int iVar = 0;
                if(quest.TryGetInteger(m_VariableName, out iVar)) {
                    return (iVar == m_Value);
                }
            }
            return false;
        }
    }
}
