using System;
using System.Collections;
using Common;

namespace Nebula.Client.Inventory {
    public interface IInventoryObjectInfo : IInventoryObjectBase, IColorInfo {
        bool binded { get; }
    }
}
