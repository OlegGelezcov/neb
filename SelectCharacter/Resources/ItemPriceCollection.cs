using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Linq;
using Common;
using System;
using System.Collections;
using ServerClientCommon;

namespace SelectCharacter.Resources {

    public class ItemPriceCollection : IInfoSource  {

        private List<ItemPrice> mPrices = new List<ItemPrice>();

        public Hashtable GetInfo() {
            List<object> prices = new List<object>();
            foreach(var p in mPrices) {
                prices.Add(p.GetInfo());
            }
            return new Hashtable { { (int)SPC.Info, prices.ToArray() } };
        }

        public void Load(string basePath) {

            mPrices.Clear();

            string fullPath = Path.Combine(basePath, "assets/item_price.xml");

            XDocument document = XDocument.Load(fullPath);
            mPrices = document.Element("item_price").Elements("object").Select(e => {
                string sType = e.GetString("type");
                switch (sType) {
                    case "module":
                    case "weapon":
                    case "scheme":
                        return (ItemPrice)(new ColoredItemPrice { itemType = sType, color = (ObjectColor)Enum.Parse(typeof(ObjectColor), e.GetString("color")), price = e.GetInt("price") });
                    case "ore":
                        return (ItemPrice)(new IDItemPrice { itemType = sType, id = e.GetString("id"), price = e.GetInt("price") });
                    case "nebula_element":
                        return (ItemPrice)(new NebulaElementItemPrice { itemType = sType, price = e.GetInt("price") });
                    default:
                        throw new Exception("invalid item price type");
                }
            }).ToList();


        }

        public bool TryGetPrice(Hashtable objectInfo, out int price) {
            price = 0;
            int placingType = objectInfo.Value<int>((int)SPC.PlacingType);
            if(placingType == (int)PlacingType.Station) {
                //its module
                ObjectColor color = (ObjectColor)(byte)objectInfo.Value<byte>((int)SPC.Color);
                foreach(var pr in mPrices) {
                    if(pr.itemType == "module" && ((ColoredItemPrice)pr).color == color) {
                        price = pr.price;
                        return true;
                    }
                }
            } else if(placingType == (int)PlacingType.Inventory) {
                InventoryObjectType itemType = (InventoryObjectType)objectInfo.Value<byte>((int)SPC.ItemType);
                if(itemType == InventoryObjectType.Weapon) {
                    ObjectColor color = (ObjectColor)(byte)objectInfo.Value<int>((int)SPC.Color);
                    foreach (var pr in mPrices) {
                        if (pr.itemType == "weapon" && ((ColoredItemPrice)pr).color == color) {
                            price = pr.price;
                            return true;
                        }
                    }
                } else if(itemType == InventoryObjectType.Scheme) {
                    ObjectColor color = (ObjectColor)(byte)objectInfo.Value<int>((int)SPC.Color);
                    foreach (var pr in mPrices) {
                        if (pr.itemType == "scheme" && ((ColoredItemPrice)pr).color == color) {
                            price = pr.price;
                            return true;
                        }
                    }
                } else if(itemType == InventoryObjectType.Material) {
                    string id = objectInfo.Value<string>((int)SPC.Id);
                    foreach (var pr in mPrices) {
                        if (pr.itemType == "ore" && ((IDItemPrice)pr).id == id) {
                            price = pr.price;
                            return true;
                        }
                    }
                } else if(itemType == InventoryObjectType.nebula_element) {
                    foreach(var pr in mPrices) {
                        if(pr.itemType == "nebula_element") {
                            price = pr.price;
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
