using Common;
using Nebula.Client.Inventory.Objects;

namespace Nebula.Client.Inventory {
    public class ClientInventoryItem : InventoryItem<IInventoryObjectInfo> {
        public ClientInventoryItem(IInventoryObjectInfo obj, int count)
            : base(obj, count) {

        }

        public ClientInventoryItem() : base() { }

        public bool isNew {
            get {
                return Object.isNew;
            }
        }

        public int Level {
            get {
                if(Object != null ) {
                    if(Object is ILeveledObjectInfo) {
                        return (Object as ILeveledObjectInfo).Level;
                    }
                }
                return 0;
            }
        }
    }
}
