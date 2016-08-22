using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Quests.Actions {
    public abstract class PostAction {
        private string m_Name;

        public PostAction(string name) {
            m_Name = name;
        }

        public string name {
            get {
                return m_Name;
            }
        }
    }

}
