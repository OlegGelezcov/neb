using Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectCharacter.Resources {
    public abstract class ItemPrice : IInfoSource {
        public string itemType { get; set; }
        public int price { get; set; }
        public abstract Hashtable GetInfo();
    }
}
