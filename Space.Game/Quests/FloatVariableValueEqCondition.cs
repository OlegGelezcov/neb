using Common;
using GameMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Quests {
    public class FloatVariableValueEqCondition : VariableValueCondition {
        private string m_VariableName;
        private float m_Value;

        public override string variableName {
            get {
                return m_VariableName;
            }
        }

        public FloatVariableValueEqCondition(string name, float value) :
            base(QuestConditionName.FLOAT_VARIABLE_VALUE_EQ) {
            m_VariableName = name;
            m_Value = value;
        }

        public override bool CheckCondition(IQuestConditionTarget target, object data = null) {
            if(quest != null ) {
                float fVar = 0.0f;
                if(quest.TryGetFloat(m_VariableName, out fVar)) {
                    return Mathf.Approximately(fVar, m_Value);
                }
            }
            return false;
        }
    }
}
