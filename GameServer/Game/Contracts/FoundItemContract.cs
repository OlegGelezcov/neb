using Common;
using Nebula.Game.Events;
using ServerClientCommon;
using Space.Game;
using System.Collections;

namespace Nebula.Game.Contracts {
    public class FoundItemContract : BaseContract  {
        private string m_ItemId;
        private string m_GroupId;
        private string m_TargetWorld;

        public override void ParseInfo(Hashtable info) {
            base.ParseInfo(info);
            m_ItemId = info.GetValue<string>((int)SPC.ItemId, string.Empty);
            m_GroupId = info.GetValue<string>((int)SPC.Group, string.Empty);
            m_TargetWorld = info.GetValue<string>((int)SPC.TargetWorld, string.Empty);
        }
        public override string ToString() {
            string baseString = base.ToString();
            string addedString = string.Format("item id: {0}, group id: {1}, target world id: {2}",
                m_ItemId, m_GroupId, m_TargetWorld);
            return baseString + System.Environment.NewLine + addedString;
        }
        public override Hashtable GetInfo() {
            Hashtable hash = base.GetInfo();
            hash.Add((int)SPC.ItemId, m_ItemId);
            hash.Add((int)SPC.Group, m_GroupId);
            hash.Add((int)SPC.TargetWorld, m_TargetWorld);
            return hash;
        }
        public override void OnDeclined() {
            base.OnDeclined();
            var player = contractOwner.GetComponent<MmoActor>();
            if (player != null) {
                if (player.Inventory.RemoveContractItems(id)) {
                    player.EventOnInventoryUpdated();
                }
            }
        }

        public FoundItemContract(Hashtable hash, ContractManager manager)
            : base(hash, manager) {
            m_ItemId = hash.GetValue<string>((int)SPC.ItemId, string.Empty);
            m_GroupId = hash.GetValue<string>((int)SPC.Group, string.Empty);
            m_TargetWorld = hash.GetValue<string>((int)SPC.TargetWorld, string.Empty);
        }

        public FoundItemContract(string id, int stage, string sourceWorld, ContractManager manager, string itemId, string groupId, string targetWorld)
            : base(id, stage, sourceWorld, ContractCategory.foundItem, manager ) {
            m_ItemId = itemId;
            m_GroupId = groupId;
            m_TargetWorld = targetWorld;
        }

        public override ContractCheckStatus CheckEvent(BaseEvent evt) {

            if(evt.eventType == EventType.InventoryItemsAdded && state == ContractState.accepted ) {
                if(stage == 0 ) {
                    InventoryItemsAddedEvent iiaEvent = evt as InventoryItemsAddedEvent;
                    if(iiaEvent != null ) {
                        if(iiaEvent.HasContractItem(itemId)) {
                            SetStage(1);
                            return ContractCheckStatus.stage_changed;
                        }
                    }
                }
            }
            if(evt.eventType == EventType.EnterStation && state == ContractState.accepted ) {
                //if(stage == 1 ) {
                    EnterStationEvent enterStationEvent = evt as EnterStationEvent;
                    if(enterStationEvent != null ) {
                        if(evt.source.mmoWorld().GetID() == targetWorld ) {
                            var player = enterStationEvent.source.GetComponent<MmoActor>();
                            if(player.Inventory.HasContractItems(id)) {
                                if(Ready()) {
                                if (player.Inventory.RemoveContractItems(id)) {
                                    player.EventOnInventoryUpdated();
                                }
                                    return ContractCheckStatus.ready;
                                }
                            }
                        }
                    }
                //}
            }
            return ContractCheckStatus.none;
        }

        public string itemId {
            get {
                return m_ItemId;
            }
        }

        private string targetWorld {
            get {
                return m_TargetWorld;
            }
        }
        private string groupId {
            get {
                return m_GroupId;
            }
        }
    }
}
