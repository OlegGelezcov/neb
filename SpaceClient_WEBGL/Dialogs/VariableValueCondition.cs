using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Client.Dialogs {

    public abstract class VariableValueCondition : DialogCondition {

        private string m_UpdateText;

        public VariableValueCondition(string name, string varName, string updText)
            : base(name) {
            m_UpdateText = updText;
            variableName = varName;
        }

        public string variableName { get; private set; }

        public string updateText {
            get {
                return m_UpdateText;
            }
        }
    }

    public class IntVariableValueGECondition : VariableValueCondition {

        private int m_Value;

        public IntVariableValueGECondition(string varName, int varValue, string updateText)
            : base(QuestConditionName.INT_VARIABLE_VARLUE_GE, varName, updateText ) {
            m_Value = varValue;
        }

        public int value {
            get {
                return m_Value;
            }
        }

        public override bool CheckCondition(IDialogConditionTarget target) {
            return false;
        }
    }

    public class IntVariableValueEQCondition : VariableValueCondition {
        private int m_Value;

        public IntVariableValueEQCondition(string varName, int varValue, string updateText)
            : base(QuestConditionName.INT_VARIABLE_VALUE_EQ, varName, updateText ) {
            m_Value = varValue;
        }

        public int value {
            get {
                return m_Value;
            }
        }

        public override bool CheckCondition(IDialogConditionTarget target) {
            return false;
        }
    }

    public class BoolVariableValueEQCondition : VariableValueCondition {

        private bool m_Value;

        public BoolVariableValueEQCondition(string varName, bool varValue, string updateText)
            : base(QuestConditionName.BOOL_VARIABLE_VALUE_EQ, varName, updateText ) {
            m_Value = varValue;
        }

        public bool value {
            get {
                return m_Value;
            }
        }

        public override bool CheckCondition(IDialogConditionTarget target) {
            return false;
        }
    }

    public class FloatVariableValueEQCondition : VariableValueCondition {
        private float m_Value;

        public FloatVariableValueEQCondition(string varName, float varValue, string updateText)
            : base(QuestConditionName.FLOAT_VARIABLE_VALUE_EQ, varName, updateText ) {
            m_Value = varValue;
        }

        public float value {
            get {
                return m_Value;
            }
        }

        public override bool CheckCondition(IDialogConditionTarget target) {
            return false;
        }
    }
}
