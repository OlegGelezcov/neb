using Common;
#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif

namespace ServerClientCommon {

    public class BankSlotPrice {
        public readonly int slots;
        public readonly int price;

#if UP
        public BankSlotPrice(UPXElement element) {
            slots = element.GetAttributeInt("count");
            price = element.GetAttributeInt("price");
        }
#else
        public BankSlotPrice(XElement element) {
            slots = element.GetInt("count");
            price = element.GetInt("price");
        }
#endif
    }
}
