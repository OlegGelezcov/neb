using ServerClientCommon;
using System.Collections;

namespace SelectCharacter.Resources {
    public class NebulaElementItemPrice : ItemPrice {
        public override Hashtable GetInfo() {
            return new Hashtable {
                { (int)SPC.Type, itemType },
                { (int)SPC.Price, price }
            };
        }
    }
}
