using ExitGames.Logging;
using MongoDB.Bson;
using ServerClientCommon;
using Space.Game;
using Space.Game.Inventory;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Space.Database {
    public class WorkshopDocument {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        public ObjectId Id { get; set; }
        public string CharacterId { get; set; }
        //public Hashtable Hold { get; set; }
        public int StationInventoryMaxSlots { get; set; }
        public List<InventoryItemDocumentElement> StationInventoryItems { get; set; }
        //public int MaxHoldCount { get; set; }

        public void Set(WorkhouseStation station) {
            //this.Hold = ((ServerWorkhouseStationHold)station.Hold).GetInfo();
            //this.MaxHoldCount = ((ServerWorkhouseStationHold)station.Hold).MaxSlots;
            this.StationInventoryMaxSlots = station.StationInventory.MaxSlots;
            if (this.StationInventoryItems == null) {
                this.StationInventoryItems = new List<InventoryItemDocumentElement>();
            }
            this.StationInventoryItems.Clear();
            foreach (var typedItems in station.StationInventory.Items) {
                foreach (var item in typedItems.Value) {
                    this.StationInventoryItems.Add(new InventoryItemDocumentElement { Count = item.Value.Count, Object = item.Value.Object.GetInfo() });
                }
            }

        }

        public WorkhouseStation SourceObject(IRes resourcse) {
            WorkhouseStation station = new WorkhouseStation();
            //if(this.Hold != null ) {
            //    station.SetHold(this.Hold);
            //}

            if(this.StationInventoryItems == null ) {
                this.StationInventoryItems = new List<InventoryItemDocumentElement>();
            }
            ServerInventory serverInventory = new ServerInventory(this.StationInventoryMaxSlots);
            serverInventory.SetItems(DatabaseUtils.TransformForInventory(this.StationInventoryItems));
            station.SetInventory(serverInventory);
            return station;
        }
    }
}
