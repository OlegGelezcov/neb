using Common;
using ServerClientCommon;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;

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
            Id = info.GetValueString((int)SPC.Id);

            //no need make this, but for debugging 
            name = info.GetValueString((int)SPC.Name);
            level = info.GetValueInt((int)SPC.Level, 1);
            color = (ObjectColor)(byte)info.GetValueInt((int)SPC.Color, (int)ObjectColor.white);
        }
    }
}
