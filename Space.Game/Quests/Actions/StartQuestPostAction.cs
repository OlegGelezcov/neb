using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Quests.Actions {
    public class StartQuestPostAction : PostAction {
        private string m_QuestId;

        public StartQuestPostAction(string questId )
            : base(PostActionName.START_QUEST) {
            m_QuestId = questId;
        }

        public string questId {
            get {
                return m_QuestId;
            }
        }
    }
}
