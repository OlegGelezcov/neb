using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using ServerClientCommon;

namespace SelectCharacter.Resources {
    public class ColoredItemPrice : ItemPrice {
        public ObjectColor color { get; set; }

        public override Hashtable GetInfo() {
            return new Hashtable {
                {(int)SPC.Type, itemType },
                {(int)SPC.Color, (int)(byte)color },
                {(int)SPC.Price, price }
            };
        }
    }
}
