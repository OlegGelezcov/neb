using GameMath;
using Nebula.Engine;
using Nebula.Game.Components;
using ServerClientCommon;
using Space.Game;
using Space.Game.Inventory;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Nebula.Game.Pets {
    public class CollectContainerState : PetBaseState {

        private readonly float kSpeed = 25.0f;
        private readonly float kChestConvergence = 40;
        private bool m_Collected = false;

        private Queue<NebulaObject> m_ChestObjects = new Queue<NebulaObject>();
        //private NebulaObject m_ChestObj;

        public CollectContainerState(PetObject petObj, NebulaObject chestObj) : base(petObj) {
            //m_ChestObj = chestObj;
            if(m_ChestObjects == null ) {
                m_ChestObjects = new Queue<NebulaObject>();
            }
            m_ChestObjects.Enqueue(chestObj);
        }

        public override PetState name {
            get {
                return PetState.CollectContainer;
            }
        }

        public override void Update(float deltaTime) {
            if(m_ChestObjects.Count == 0 ) {
                pet.SetState(new PetIdleState(pet));
                return;
            }

            var chest = m_ChestObjects.Peek();


            if(!chest) {
                m_ChestObjects.Dequeue();
                //pet.SetState(new PetIdleState(pet));
                return;
            }
            if(m_Collected) {
                pet.SetState(new PetIdleState(pet));
                return;
            }
            MoveToChest(deltaTime,  chest);
        }

        private void MoveToChest(float deltaTime, NebulaObject chest) {
            Vector3 direction = (chest.transform.position - pet.transform.position);
            if(direction.magnitude < kChestConvergence) {
                Collect(chest);
                return;
            } else {
                Vector3 moving = kSpeed * direction.normalized * deltaTime;
                pet.Move(pet.transform.position, pet.transform.rotation, pet.transform.position + moving, pet.ComputeRotation(direction.normalized, 0.5f, deltaTime).eulerAngles, kSpeed);
            }
        }

        public void AddChest(NebulaObject newChest) {
            m_ChestObjects.Enqueue(newChest);
        }

        private void Collect(NebulaObject ichest) {
            if(!m_Collected) {
                m_ChestObjects.Dequeue();
                if(m_ChestObjects.Count == 0 ) {
                    m_Collected = true;
                }
                //m_Collected = true;

                var player = pet.owner.GetComponent<MmoActor>();
                var chest = ichest.GetComponent<ChestComponent>();
                if(player != null && chest != null) {
                    ConcurrentBag<ServerInventoryItem> addedObjects;
                    bool status = player.Inventory.AddAllFromChest(chest, player.nebulaObject.Id, out addedObjects);
                    pet.nebulaObject.MmoMessage().SendCollectChest(new System.Collections.Hashtable {
                        { (int)SPC.Target, ichest.Id },
                        { (int)SPC.TargetType, ichest.Type },
                        { (int)SPC.Status, status }
                    });
                    if(status) {
                        player.EventOnInventoryUpdated();
                    }
                }
            }
        }
    }
}
