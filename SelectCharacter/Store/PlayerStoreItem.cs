using Common;
using ServerClientCommon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectCharacter.Store {
    public class PlayerStoreItem : IInfoSource {
        public string storeItemID { get; set; }
        public int count { get; set; }
        public Hashtable objectInfo { get; set; }
        public long time { get; set; }
        public int price { get; set; }

        public Hashtable GetInfo() {
            return new Hashtable {
                { (int)SPC.Id, storeItemID },
                { (int)SPC.Count, count },
                { (int)SPC.AttachedObject, objectInfo },
                { (int)SPC.Time, (int)time },
                { (int)SPC.Price, price }
            };
        }
    }
}
