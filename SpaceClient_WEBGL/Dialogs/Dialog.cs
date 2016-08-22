using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Client.Dialogs {
    public class Dialog {
        public string id { get; private set; }
        public List<DialogTopic> topics { get; private set; }
        public List<DialogCondition> startConditions { get; private set; } = new List<DialogCondition>();
        public int index { get; private set; }

        private int m_TopicIndex = -1;

        public Dialog() {
            id = string.Empty;
            topics = new List<DialogTopic>();
        }

        public Dialog(string id, List<DialogTopic> topics, List<DialogCondition> startConditions, int index) {
            this.id = id;
            this.topics = topics;
            this.startConditions = startConditions;
            this.index = index;
        }

        public void ResetTopicIndex() {
            m_TopicIndex = -1;
        }

        public DialogTopic currentTopic {
            get {
                if(m_TopicIndex >= 0 && m_TopicIndex < topics.Count ) {
                    return topics[m_TopicIndex];
                }
                return null;
            }
        }

        public DialogTopic nextTopic {
            get {
                m_TopicIndex++;
                return currentTopic;
            }
        }

        public bool CheckConditions(IDialogConditionTarget target) {
            bool valid = true;
            foreach(var condition in startConditions ) {
                if(!condition.CheckCondition(target)) {
                    valid = false;
                    break;
                }
            }
            return valid;
        }

    }
}
