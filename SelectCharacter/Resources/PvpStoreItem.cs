using Common;
using System.Xml.Linq;
using System;
using System.Collections;
using ServerClientCommon;

namespace SelectCharacter.Resources {
    public class PvpStoreItem : IInfoSource {
        public string type { get; private set; }
        public int price { get; private set; }

        public PvpStoreItem(XElement e) {
            type = e.GetString("type");
            price = e.GetInt("price");
        }

        public bool isWeapon {
            get {
                return (type != null) && (type.ToLower() == "wp");
            }
        }

        public Hashtable GetInfo() {
            return new Hashtable {
                { (int)SPC.Price, price }
            };
        }
    }
}
