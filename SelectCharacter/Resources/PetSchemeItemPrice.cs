using ServerClientCommon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectCharacter.Resources {
    public class PetSchemeItemPrice : ItemPrice {
        public override Hashtable GetInfo() {
            return new Hashtable {
                { (int)SPC.Type, itemType },
                { (int)SPC.Price, price }
            };
        }
    }
}
