using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nebula.Game.Events;
using GameMath;
using Nebula.Quests.Actions;

namespace Nebula.Quests.Triggers {
    public class QuestItemUsedNearPointTrigger : QuestTrigger {

        public string itemId { get; private set; }
        public string worldId { get; private set; }
        public Vector3 point { get; private set; }
        public float radius { get; private set; }

        public QuestItemUsedNearPointTrigger(string itid, string wid, Vector3 pt, float radius, List<PostAction> actions)
            : base(Common.QuestTriggerType.quest_item_used_near_point, Common.EventType.QuestItemUsed, actions) {
            this.itemId = itid;
            this.worldId = wid;
            this.point = pt;
            this.radius = radius;
        }

        protected override bool Check(IQuestConditionTarget target, BaseEvent evt) {
            if(evt != null &&  evt.eventType == Common.EventType.QuestItemUsed) {
                QuestItemUsedEvent qe = evt as QuestItemUsedEvent;
                if(qe.itemId == itemId ) {
                    if(target.isWorld(worldId)) {
                        if(target.NearPoint(point, radius)) {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
