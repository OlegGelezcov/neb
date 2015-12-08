using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Common
{
    public interface IInventoryObjectBase : IInfo, IPlacingType
    {
        string Id { get; }
        InventoryObjectType Type { get; }
        Hashtable rawHash { get; }
    }
}
