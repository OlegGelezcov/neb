using Common;
using Nebula.Game.Components;
using Nebula.Inventory;
using Nebula.Inventory.Objects;
using ServerClientCommon;
using Space.Game.Drop;
using Space.Game.Inventory.Objects;
using Space.Game.Ship;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Space.Game.Inventory {
    public class ServerInventory : Common.Inventory<ServerInventoryItem, IInventoryObject>, IInfo
    {

        public ServerInventory(int maxSlots) 
            : base(maxSlots ) 
        { 
            
        }

        public ServerInventory(Hashtable inventoryInfo)
        {
            this.ParseInfo(inventoryInfo);
        }

        public override bool Add(IInventoryObject obj, int count) {
            return base.Add(obj, count);
        }

        public bool AddFromContainer(IChest container,  string actorId, string objId, out ServerInventoryItem obj) {
            obj = null;
            if(container.TryGetObject(actorId, objId, out obj)) {
                container.TryRemoveObject(actorId, objId);
                return Add(obj.Object, obj.Count);
            }
            return false;
        }

        public bool AddAllFromChest(IChest chest,  string actorId, out ConcurrentBag<ServerInventoryItem> addedObjects) 
        {
            addedObjects = new ConcurrentBag<ServerInventoryItem>();

            ConcurrentBag<string> idsForRemove = new ConcurrentBag<string>();

            ConcurrentDictionary<string, ServerInventoryItem> actorObjects = null;
            if(chest.TryGetActorObjects(actorId, out actorObjects)) {
                foreach(var pair in actorObjects ) {
                    if(Add(pair.Value.Object, pair.Value.Count)) {
                        idsForRemove.Add(pair.Value.Object.Id);
                        addedObjects.Add(pair.Value);
                    }
                }
                return chest.TryRemoveActorObjectids(actorId, idsForRemove.ToList());
            }
            return false;
        }

        public Hashtable TransformScheme(string schemeID, DropManager dropManager) {
            ServerInventoryItem schemeItem;
            if (!TryGetItem(InventoryObjectType.Scheme, schemeID, out schemeItem)) {
                return new Hashtable { { ACTION_RESULT.RESULT, ACTION_RESULT.FAIL }, { ACTION_RESULT.MESSAGE, "Scheme not found in inventory" } };
            }
            ShipModule result = (schemeItem.Object as SchemeObject).Transform(dropManager) as ShipModule;
            Remove(InventoryObjectType.Scheme, schemeID, 1);
            Add(result, 1);
            return new Hashtable { { ACTION_RESULT.RESULT,  ACTION_RESULT.SUCCESS },
                { (int)SPC.Workshop, result.Workshop.ToString() },
                { ACTION_RESULT.RETURN, schemeID } };
        }

        public bool RemoveContractItems(string contractId ) {
            ConcurrentBag<ServerInventoryItem> contractItems = new ConcurrentBag<ServerInventoryItem>();
            if(Items.ContainsKey(InventoryObjectType.contract_item)) {
                foreach(var pkv in Items[InventoryObjectType.contract_item]) {
                    if(pkv.Value.Object is ContractItemObject) {
                        if( (pkv.Value.Object as ContractItemObject).contractId == contractId ) {
                            contractItems.Add(pkv.Value);
                        }
                    }
                }
            }

            foreach(var it in contractItems) {
                Remove(it.Object.Type, it.Object.Id, it.Count);
            }

            return (contractItems.Count > 0);
        }

        public bool HasContractItems(string contractId ) {
            if(Items.ContainsKey(InventoryObjectType.contract_item)) {
                foreach(var pit in Items[InventoryObjectType.contract_item]) {
                    ContractItemObject obj = pit.Value.Object as ContractItemObject;
                    if(obj != null ) {
                        if(obj.contractId == contractId ) {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        //public bool SplitItems(string itemID, int count ) {
        //    ServerInventoryItem item;
        //    if( TryGetItem(itemID, out item) ) {
        //        if(count < item.Count ) {
        //            Remove(item.Object.Type, itemID, count);

        //        }
        //    }
        //}

        public bool CheckCraftMaterials(Dictionary<string, int> craftMaterials)
        {
            bool checkResult = true;
            foreach(var pair in craftMaterials )
            {
                ServerInventoryItem item = null;
                if(this.TryGetItem(InventoryObjectType.Material, pair.Key, out item ) )
                {
                    if(item.Count < pair.Value )
                    {
                        checkResult = false;
                        break;
                    }
                }
                else
                {
                    checkResult = false;
                    break;
                }
            }
            return checkResult;
        }



        public Hashtable GetInfo() {
            Hashtable result = new Hashtable();
            result.Add((int)SPC.SlotsUsed, this.SlotsUsed);
            result.Add((int)SPC.MaxSlots, this.MaxSlots);

            object[] flatItems = new object[this.SlotsUsed];
            int index = 0;

            foreach (var typedItems in _items) {
                foreach (var itemPair in typedItems.Value) {
                    if (index < flatItems.Length) {
                        flatItems[index++] = itemPair.Value.GetInfo();
                    }
                }
            }
            result.Add((int)SPC.Items, flatItems);
            return result;
        }

        public void ParseInfo(Hashtable info)
        {
            this._maxSlots = info.GetValue<int>((int)SPC.MaxSlots, 0);
            object[] items = info.GetValue<object[]>((int)SPC.Items, new object[] { });
            this._items = new ConcurrentDictionary<InventoryObjectType, ConcurrentDictionary<string, ServerInventoryItem>>();

            if(items != null )
            {
                foreach(object objItem in items)
                {
                    if(objItem is Hashtable)
                    {
                        Hashtable itemInfo = objItem as Hashtable;
                        int count = 0;
                        var obj = InventoryUtils.Create(itemInfo, out count);
                        if (count > 0 && obj != null)
                        {
                            this.Add(obj, count);
                        }
                    }
                }
            }
        }

        public Dictionary<string, InventoryObjectType> GetItemIds()
        {
            Dictionary<string, InventoryObjectType> result = new Dictionary<string, InventoryObjectType>();
            foreach( var p in this._items )
            {
                foreach(var p2 in p.Value )
                {
                    result.Add(p2.Key, p.Key);
                }
            }
            return result;
        }

        public List<IDCountPair> GetItemIds(InventoryObjectType type) {
            List<IDCountPair> ids = new List<IDCountPair>();
            foreach(var p in _items ) {
                foreach(var p2 in p.Value) {
                    if(p2.Value.Object.Type == type ) {
                        ids.Add(new IDCountPair {  ID = p2.Key, count = p2.Value.Count  });
                    }
                }
            }
            return ids;
        }

        public int FreeSlots
        {
            get
            {
                return Math.Max(0, this.MaxSlots - this.SlotsUsed);
            }
        }

        public int SlotsForItems(Dictionary<string, InventoryObjectType> ids)
        {
            int counter = 0;
            foreach(var idType in ids )
            {
                if(false == this.HasItem(idType.Value, idType.Key))
                {
                    counter++;
                }
            }
            return counter;
        }

        public bool HasSlotsForItems(List<string> items) {
            int counter = 0;
            foreach(var itemId in items ) {
                if(!HasItem(itemId)) {
                    counter++;
                }
            }
            return (counter <= FreeSlots);
        }


        public int NumSlotsForItems(List<string> items ) {
            int counter = 0;
            foreach (var itemId in items) {
                if (!HasItem(itemId)) {
                    counter++;
                }
            }

            int diff = counter - FreeSlots;
            if(diff < 0 ) {
                diff = 0;
            }
            return diff;
        }

        public bool EnoughSpace(Dictionary<string, InventoryObjectType> ids) {
            int slotsRequired = this.SlotsForItems(ids);
            return slotsRequired <= this.FreeSlots;
        }

        public void SetItems(List<Hashtable> items)
        {
            this._items = new ConcurrentDictionary<InventoryObjectType, ConcurrentDictionary<string, ServerInventoryItem>>();
            foreach (var item in items)
            {
                int count =  item.GetValue<int>((int)SPC.Count, 0);
                Hashtable info = item.GetValue<Hashtable>((int)SPC.Info, new Hashtable());
                int dumpCount = 0;
                var obj = InventoryUtils.Create(info, out dumpCount);
                if (count > 0)
                {
                    this.Add(obj, count);
                }
            }
        }

        public bool HasCraftResourceItems(List<IDCountPair> items) {
            bool ok = true;
            foreach(var pair in items) {
                if(ItemCount(InventoryObjectType.craft_resource, pair.ID) < pair.count) {
                    ok = false;
                    break;
                }
            }
            return ok;
        }

        public bool RemoveCraftResourceItems(List<IDCountPair> items) {
            bool ok = true;
            foreach(var pair in items) {
                Remove(InventoryObjectType.craft_resource, pair.ID, pair.count);
            }
            return ok;
        }

        public bool HasFreeSpace() {
            return this.FreeSlots > 0;
        }
    }
}
