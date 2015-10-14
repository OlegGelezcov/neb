using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using ServerClientCommon;

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
            get {
                return false;
            }
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

        public void ParseInfo(Hashtable info) {
            mRaw = info;
            Id = info.GetValue<string>((int)SPC.Id, string.Empty);
            templateId = info.GetValue<string>((int)SPC.Template, string.Empty);
        }
    }
}
