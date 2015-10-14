using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Space.Game.Inventory.Objects
{
    public class InventoryObjectComparer : IEqualityComparer<IInventoryObject>
    {

        public bool Equals(IInventoryObject x, IInventoryObject y)
        {
            return (x.Id == y.Id);
        }

        public int GetHashCode(IInventoryObject obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}

