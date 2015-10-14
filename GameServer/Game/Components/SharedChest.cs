using Common;
using Nebula.Engine;
using Nebula.Server.Components;
using ServerClientCommon;
using Space.Game;
using Space.Game.Drop;
using Space.Game.Inventory;
using Space.Game.Inventory.Objects;
using Space.Server;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Nebula.Game.Components {
    public class SharedChest : NebulaBehaviour, IChest  {

        private ConcurrentDictionary<string, ServerInventoryItem> content { get; set; } = new ConcurrentDictionary<string, ServerInventoryItem>();

        private float duration { get; set; }
        private float updateDropListInterval { get; set; } = -1f;
        private DropList dropList { get; set; }

        private float mTimer;
        private float mUpdateDropListTimer;

        public void Init(SharedChestComponentData data) {
            duration = data.duration;
            updateDropListInterval = data.updateDropListInterval;
            dropList = data.dropList;
            UpdateDropList();
        }

        public override void Start() {
            mTimer = duration;
            mUpdateDropListTimer = updateDropListInterval;
        }

        private void UpdateDropList() {
            content.Clear();
            switch (dropList) {
                case DropList.RandomSchemeOfWorldLevel:
                    {
                        SchemeDropper dropper = new SchemeDropper(
                            CommonUtils.GetRandomEnumValue<Workshop>(new List<Workshop>()), 
                            (nebulaObject.world as MmoWorld).Zone.Level, 
                            nebulaObject.resource);

                        SchemeObject scheme = dropper.Drop() as SchemeObject;
                        AddContent(new ServerInventoryItem(scheme, 1));
                    }
                    break;
            }
        }
        public override void Update(float deltaTime) {

            if(updateDropListInterval > 0 ) {
                if(mUpdateDropListTimer > 0f) {
                    mUpdateDropListTimer -= deltaTime;
                    if(mUpdateDropListTimer <= 0f) {
                        UpdateDropList();
                        mUpdateDropListTimer = updateDropListInterval;
                    }
                }
            }
            if (duration > 0f) {
                if (mTimer > 0f) {
                    mTimer -= deltaTime;
                    if (mTimer <= 0f) {
                        DestroySelf();
                    }
                }
            }
        }

        public override int behaviourId {
            get {
                return (int)ComponentID.SharedChest;
            }
        }

        public void AddContent(ServerInventoryItem item) {
            if(!content.ContainsKey(item.Object.Id)) {
                content.TryAdd(item.Object.Id, item);
            }
        }

        public void SetDuration(float d) {
            duration = d;
            mTimer = duration;
        }

        public bool TryGetObject(string playerID, string inventoryObjectID, out ServerInventoryItem item) {
            return content.TryGetValue(inventoryObjectID, out item);
        }

        public bool TryGetActorObjects(string playerID, out ConcurrentDictionary<string, ServerInventoryItem> playerObjects) {
            playerObjects = content;
            return true;
        }

        public bool TryRemoveActorObjectids(string playerID, List<string> inventoryObjectIDs) {
            bool result = true;

            foreach(string inventoryObjectID in inventoryObjectIDs) {
                ServerInventoryItem old;
                result = result && content.TryRemove(inventoryObjectID, out old);
            }

            if(content.Count == 0 ) {
                DestroySelf();
            }
            return result;
        }

        public bool TryRemoveObject(string playerID, string inventoryObjectID) {
            ServerInventoryItem old;
            bool result = content.TryRemove(inventoryObjectID, out old);

            if(content.Count == 0 ) {
                DestroySelf();
            }
            return result;
        }

        private void DestroySelf() {
            if(nebulaObject) {
                (nebulaObject as Item).Destroy();
            }
        }

        public Hashtable GetInfoForActor(string actorId) {
            Hashtable info = new Hashtable();
            foreach(var pair in content) {
                info.Add(pair.Key, pair.Value.GetInfo());
            }
            info.Add((int)SPC.Target, nebulaObject.Id);
            info.Add((int)SPC.TargetType, nebulaObject.Type);
            return info;
        }


        public object[] ContentRaw(string playerID) {
            ConcurrentDictionary<string, ServerInventoryItem> filteredObjects = null;
            if (TryGetActorObjects(playerID, out filteredObjects)) {

                object[] raw = new object[filteredObjects.Count];
                int index = 0;
                foreach (var p in filteredObjects) {
                    raw[index++] = new Hashtable { { (int)SPC.Count, 1 }, { (int)SPC.Info, p.Value.GetInfo() } };
                }
                return raw;
            }
            return new object[] { };
        }
    }
}
