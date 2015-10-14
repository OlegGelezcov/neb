using Common;
using ServerClientCommon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Client.Store {
    public class ColoredItemPrice : ItemPrice {

        public ObjectColor color { get; private set; }

        public ColoredItemPrice(Hashtable hash) : base(hash) {
            color = (ObjectColor)(byte)hash.GetValue<int>((int)SPC.Color, (int)(byte)ObjectColor.white);
        }


    }
}
