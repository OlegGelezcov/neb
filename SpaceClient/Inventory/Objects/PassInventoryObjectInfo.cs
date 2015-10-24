using Common;
using ServerClientCommon;
using System.Collections;

namespace Nebula.Client.Inventory.Objects {
    public class PassInventoryObjectInfo : IInventoryObjectInfo {

        public PassInventoryObjectInfo(Hashtable hash) {
            ParseInfo(hash);
        }

        /// <summary>
        /// Always one
        /// </summary>
        public int level { get; private set; }
        /// <summary>
        /// Always empty
        /// </summary>
        public string name { get; private set; }

        /// <summary>
        /// Always white
        /// </summary>
        public ObjectColor color { get; private set; }
        /// <summary>
        /// Pass never not bindable
        /// </summary>
        public bool binded {
            get {
                return false;
            }
        }

        public string Id {
            get;
            private set;
        }

        /// <summary>
        /// Now all objects has placing type inventory
        /// </summary>
        public int placingType {
            get {
                return (int)PlacingType.Inventory;
            }
        }

        /// <summary>
        /// Not transformable hash received from server
        /// </summary>
        public Hashtable rawHash {
            get;
            private set;
        }

        public InventoryObjectType Type {
            get {
                return InventoryObjectType.pass;
            }
        }

        public Hashtable GetInfo() {
            return rawHash;
        }

        public bool HasColor() {
            return false;
        }

        public ObjectColor MyColor() {
            return color;
        }

        public void ParseInfo(Hashtable info) {
            rawHash = info;
            Id = info.GetValue<string>((int)SPC.Id, string.Empty);

            //no need make this, but for debugging 
            name = info.GetValue<string>((int)SPC.Name, string.Empty);
            level = info.GetValue<int>((int)SPC.Level, 1);
            color = (ObjectColor)(byte)info.GetValue<int>((int)SPC.Color, (int)ObjectColor.white);
        }
    }
}
