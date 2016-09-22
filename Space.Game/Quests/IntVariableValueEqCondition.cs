using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Quests {
    public class IntVariableValueEqCondition : VariableValueCondition {

        private string m_VariableName;
        private int m_Value;

        public override string variableName {
            get {
                return m_VariableName;
            }
        }

        public IntVariableValueEqCondition(string name, int value) : 
            base(QuestConditionName.INT_VARIABLE_VALUE_EQ) {
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

    public class IntVariableValueGECondition : VariableValueCondition {
        private string m_VariableName;
        private int m_Value;

        public override string variableName {
            get {
                return m_VariableName;
            }
        }

        public IntVariableValueGECondition(string name, int value ) : base(QuestConditionName.INT_VARIABLE_VARLUE_GE) {
            m_VariableName = name;
            m_Value = value;
        }

        public override bool CheckCondition(IQuestConditionTarget target, object data = null) {
            if (quest != null) {
                int iVar = 0;
                if (quest.TryGetInteger(m_VariableName, out iVar)) {
                    return (iVar >= m_Value);
                }
            }
            return false;
        }
    }
}
