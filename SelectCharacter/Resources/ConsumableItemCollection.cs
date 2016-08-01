using Common;
using Nebula.Inventory.Objects;
using ServerClientCommon;
using Space.Game.Inventory;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace SelectCharacter.Resources {
    public class ConsumableItemCollection : IInfoSource {
        public ConcurrentDictionary<string, ConsumableItem> items { get; private set; }

        public ConsumableItemCollection() {
            items = new ConcurrentDictionary<string, ConsumableItem>();
        }
        public void Load(string basePath ) {
            items.Clear();
            string fullPath = Path.Combine(basePath, "assets/consumable.xml");

            XDocument document = XDocument.Load(fullPath);
            var dumpList = document.Element("items").Elements("item").Select(itemElementt => {
                string id = itemElementt.GetString("id");
                InventoryObjectType type = (InventoryObjectType)Enum.Parse(typeof(InventoryObjectType), itemElementt.GetString("type"));
                int price = itemElementt.GetInt("price");
                int count = itemElementt.GetInt("count");
                MoneyType moneyType = (MoneyType)Enum.Parse(typeof(MoneyType), itemElementt.GetString("money_type"));
                ConsumableItem consumableItem = new ConsumableItem(id, type, price, count, moneyType);
                items.TryAdd(consumableItem.id, consumableItem);
                return consumableItem;
            }).ToList();
        }

        public bool TryGetItem(string id, out ConsumableItem item) {
            return items.TryGetValue(id, out item);
        }

        public IInventoryObject GetItemFromConsumable(ConsumableItem item, int race) {
            switch(item.id) {
                case "personal_beacon_1hr":
                    return new PersonalBeaconObject("personal_beacon_1hr", 3600, true);
                case "personal_beacon_2hr":
                    return new PersonalBeaconObject("personal_beacon_2hr", 7200, true);
                case "repair_kit_25pc":
                    return new RepairKitObject("repair_kit_25pc", 0.25f, true);
                case "repair_kit_50pc":
                    return new RepairKitObject("repair_kit_50pc", 0.5f, true);
                case "repair_patch_100pc":
                    return new RepairPatchObject("repair_patch_100pc", 2f, true);
                case "repair_patch_300pc":
                    return new RepairPatchObject("repair_patch_300pc", 6f, true);
                case "fort_level_upgrade_1_10":
                    return new FortUpgradeObject(1, 10, (Race)(byte)race, true);
                case "fort_level_upgrade_11_20":
                    return new FortUpgradeObject(11, 20, (Race)(byte)race, true);
                case "fort_level_upgrade_21_30":
                    return new FortUpgradeObject(21, 30, (Race)(byte)race, true);
                case "out_level_upgrade_1_10":
                    return new OutpostUpgradeObject(1, 10, (Race)(byte)race, true);
                case "out_level_upgrade_11_20":
                    return new OutpostUpgradeObject(11, 20, (Race)(byte)race, true);
                case "out_level_upgrade_21_30":
                    return new OutpostUpgradeObject(21, 30, (Race)(byte)race, true);
                case "turret":
                    return new TurretInventoryObject(race, true);
                case "fortification":
                    return new FortificationInventoryObject(race, true);
                case "outpost":
                    return new OutpostInventoryObject(race, true);
                case "mining_station_light":
                    return new MiningStationInventoryObject(race, 10, 30, true);

                case "planet_command_center":
                    return new PlanetCommandCenterInventoryObject();
                case "planet_mining_station":
                    return new PlanetMiningStationInventoryObject();
                case "planet_turret":
                    return new PlanetTurretInventoryObject();
                case "planet_resource_hangar":
                    return new PlanetResourceHangarInventoryObject();
                case "planet_resource_accelerator":
                    return new PlanetResourceAcceleratorInventoryObject();
                default:
                    return null;
            }
        }

        public Hashtable GetInfo() {
            Hashtable result = new Hashtable();
            foreach(var pItem in items ) {
                result.Add(pItem.Key, pItem.Value.GetInfo());
            }
            return result;
        }
    }
}
