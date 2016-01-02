using Common;
using Nebula.Inventory.Objects;
using ServerClientCommon;
using Space.Game.Inventory;
using Space.Game.Inventory.Objects;
using Space.Game.Ship;
using System;
using System.Collections;

namespace Nebula.Inventory {

    /// <summary>
    /// Utils functions for working with inventory
    /// </summary>
    public static class InventoryUtils {


        public static IInventoryObject Create(object info) {
            if(info == null ) {
                return null;
            }
            Hashtable hash = info as Hashtable;
            if(hash == null ) {
                return null;
            }

            if(!hash.ContainsKey((int)SPC.Count)) {
                hash.Add((int)SPC.Count, 1);
            }

            int cnt = 0;
            return Create(hash, out cnt);
        }


        /// <summary>
        /// Create item from hashtable
        /// </summary>
        /// <param name="itemInfo"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static IInventoryObject Create(Hashtable itemInfo, out int count) {
            count = 0;
            if (itemInfo.ContainsKey((int)SPC.Count)) {
                count = itemInfo.GetValue<int>((int)SPC.Count, 0);
            }
            InventoryObjectType objType = (InventoryObjectType)(byte)itemInfo.GetValue<int>((int)SPC.ItemType, InventoryObjectType.Weapon.toByte());
            switch (objType) {
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
                case InventoryObjectType.exp_boost:
                    return new ExpBoostObject(itemInfo);
                case InventoryObjectType.loot_box:
                    return new LootBoxObject(itemInfo);
                //case InventoryObjectType.pass:
                   // return new PassInventoryObject(itemInfo);
                //case InventoryObjectType.credits:
                //    return new CreditsObject(itemInfo);
                default:
                    throw new Exception("Not supported object type: {0}".f(objType));
            }
        }
    }
}
