using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Client.Dialogs {
    public class CountOfItemsGECondition : DialogCondition {
        private string m_Id;
        private InventoryObjectType m_Type;
        private int m_Value;
        private string m_UpdateText;

        public CountOfItemsGECondition(InventoryObjectType type, string id, int value, string updateText)
            : base(QuestConditionName.COUNT_OF_ITEMS_GE) {
            m_Type = type;
            m_Id = id;
            m_Value = value;
            m_UpdateText = updateText;
        }

        public string id {
            get {
                return m_Id;
            }
        }

        public InventoryObjectType type {
            get {
                return m_Type;
            }
        }

        public int value {
            get {
                return m_Value;
            }
        }

        public string updateText {
            get {
                return m_UpdateText;
            }
        }

        public override bool CheckCondition(IDialogConditionTarget target) {
            return false;
        }
    }
}
