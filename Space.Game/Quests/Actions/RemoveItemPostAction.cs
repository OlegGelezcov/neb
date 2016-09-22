using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Quests.Actions {
    public class RemoveItemPostAction : PostAction  {

        private InventoryObjectType m_Type;
        private string m_Id;
        private int m_Count;

        public RemoveItemPostAction(InventoryObjectType type, string id, int count)
            : base(PostActionName.REMOVE_ITEM) {
            m_Type = type;
            m_Id = id;
            m_Count = count;
        }

        public InventoryObjectType type {
            get {
                return m_Type;
            }
        }

        public string id {
            get {
                return m_Id;
            }
        }

        public int count {
            get {
                return m_Count;
            }
        }
    }
}
