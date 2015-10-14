using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Common;
using System.Collections;

namespace Nebula.Client.Res {
    public class ResMiscInventoryItems {
        private Dictionary<string, ResMiscInventoryItemData> items;

        public void Load(string xml) {
            XDocument document = XDocument.Parse(xml);

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

                return new ResMiscInventoryItemData(id, type, name, data);

            }).ToDictionary(obj => obj.Id(), obj => obj);
        }

        public bool TryGetItemData(string id, out ResMiscInventoryItemData data) {
            return this.items.TryGetValue(id, out data);
        }

        public bool TryGetItemData(InventoryObjectType type, out ResMiscInventoryItemData data) {
            data = null;
            foreach (var kv in this.items) {
                if (kv.Value.Type() == type) {
                    data = kv.Value;
                    return true;
                }
            }
            return false;
        }

        //public ResMiscInventoryItemData CreditsData() {
        //    ResMiscInventoryItemData credits = null;
        //    TryGetItemData(InventoryObjectType.credits, out credits);
        //    return credits;
        //}
    }
}
