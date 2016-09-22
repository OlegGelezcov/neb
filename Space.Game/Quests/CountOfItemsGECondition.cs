using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Quests {
    public class CountOfItemsGECondition : QuestCondition {

        private InventoryObjectType m_ItemType;
        private string m_ItemId;
        private int m_Value;

        public CountOfItemsGECondition(InventoryObjectType itemType, string itemId, int value) : 
            base(QuestConditionName.COUNT_OF_ITEMS_GE) {
            m_ItemType = itemType;
            m_ItemId = itemId;
            m_Value = value;
        }

        public InventoryObjectType itemType {
            get {
                return m_ItemType;
            }
        }

        public string itemId {
            get {
                return m_ItemId;
            }
        }

        public int value {
            get {
                return m_Value;
            }
        }

        public override bool CheckCondition(IQuestConditionTarget target, object data = null) {
            return target.CountOfItems(itemType, itemId) >= value;
        }
    }
}
