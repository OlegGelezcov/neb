using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Nebula.Inventory.DropList {
    public class ItemDropList {
        private readonly List<DropItem> m_Items;

        public ItemDropList() {
            m_Items = new List<DropItem>();
        }

        public ItemDropList(List<DropItem> items) {
            m_Items = items;
        }

        public void SetItems(XElement parent) {
            m_Items.Clear();
            DropListFactory factory = new DropListFactory();
            m_Items.AddRange(factory.Create(parent));
        }

        public List<DropItem> items {
            get {
                return m_Items;
            }
        }

        public ContractObjectDropItem GetContractObjectItem(string template) {
            foreach(var it in items) {
                if(it.type == Common.InventoryObjectType.contract_item ) {
                    if( (it as ContractObjectDropItem).templateId == template ) {
                        return (it as ContractObjectDropItem);
                    }
                }
            }
            return null;
        }
    }
}
