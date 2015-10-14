using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using ServerClientCommon;

namespace Space.Database {
    public static class DatabaseUtils {

        public static List<Hashtable> TransformForInventory(List<InventoryItemDocumentElement> items) {
            List<Hashtable> result = new List<Hashtable>();
            foreach(var item in items) {
                Hashtable ht = new Hashtable {
                    {(int)SPC.Count, item.Count },
                    { (int)SPC.Info, item.Object}
                };
                result.Add(ht);
            }
            return result;
        }
    }
}
