using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Quests.Actions {
    public class SetBoolVariablePostAction : PostAction {
        private string m_VariableName;
        private bool m_VariableValue;

        public SetBoolVariablePostAction(string varName, bool varValue) 
            : base(PostActionName.SET_BOOL_VARIABLE) {
            m_VariableName = varName;
            m_VariableValue = varValue;
        }

        public string variableName {
            get {
                return m_VariableName;
            }
        }

        public bool variableValue {
            get {
                return m_VariableValue;
            }
        }
    }
}
