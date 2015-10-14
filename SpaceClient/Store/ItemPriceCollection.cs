using Common;
using Nebula.Client.Inventory;
using ServerClientCommon;
using System.Collections;
using System.Collections.Generic;

namespace Nebula.Client.Store {
    public class ItemPriceCollection : IInfoParser{

        public List<ItemPrice> prices { get; private set; } = new List<ItemPrice>();


        public void ParseInfo(Hashtable hash) {
            object[] arr = hash[(int)SPC.Info] as object[];
            prices.Clear();
            if(arr == null ) {
                return;
            }

            foreach(object obj in arr) {
                Hashtable h = obj as Hashtable;
                if(h == null ) {
                    continue;
                }

                var p = ParsePrice(h);
                if(p != null ) {
                    prices.Add(p);
                }
            }
        }

        private ItemPrice ParsePrice(Hashtable hash) {
            string str = hash.GetValue<string>((int)SPC.Type, string.Empty);
            if(string.IsNullOrEmpty(str)) {
                return null;
            }
            switch (str.Trim().ToLower()) {
                case "module": 
                case "weapon": 
                case "scheme":
                    return new ColoredItemPrice(hash);
                case "ore":
                    return new IDItemPrice(hash);
                case "nebula_element":
                    return new NebulaElementItemPrice(hash);
                default:
                    return null;
            }
        }

        public bool TryGetPrice(IInventoryObjectInfo invObject, out int price) {
            price = 0;
            switch(invObject.Type) {
                case InventoryObjectType.Module:
                case InventoryObjectType.Weapon:
                case InventoryObjectType.Scheme:
                    foreach(var p in prices) {
                        if(p is ColoredItemPrice) {
                            if((p as ColoredItemPrice).type == invObject.Type) {
                                price = p.price;
                                return true;
                            }
                        }
                    }
                    return false;
                case InventoryObjectType.Material:
                    foreach(var p in prices) {
                        if(p is IDItemPrice) {
                            if((p as IDItemPrice).id == invObject.Id) {
                                price = p.price;
                                return true;
                            }
                        }
                    }
                    return false;
                case InventoryObjectType.nebula_element:
                    foreach(var p in prices) {
                        if(p is NebulaElementItemPrice) {
                            price = p.price;
                            return true;
                        }
                    }
                    return false;
                default:
                    return false;

            }
        }
    }
}
