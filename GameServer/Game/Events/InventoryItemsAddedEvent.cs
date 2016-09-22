//using Nebula.Engine;
//using Space.Game.Inventory;
//using System.Collections.Generic;

//namespace Nebula.Game.Events {
//    public class InventoryItemsAddedEvent : BaseEvent {
//        private List<ServerInventoryItem> m_Items;

//        public InventoryItemsAddedEvent(NebulaObject source, List<ServerInventoryItem> items )
//            : base(Common.EventType.InventoryItemsAdded, source ) {
//            m_Items = items;
//        }

//        public List<ServerInventoryItem> items {
//            get {
//                return m_Items;
//            }
//        }

//        public bool HasContractItem(string id ) {
//            foreach(var it in items ) {
//                if(it.Object.Type == Common.InventoryObjectType.contract_item) {
//                    if(it.Object.Id == id ) {
//                        return true;
//                    }
//                }
//            }
//            return false;
//        }
//    }
//}
