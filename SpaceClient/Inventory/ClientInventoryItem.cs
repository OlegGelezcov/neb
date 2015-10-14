using System;
using Common;

namespace Nebula.Client.Inventory
{
    public class ClientInventoryItem : InventoryItem<IInventoryObjectInfo>
    {
        public ClientInventoryItem(IInventoryObjectInfo obj, int count)
            : base(obj, count)
        {

        }

        public ClientInventoryItem() : base() { }
    }
}
