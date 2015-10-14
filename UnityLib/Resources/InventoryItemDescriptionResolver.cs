//namespace Nebula.Resources {
//    using UnityEngine;
//    using System.Collections;
//    using Nebula.Client.Inventory;
//    using Nebula.Client;
//    using Nebula.Client.Inventory.Objects;

//    public class InventoryItemDescriptionResolver {

//        private readonly StringSubCache<string> subCache = new StringSubCache<string>();

//        public string Resolve(ClientInventoryItem item) {
//            switch (item.Object.Type) {
//                case Common.InventoryObjectType.Material:
//                    return ResolveMaterialDescription(item);
//                case Common.InventoryObjectType.Module:
//                    return ResolveModuleDescription(item);
//                case Common.InventoryObjectType.Weapon:
//                    return ResolveWeaponDescription(item);
//                case Common.InventoryObjectType.Scheme:
//                    return ResolveSchemeDescription(item);
//                case Common.InventoryObjectType.fortification:
//                case Common.InventoryObjectType.fort_upgrade:
//                case Common.InventoryObjectType.mining_station:
//                case Common.InventoryObjectType.outpost:
//                case Common.InventoryObjectType.out_upgrade:
//                case Common.InventoryObjectType.personal_beacon:
//                case Common.InventoryObjectType.repair_kit:
//                case Common.InventoryObjectType.repair_patch:
//                case Common.InventoryObjectType.turret:
//                    return StringCache.GetConsumableDescription(item.Object.Id);
//                default:
//                    return string.Empty;
//            }
//        }

//        private string ResolveMaterialDescription(ClientInventoryItem item) {
//            var ore = DataResources.Instance.OreData(item.Object.Id);
//            if (ore == null) {
//                return string.Empty;
//            }
//            return subCache.String(ore.Description, ore.Description);
//        }

//        private string ResolveModuleDescription(ClientInventoryItem item) {
//            var module = DataResources.Instance.ModuleData(((ClientShipModule)item.Object).templateId);
//            if (module == null) {
//                return string.Empty;
//            }
//            return subCache.String(module.DescriptionId, module.DescriptionId);
//        }

//        private string ResolveWeaponDescription(ClientInventoryItem item) {
//            var weapon = DataResources.Instance.Weapon(((WeaponInventoryObjectInfo)item.Object).Template);
//            if (weapon == null) {
//                return string.Empty;
//            }

//            return subCache.String(weapon.Description, weapon.Description);
//        }

//        private string ResolveSchemeDescription(ClientInventoryItem item) {
//            var scheme = item.Object as SchemeInventoryObjectInfo;
//            if (scheme == null) {
//                return string.Empty;
//            }
//            switch (scheme.Workshop) {
//                case Common.Workshop.Arlen:
//                    return subCache.String("S_AR_DESC", "S_AR_DESC");
//                case Common.Workshop.BigBang:
//                    return subCache.String("S_BB_DESC", "S_BB_DESC");
//                case Common.Workshop.DarthTribe:
//                    return subCache.String("S_DT_DESC", "S_DT_DESC");
//                case Common.Workshop.Dyneira:
//                    return subCache.String("S_DY_DESC", "S_DY_DESC");
//                case Common.Workshop.Equilibrium:
//                    return subCache.String("S_EQ_DESC", "S_EQ_DESC");
//                case Common.Workshop.KrolRo:
//                    return subCache.String("S_KR_DESC", "S_KR_DESC");
//                case Common.Workshop.Lerjees:
//                    return subCache.String("S_LE_DESC", "S_LE_DESC");
//                case Common.Workshop.Phelpars:
//                    return subCache.String("S_PH_DESC", "S_PH_DESC");
//                case Common.Workshop.Rakhgals:
//                    return subCache.String("S_RK_DESC", "S_RK_DESC");
//                case Common.Workshop.RedEye:
//                    return subCache.String("S_RE_DESC", "S_RE_DESC");
//                case Common.Workshop.Yshuar:
//                    return subCache.String("S_YS_DESC", "S_YS_DESC");
//                case Common.Workshop.Zoards:
//                    return subCache.String("S_ZO_DESC", "S_ZO_DESC");
//                default:
//                    return string.Empty;
//            }
//        }

//    }
//}
