using Common;
using System.Collections.Generic;
using Nebula.Client.Inventory.Objects;
using ServerClientCommon;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;

namespace Nebula.Client.Inventory {
    public class InventoryObjectInfoFactory {

        public static object GetAttachment(Hashtable objectHash) {
            if (objectHash == null) { return null; }

            int placingType = objectHash.Value<int>((int)SPC.PlacingType);
            switch ((PlacingType)placingType) {
                case PlacingType.Inventory:
                    return Get(objectHash);
                case PlacingType.Station:
                    return new ClientShipModule(objectHash);
                default:
                    throw new NebulaException(string.Format("unsupported placing type = {0}", (PlacingType)placingType));
            }

        }

        public static IInventoryObjectInfo Get(Hashtable info) {
            if (info.ContainsKey((int)SPC.ItemType)) {
                InventoryObjectType type = (InventoryObjectType)(byte)info.GetValueInt((int)SPC.ItemType, (int)(byte)InventoryObjectType.DrillScheme);

                switch (type) {
                    case InventoryObjectType.Weapon:
                        return new WeaponInventoryObjectInfo(info);
                    case InventoryObjectType.Scheme:
                        return new SchemeInventoryObjectInfo(info);
                    case InventoryObjectType.Material:
                        return new MaterialInventoryObjectInfo(info);
                    case InventoryObjectType.DrillScheme:
                        return new DrillSchemeObjectInfo(info);
                    case InventoryObjectType.Module:
                        return new ClientShipModule(info);
                    case InventoryObjectType.fortification:
                        return new FortificationInventoryObjectInfo(info);
                    case InventoryObjectType.fort_upgrade:
                        return new FortUpgradeInventoryObjectInfo(info);
                    case InventoryObjectType.mining_station:
                        return new MiningStationInventoryObjectInfo(info);
                    case InventoryObjectType.outpost:
                        return new OutpostInventoryObjectInfo(info);
                    case InventoryObjectType.out_upgrade:
                        return new OutpostUpgradeInventoryObjectInfo(info);
                    case InventoryObjectType.personal_beacon:
                        return new PersonalBeaconInventoryObjectInfo(info);
                    case InventoryObjectType.repair_kit:
                        return new RepairKitInventoryObjectInfo(info);
                    case InventoryObjectType.repair_patch:
                        return new RepairPatchInventoryObjectInfo(info);
                    case InventoryObjectType.turret:
                        return new TurretInventoryObjectInfo(info);
                    case InventoryObjectType.nebula_element:
                        return new NebulaElementObjectInfo(info);
                    case InventoryObjectType.pass:
                        return new PassInventoryObjectInfo(info);
                    //case InventoryObjectType.credits:
                    //    return new CreditsObjectInfo(info);
                    default:
                        return null;
                }
            }
            return null;
        }


        //public static WeaponInventoryObjectInfo GetClientWeaponObject(Hashtable table) 
        //{
        //    return new WeaponInventoryObjectInfo(table);
        //}

        //public static SchemeInventoryObjectInfo GetClientSchemeObject(Hashtable info )
        //{
        //    SchemeInventoryObjectInfo scheme = new SchemeInventoryObjectInfo();
        //    scheme.ParseInfo(info);
        //    return scheme;
        //}

        //public static MaterialInventoryObjectInfo GetClientMaterialObject(Hashtable info )
        //{
        //    return new MaterialInventoryObjectInfo(info);
        //}

        //public static DrillSchemeObjectInfo GetClientDrillSchemeObject(Hashtable info)
        //{
        //    return new DrillSchemeObjectInfo(info);
        //}

        //public static ClientShipModule GetClientShipModuleObject(Hashtable info) {
        //    return new ClientShipModule(info);
        //}

        //public static CreditsObjectInfo GetCreditsObject(Hashtable info) {
        //    return new CreditsObjectInfo(info);
        //}


        public static List<ClientInventoryItem> GetContentList(object[] content) {
            List<ClientInventoryItem> res = new List<ClientInventoryItem>();
            foreach (object o in content) {
                Hashtable h = o as Hashtable;
                if (h != null) {
                    int count = h.GetValueInt((int)SPC.Count);
                    Hashtable info = h.GetValueHash((int)SPC.Info);
                    res.Add(new ClientInventoryItem(Get(info), count));
                }
            }
            return res;
        }

        public static List<ClientInventoryItem> ParseItemsArray(object[] inventoryItems) {
            List<ClientInventoryItem> result = new List<ClientInventoryItem>(inventoryItems.Length);
            foreach (object obj in inventoryItems) {
                if (obj is Hashtable) {
                    Hashtable itemInfo = obj as Hashtable;
                    int itemCount = itemInfo.GetValueInt((int)SPC.Count);
                    InventoryObjectType itemType = itemInfo.GetValueByte((int)SPC.ItemType, (byte)InventoryObjectType.Weapon).toEnum<InventoryObjectType>();

                    var itemObj = Get(itemInfo);
                    if (itemObj != null) {
                        result.Add(new ClientInventoryItem(itemObj, itemCount));
                    } else {
                        throw new NebulaException(string.Format("not founded factory for inventory object type = {0}", itemType));
                    }
                }
            }
            return result;
        }

        //public static ShipModelSlotType ParseShipModelSlot(Hashtable info, out ClientShipModule module) 
        //{
        //    module = null;
        //    ShipModelSlotType type = info.GetValue<byte>((int)SPC.Type, (byte)ShipModelSlotType.CB).toEnum<ShipModelSlotType>();
        //    Hashtable moduleInfo = info[(int)SPC.Info] as Hashtable;
        //    if (moduleInfo != null && moduleInfo.Count > 0) 
        //    {
        //        module = new ClientShipModule(moduleInfo);
        //    }
        //    return type;
        //}

        //public static ClientWorkhouseStationHold ParseWorkstationHold(Hashtable hold)
        //{
        //    return new ClientWorkhouseStationHold(hold);
        //}
    }
}
