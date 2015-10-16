using ServerClientCommon;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Common;
using Space.Game.Inventory;
using Nebula.Inventory.Objects;
using System.Collections;

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
                    return new FortUpgradeObject("fort_level_upgrade_1_10", 1, 10, (Race)(byte)race, true);
                case "fort_level_upgrade_11_20":
                    return new FortUpgradeObject("fort_level_upgrade_11_20", 11, 20, (Race)(byte)race, true);
                case "fort_level_upgrade_21_30":
                    return new FortUpgradeObject("fort_level_upgrade_21_30", 21, 30, (Race)(byte)race, true);
                case "out_level_upgrade_1_10":
                    return new OutpostUpgradeObject("out_level_upgrade_1_10", 1, 10, (Race)(byte)race, true);
                case "out_level_upgrade_11_20":
                    return new OutpostUpgradeObject("out_level_upgrade_11_20", 11, 20, (Race)(byte)race, true);
                case "out_level_upgrade_21_30":
                    return new OutpostUpgradeObject("out_level_upgrade_21_30", 21, 30, (Race)(byte)race, true);
                case "turret":
                    return new TurretInventoryObject("turret", race, true);
                case "fortification":
                    return new FortificationInventoryObject("fortification", race, true);
                case "outpost":
                    return new OutpostInventoryObject("outpost", race, true);
                case "mining_station_light":
                    return new MiningStationInventoryObject("mining_station_light", race, 10, 30, true);
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
