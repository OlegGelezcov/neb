using Nebula.Game.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Game.Events {
    public class QuestChestEventSubscriber : EventSubscriber {

        private QuestChest m_QuestChest;

        public override void Start() {
            base.Start();
            m_QuestChest = GetComponent<QuestChest>();
        }

        public override bool OnEvent(BaseEvent evt) {
            if(evt.eventType == Common.EventType.QuestStarted) {
                QuestStartedEvent questStartedEvent = evt as QuestStartedEvent;
                if(questStartedEvent != null && m_QuestChest != null) {
                    if(m_QuestChest.HasQuest(questStartedEvent.quest)) {
                        m_QuestChest.RegenerateContentForPlayer(questStartedEvent.source.Id);
                    }
                }
                return true;
            }
            return false;
        }
    }
}
