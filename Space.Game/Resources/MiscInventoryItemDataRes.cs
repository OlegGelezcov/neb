using Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;


namespace Space.Game.Resources {
    public class MiscInventoryItemDataRes {

        private Dictionary<string, MiscInventoryItemData> items;

        public void Load(string basePath) {
            string fullPath = Path.Combine(basePath, "Data/misc_inventory_items.xml");
            XDocument document = XDocument.Load(fullPath);
            this.items = document.Element("items").Elements("item").Select(e => {
                string id = e.GetString("id");
                InventoryObjectType type = (InventoryObjectType)Enum.Parse(typeof(InventoryObjectType), e.GetString("type"));
                string name = e.GetString("name");
                Hashtable data = new Hashtable();

                var dump = e.Element("inputs").Elements("input").Select(ie => {
                    string key = ie.GetString("key");
                    string valType = ie.GetString("type");
                    string valStr = ie.GetString("value");
                    object val = CommonUtils.ParseValue(valStr, valType);
                    data.Add(key, val);
                    return key;
                }).ToList();

                return new MiscInventoryItemData(id, type, name, data);

            }).ToDictionary(obj => obj.Id(), obj => obj);
        }

        public bool TryGetItemData(string id, out MiscInventoryItemData data) {
            return this.items.TryGetValue(id, out data);
        }

        public bool TryGetItemData(InventoryObjectType type, out MiscInventoryItemData data) {
            data = null;
            foreach (var kv in this.items) {
                if (kv.Value.Type() == type) {
                    data = kv.Value;
                    return true;
                }
            }
            return false;
        }

        //public MiscInventoryItemData CreditsObject() {
        //    MiscInventoryItemData credits = null;
        //    if (TryGetItemData(InventoryObjectType.credits, out credits)) {
        //        return credits;
        //    }
        //    return null;
        //}
    }
}
