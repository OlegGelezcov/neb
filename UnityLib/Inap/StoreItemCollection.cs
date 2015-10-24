using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Nebula.Inap {
    public class StoreItemCollection : IStoreItemCollection {

        public void Load(string xml) {
            XDocument document = XDocument.Parse(xml);
            type = (StoreType)System.Enum.Parse(typeof(StoreType), document.Element("items").GetString("type"));
            items = document.Element("items").Elements("item").Select(itemElement => {
                string id = itemElement.GetString("id");
                string name = itemElement.GetString("name");
                string desc = itemElement.GetString("desc");
                int discount = itemElement.GetInt("discount");
                string price = itemElement.GetString("price");
                string priceEur = itemElement.GetString("price_eur");
                bool available = itemElement.GetBool("available");
                string icon = itemElement.GetString("icon");
                return (BaseStoreItem)(new GoogleStoreItem(id, name, desc, price, discount, available, priceEur, icon));
            }).ToList();
        }

        public List<BaseStoreItem> items {
            get;
            private set;
        }

        public StoreType type {
            get;
            private set;
        }

        public BaseStoreItem GetItem(string id) {
            foreach(var it in items) {
                if(it.id == id ) {
                    return it;
                }
            }
            return null;
        }
    }
}
