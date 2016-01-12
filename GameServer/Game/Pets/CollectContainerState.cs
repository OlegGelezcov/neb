using GameMath;
using Nebula.Engine;
using Nebula.Game.Components;
using ServerClientCommon;
using Space.Game;
using Space.Game.Inventory;
using System.Collections.Concurrent;

namespace Nebula.Game.Pets {
    public class CollectContainerState : PetBaseState {

        private readonly float kSpeed = 25.0f;
        private readonly float kChestConvergence = 40;
        private bool m_Collected = false;

        private NebulaObject m_ChestObj;

        public CollectContainerState(PetObject petObj, NebulaObject chestObj) : base(petObj) {
            m_ChestObj = chestObj;
        }

        public override PetState name {
            get {
                return PetState.CollectContainer;
            }
        }

        public override void Update(float deltaTime) {
            if(!m_ChestObj) {
                pet.SetState(new PetIdleState(pet));
                return;
            }
            if(m_Collected) {
                pet.SetState(new PetIdleState(pet));
                return;
            }
            MoveToChest(deltaTime);
        }

        private void MoveToChest(float deltaTime) {
            Vector3 direction = (m_ChestObj.transform.position - pet.transform.position);
            if(direction.magnitude < kChestConvergence) {
                Collect();
                return;
            } else {
                Vector3 moving = kSpeed * direction.normalized * deltaTime;
                pet.Move(pet.transform.position, pet.transform.rotation, pet.transform.position + moving, pet.ComputeRotation(direction.normalized, 0.5f, deltaTime).eulerAngles, kSpeed);
            }
        }

        private void Collect() {
            if(!m_Collected) {
                m_Collected = true;

                var player = pet.owner.GetComponent<MmoActor>();
                var chest = m_ChestObj.GetComponent<ChestComponent>();
                if(player != null && chest != null) {
                    ConcurrentBag<ServerInventoryItem> addedObjects;
                    bool status = player.Inventory.AddAllFromChest(chest, player.nebulaObject.Id, out addedObjects);
                    pet.nebulaObject.MmoMessage().SendCollectChest(new System.Collections.Hashtable {
                        { (int)SPC.Target, m_ChestObj.Id },
                        { (int)SPC.TargetType, m_ChestObj.Type },
                        { (int)SPC.Status, status }
                    });
                }
            }
        }
    }
}
