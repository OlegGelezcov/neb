using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Quests {
    public abstract class QuestVariableData {
        private string m_Type;
        private string m_Name;

        public QuestVariableData(string type, string name) {
            m_Type = type;
            m_Name = name;
        }

        public string type {
            get {
                return m_Type;
            }
        }

        public string name {
            get {
                return m_Name;
            }
        }
    }
}
