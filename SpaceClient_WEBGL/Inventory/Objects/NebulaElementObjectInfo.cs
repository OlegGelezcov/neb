using Common;
using ServerClientCommon;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;

namespace Nebula.Client.Inventory.Objects {
    public class NebulaElementObjectInfo : IInventoryObjectInfo {

        private Hashtable mRaw;

        public NebulaElementObjectInfo(Hashtable info) {
            ParseInfo(info);
        }

        public string templateId {
            get;
            private set;
        }

        public bool binded {
            get;
            private set;
        }

        public string Id {
            get;
            private set;
        }

        public int placingType {
            get {
                return (int)PlacingType.Inventory;
            }
        }

        public Hashtable rawHash {
            get {
                return mRaw;
            }
        }

        public InventoryObjectType Type {
            get {
                return InventoryObjectType.nebula_element;
            }
        }

        public Hashtable GetInfo() {
            return mRaw;
        }

        public bool HasColor() {
            return false;
        }

        public ObjectColor MyColor() {
            return ObjectColor.white;
        }
        public bool isNew {
            get;
            private set;
        }

        public void ParseInfo(Hashtable info) {
            mRaw = info;
            Id = info.GetValueString((int)SPC.Id);
            templateId = info.GetValueString((int)SPC.Template);
            binded = info.GetValueBool((int)SPC.Binded);
            isNew = info.GetValueBool((int)SPC.IsNew);
        }
    }
}
