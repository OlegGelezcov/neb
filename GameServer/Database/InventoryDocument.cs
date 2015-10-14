using Space.Game.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Space.Game;
using MongoDB.Bson;

namespace Space.Database {
    public class InventoryDocument {
        public ObjectId Id { get; set; }
        public string CharacterId { get; set; }
        public int MaxSlots { get; set; }

        public List<InventoryItemDocumentElement> Items { get; set; }


        public void Set(ServerInventory sourceObject) {
            this.MaxSlots = sourceObject.MaxSlots;
            if(this.Items == null ) {
                this.Items = new List<InventoryItemDocumentElement>();
            }
            this.Items.Clear();
            foreach(var typedItems in sourceObject.Items) {
                foreach(var item in typedItems.Value) {
                    this.Items.Add(new InventoryItemDocumentElement {
                        Count = item.Value.Count,
                        Object = item.Value.Object.GetInfo()
                    });
                }
            }
        }

        public ServerInventory SourceObject(IRes resource) {
            ServerInventory serverInventory = new ServerInventory(this.MaxSlots);
            if(this.Items != null) {
                serverInventory.SetItems(DatabaseUtils.TransformForInventory(this.Items));
            }
            return serverInventory;
        }
    }
}
