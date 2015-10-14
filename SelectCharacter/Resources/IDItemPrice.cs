using ServerClientCommon;
using System.Collections;

namespace SelectCharacter.Resources {
    public class IDItemPrice : ItemPrice {
        public string id { get; set; }

        public override Hashtable GetInfo() {
            return new Hashtable {
                {(int)SPC.Type, itemType },
                {(int)SPC.Id,  id },
                {(int)SPC.Price, price }
            };
        }
    }
}
