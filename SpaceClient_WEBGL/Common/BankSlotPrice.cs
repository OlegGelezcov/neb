using Common;
using System.Xml.Linq;

namespace ServerClientCommon {
    public class BankSlotPrice {
        public readonly int slots;
        public readonly int price;

        public BankSlotPrice(XElement element) {
            slots = element.GetInt("count");
            price = element.GetInt("price");
        }
    }
}
