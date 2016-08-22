using GameMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Quests {
    public class FloatVariableValueEqCondition : QuestCondition {
        private string m_VariableName;
        private float m_Value;

        public FloatVariableValueEqCondition(string name, float value) {
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
