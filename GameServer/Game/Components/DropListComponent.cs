using Common;
using Nebula.Engine;
using Nebula.Inventory.DropList;
using Nebula.Server.Components;
using Space.Game;
using System.Collections.Generic;

namespace Nebula.Game.Components {
    public abstract class DropListComponent : NebulaBehaviour {
        private ItemDropList m_DropList;

        public override int behaviourId {
            get {
                return (int)ComponentID.DropList;
            }
        }

        public virtual void Init(DropListComponentData data) {
            if(data.parentElement == null ) {
                m_DropList = new ItemDropList();
            } else {
                DropListFactory factory = new DropListFactory();
                m_DropList = new ItemDropList(factory.Create(data.parentElement));
            }
        }

        public void SetDropList(List<DropItem> items ) {
            m_DropList = new ItemDropList(items);
        }

        protected ItemDropList dropList {
            get {
                return m_DropList;
            }
        }

        public abstract ActorDropListPair GetDropList(DamageInfo actor);
    }

    public class ActorDropListPair {
        private DamageInfo m_Player;
        private ItemDropList m_DropList;

        public DamageInfo player {
            get {
                return m_Player;
            }
        }

        public ItemDropList dropList {
            get {
                return m_DropList;
            }
        }

        public ActorDropListPair(DamageInfo actor, ItemDropList dropList) {
            m_Player = actor;
            m_DropList = dropList;
        }
    }
}
