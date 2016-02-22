using Common;
using Nebula.Game.Components;
using Nebula.Game.Events;
using Nebula.Inventory.Objects;
using ServerClientCommon;
using Space.Game;
using System.Collections;

namespace Nebula.Game.Contracts {
    public class ItemDeliveryContract : BaseContract {
        private string m_ItemId;
        private string m_TargetWorld;

        public override void ParseInfo(Hashtable info) {
            base.ParseInfo(info);
            m_ItemId = info.GetValue<string>((int)SPC.ItemId, string.Empty);
            m_TargetWorld = info.GetValue<string>((int)SPC.TargetWorld, string.Empty);
        }

        public override string ToString() {
            string baseString =  base.ToString();
            string addedString = string.Format("item id: {0} target world: {1}", m_ItemId, m_TargetWorld);
            return baseString + System.Environment.NewLine + addedString;
        }

        public override Hashtable GetInfo() {
            Hashtable hash =  base.GetInfo();
            hash.Add((int)SPC.ItemId, itemId);
            hash.Add((int)SPC.TargetWorld, targetWorld);
            return hash;
        }

        public override void OnAccepted() {
            base.OnAccepted();
            var player = contractOwner.GetComponent<MmoActor>();
            if(player != null ) {
                if(player.Inventory.HasFreeSpace() ) {
                    ContractItemObject itemObject = new ContractItemObject(itemId, id);
                    if(player.Inventory.Add(itemObject, 1)) {
                        player.EventOnInventoryUpdated();
                        player.GetComponent<MmoMessageComponent>().ReceiveItemsAdded(InventoryType.ship);
                    }
                }
            }
        }

        public override void OnDeclined() {
            base.OnDeclined();
            var player = contractOwner.GetComponent<MmoActor>();
            if(player != null ) {
                if(player.Inventory.RemoveContractItems(id)) {
                    player.EventOnInventoryUpdated();
                }
            }
        }

        public ItemDeliveryContract(Hashtable hash, ContractManager manager)
            : base(hash, manager) {
            m_ItemId = hash.GetValue<string>((int)SPC.ItemId, string.Empty);
            m_TargetWorld = hash.GetValue<string>((int)SPC.TargetWorld, string.Empty);
        }

        public ItemDeliveryContract(string id, int stage,
            string sourceWorld, ContractManager manager, string itemId, string targetWorld)
            : base(id, stage, sourceWorld, ContractCategory.itemDelivery, manager) {
            m_ItemId = itemId;
            m_TargetWorld = targetWorld;
        }

        public override ContractCheckStatus CheckEvent(BaseEvent evt) {
            if(evt.eventType == EventType.EnterStation && state == ContractState.accepted) {
                EnterStationEvent enterStationEvent = evt as EnterStationEvent;
                if(enterStationEvent != null ) {
                    if (evt.source.mmoWorld().GetID() == targetWorld) {
                        var player = enterStationEvent.source.GetComponent<MmoActor>();
                        if (player.Inventory.HasItem(InventoryObjectType.contract_item, itemId)) {
                            if (Ready()) {
                                if (player.Inventory.RemoveContractItems(id)) {
                                    player.EventOnInventoryUpdated();
                                }
                                return ContractCheckStatus.ready;
                            }
                        }
                    }
                }
            }
            return ContractCheckStatus.none;
        }

        private string itemId {
            get {
                return m_ItemId;
            }
        }

        private string targetWorld {
            get {
                return m_TargetWorld;
            }
        }
    }
}
