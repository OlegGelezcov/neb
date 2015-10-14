using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Common;
using ServerClientCommon;

namespace Nebula.Client.Store {
    public class IDItemPrice : ItemPrice {

        public string id { get; private set; }

        public IDItemPrice(Hashtable hash) : base(hash) {
            id = hash.GetValue<string>((int)SPC.Id, string.Empty);
        }
    }
}
