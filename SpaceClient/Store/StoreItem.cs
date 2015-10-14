using Common;
using ServerClientCommon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Client.Store {
    public class StoreItem : IInfoParser {
        public string storeItemID { get; private set; } = string.Empty;
        public int count { get; private set; } = 0;
        public Hashtable objectInfo { get; private set; } = new Hashtable();
        public long time { get; private set; } = 0;
        public int price { get; private set; } = 0;

        public void ParseInfo(Hashtable info) {
            storeItemID = info.Value<string>((int)SPC.Id, string.Empty);
            count = info.Value<int>((int)SPC.Count, 0);
            objectInfo = info.Value<Hashtable>((int)SPC.AttachedObject, new Hashtable());
            time = info.Value<int>((int)SPC.Time, 0);
            price = info.GetValue<int>((int)SPC.Price, 0);
        }

        public StoreItem() {

        }

        public StoreItem(Hashtable info) {
            ParseInfo(info);
        }
    }
}
