using System;
using System.Collections;
using Space.Game.Drop;
using Common;

namespace Space.Game.Inventory
{
    public interface IInventoryObject : IDroppable, IInventoryObjectBase
    {
        bool binded { get; }

        void Bind();

        bool splittable { get; }
    }

    public interface ISplittable {
        IInventoryObject splittedCopy { get; }
    }
}

