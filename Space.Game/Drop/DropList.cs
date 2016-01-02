using Common;
using Space.Game;
using Space.Game.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Nebula.Drop {
    public  class DropList {
        public string id { get;  }
        public List<DropListItem> items { get; }

        public DropList(string id) {
            this.id = id;
        }

        public DropList(XElement element) {
            id = element.GetString("id");
            items = element.Elements("item").Select(itemElement => {
                InventoryObjectType category = (InventoryObjectType)Enum.Parse(typeof(InventoryObjectType), itemElement.GetString("category"));
                switch (category) {
                    case InventoryObjectType.Module:
                        return (DropListItem)new ModuleDropListItem(itemElement);
                    case InventoryObjectType.Weapon:
                        return (DropListItem)new WeaponDropListItem(itemElement);
                    default:
                        return null;
                }
            }).ToList();
        }

        public List<ServerInventoryItem> Roll(IRes resource, int level, Workshop workshop) {
            List<ServerInventoryItem> resultItems = new List<ServerInventoryItem>();
            foreach(var dropItem in items) {
                var inventoryItem = dropItem.Roll(resource, level, workshop);
                if(inventoryItem != null ) {
                    resultItems.Add(inventoryItem);
                }
            }
            return resultItems;
        }


    }
}
