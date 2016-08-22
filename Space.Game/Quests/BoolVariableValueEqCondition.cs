using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Quests {
    public class BoolVariableValueEqCondition : QuestCondition  {
        private string m_VariableName;
        private bool m_Value;

        public BoolVariableValueEqCondition(string name, bool value ) {
            m_VariableName = name;
            m_Value = value;
        }

        public override bool CheckCondition(IQuestConditionTarget target, object data = null) {
            LibLogger.Log("check bool condition");
            if(quest != null ) {
                bool bVal = false;
                if(quest.TryGetBool(m_VariableName, out bVal)) {
                    LibLogger.Log(string.Format("condition: {0}", (bVal == m_Value)));
                    return (bVal == m_Value);
                }
            }
            return false;
        }
    }
}
