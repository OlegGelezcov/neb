using Nebula.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Game.Events {
    public class QuestStartedEvent : BaseEvent {

        private string m_Quest;

        public string quest {
            get {
                return m_Quest;
            }
        }

        public QuestStartedEvent(NebulaObject source, string quest)
            : base(Common.EventType.QuestStarted, source) {
            m_Quest = quest;
        }
    }
}
