using Common;
using ServerClientCommon;
using Space.Game.Inventory;
using System.Collections;

namespace Nebula.Inventory.Objects {

    public class NebulaElementObject : IInventoryObject, ISplittable  {

        public string templateId { get; private set; }
        private Hashtable mRaw;

        public NebulaElementObject(string inID, string templateID) {
            Id = inID;
            templateId = templateID;
            isNew = true;
        }

        public NebulaElementObject(Hashtable itemInfo) {
            ParseInfo(itemInfo);
        }

        public bool binded {
            get;
            private set;
        }

        public bool isNew {
            get;
            private set;
        }

        public void ResetNew() {
            isNew = false;
        }
        public void SetNew(bool val) {
            isNew = val;
        }
        public string Id {
            get;
            private set;
        }

        public int Level {
            get {
                return 1;
            }
        }

        public int placingType {
            get {
                return (int)PlacingType.Inventory;
            }
        }

        public Hashtable rawHash {
            get {
                if(mRaw == null ) {
                    mRaw = GetInfo();
                }
                return mRaw;
            }
        }

        public bool splittable {
            get {
                return true;
            }
        }

        public IInventoryObject splittedCopy {
            get {
                return new NebulaElementObject(System.Guid.NewGuid().ToString(), templateId);
            }
        }

        public InventoryObjectType Type {
            get {
                return InventoryObjectType.nebula_element;
            }
        }

        public void Bind() {
            
        }

        public Hashtable GetInfo() {
            Hashtable hash = new Hashtable();
            hash.Add((int)SPC.Id, Id);
            hash.Add((int)SPC.Level, Level);
            hash.Add((int)SPC.ItemType, (int)(byte)Type);
            hash.Add((int)SPC.Template, templateId);
            hash.Add((int)SPC.PlacingType, placingType);
            hash.Add((int)SPC.Binded, binded);
            hash.Add((int)SPC.Splittable, splittable);
            hash.Add((int)SPC.IsNew, isNew);
            return hash;
        }

        public void ParseInfo(Hashtable info) {
            mRaw = info;
            Id = info.GetValue<string>((int)SPC.Id, string.Empty);
            templateId = info.GetValue<string>((int)SPC.Template, string.Empty);
            binded = info.GetValue<bool>((int)SPC.Binded, false);
            isNew = info.GetValue<bool>((int)SPC.IsNew, false);
        }
    }
}
