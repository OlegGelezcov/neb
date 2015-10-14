using System;
using System.Collections.Generic;
using Common;
using System.Linq;
using System.Collections;
using Space.Game.Drop;
using Space.Game.Inventory.Objects;
using Space.Game.Ship;
using ServerClientCommon;
using Nebula.Game.Components;
using System.Collections.Concurrent;
using Nebula.Inventory.Objects;

namespace Space.Game.Inventory
{
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

        public static IInventoryObject Create(Hashtable itemInfo, out int count)
        {
            count = 0;
            if (itemInfo.ContainsKey((int)SPC.Count)) {
                count = itemInfo.GetValue<int>((int)SPC.Count, 0);
            }
            InventoryObjectType objType = (InventoryObjectType)(byte)itemInfo.GetValue<int>((int)SPC.ItemType, InventoryObjectType.Weapon.toByte());
            switch (objType)
            {
                case InventoryObjectType.Material:
                    return new MaterialObject(itemInfo);
                case InventoryObjectType.Scheme:
                    return new SchemeObject(itemInfo);
                case InventoryObjectType.Weapon:
                    return new WeaponObject(itemInfo);
                case InventoryObjectType.Module:
                    return new ShipModule(itemInfo);
                case InventoryObjectType.fortification:
                    return new FortificationInventoryObject(itemInfo);
                case InventoryObjectType.fort_upgrade:
                    return new FortUpgradeObject(itemInfo);
                case InventoryObjectType.mining_station:
                    return new MiningStationInventoryObject(itemInfo);
                case InventoryObjectType.outpost:
                    return new OutpostInventoryObject(itemInfo);
                case InventoryObjectType.out_upgrade:
                    return new OutpostUpgradeObject(itemInfo);
                case InventoryObjectType.personal_beacon:
                    return new PersonalBeaconObject(itemInfo);
                case InventoryObjectType.repair_kit:
                    return new RepairKitObject(itemInfo);
                case InventoryObjectType.repair_patch:
                    return new RepairPatchObject(itemInfo);
                case InventoryObjectType.turret:
                    return new TurretInventoryObject(itemInfo);
                case InventoryObjectType.nebula_element:
                    return new NebulaElementObject(itemInfo);
                //case InventoryObjectType.credits:
                //    return new CreditsObject(itemInfo);
                default:
                    throw new Exception("Not supported object type: {0}".f(objType));
            }
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
            this._items = new Dictionary<InventoryObjectType, Dictionary<string, ServerInventoryItem>>();

            if(items != null )
            {
                foreach(object objItem in items)
                {
                    if(objItem is Hashtable)
                    {
                        Hashtable itemInfo = objItem as Hashtable;
                        int count = 0;
                        var obj = Create(itemInfo, out count);
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

        public bool EnoughSpace(Dictionary<string, InventoryObjectType> ids) {
            int slotsRequired = this.SlotsForItems(ids);
            return slotsRequired <= this.FreeSlots;
        }

        public void SetItems(List<Hashtable> items)
        {
            this._items = new Dictionary<InventoryObjectType, Dictionary<string, ServerInventoryItem>>();
            foreach (var item in items)
            {
                int count =  item.GetValue<int>((int)SPC.Count, 0);
                Hashtable info = item.GetValue<Hashtable>((int)SPC.Info, new Hashtable());
                int dumpCount = 0;
                var obj = Create(info, out dumpCount);
                if (count > 0)
                {
                    this.Add(obj, count);
                }
            }
        }

        public bool HasFreeSpace() {
            return this.FreeSlots > 0;
        }
    }
}
