/*
using Common;

namespace Nebula.Quests {
    public class QuestItemUsedNearActivatorWithBadgeCondition : QuestCondition {

        private string m_ItemId;
        private string m_Badge;

        public QuestItemUsedNearActivatorWithBadgeCondition(string itemID, string badge) 
            : base(QuestConditionName.QUEST_ITEM_USED_NEAR_ACTIVATOR_WITH_BADGE) {
            m_ItemId = itemID;
            m_Badge = badge;
        }

        public override bool CheckCondition(IQuestConditionTarget target, object data = null) {
            if(data != null ) {
                string sid = data as string;
                if (sid != null) {
                    if (sid == m_ItemId) {
                        if(target.HasNearActivatorsWithBadge(m_Badge)) {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
*/
