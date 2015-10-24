using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Inap {
    public abstract class BaseStoreItem {

        public string id { get; private set; }
        public string name { get; private set; }
        public string description { get; private set; }
        public string price { get; private set; }
        public bool available { get; private set; }
        public int discount { get; private set; }
        public string icon { get; private set; }

        public BaseStoreItem(string inId, string inName, string inDescription, string inPrice, int inDiscount, bool inAvailable, string inIcon) {
            id = inId;
            name = inName;
            description = inDescription;
            price = inPrice;
            discount = inDiscount;
            available = inAvailable;
            icon = inIcon;
        }

        public abstract StoreType type { get; }
    }
}
