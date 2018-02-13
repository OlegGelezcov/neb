

using Common;

namespace Nebula.Client.Inventory.Objects {
    public interface IRaceableInventoryObject {
        int race { get; }
        Race Race { get; }
    }
}
