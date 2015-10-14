using Space.Game.Drop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Space.Game.Inventory
{
    /// <summary>
    /// define object which might transform to other object
    /// </summary>
    public interface ITransformable
    {
        object Transform(DropManager dropper);
    }
}

