using System;
using System.Collections;
using Common;
using ServerClientCommon;

namespace Space.Game.Inventory
{
    public class ServerInventoryItem : InventoryItem<IInventoryObject> {
        public ServerInventoryItem(IInventoryObject obj, int count)
            : base(obj, count) {
        }

        public ServerInventoryItem() : base() { }

       

        public override Hashtable GetInfo()
        {
            if (_object == null)
                return new Hashtable { { (int)SPC.Count, _count } };

            var objectInfo = _object.GetInfo();
            
            objectInfo.Add((int)SPC.Count, _count);
            return objectInfo;
        }

       public bool isNew {
            get {
                return Object.isNew;
            }
        }

        public void ResetNew() {
            Object.ResetNew();
        }
    }
}
