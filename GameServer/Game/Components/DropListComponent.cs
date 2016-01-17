using Common;
using Nebula.Engine;
using Nebula.Inventory.DropList;
using Nebula.Server.Components;

namespace Nebula.Game.Components {
    public class DropListComponent : NebulaBehaviour {
        private ItemDropList m_DropList;

        public override int behaviourId {
            get {
                return (int)ComponentID.DropList;
            }
        }

        public void Init(DropListComponentData data) {
            if(data.parentElement == null ) {
                m_DropList = new ItemDropList();
            } else {
                DropListFactory factory = new DropListFactory();
                m_DropList = new ItemDropList(factory.Create(data.parentElement));
            }
        }

        public ItemDropList dropList {
            get {
                return m_DropList;
            }
        }
    }
}
