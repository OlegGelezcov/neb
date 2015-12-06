using Common;
using ServerClientCommon;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;

namespace Nebula.Client.Inventory.Objects {
    public class MaterialInventoryObjectInfo : IInventoryObjectInfo {
        private string id;
        private MaterialType materialType;
        private string templateId;
        private string name;
        private int level;
        private Workshop workshop;

        public bool binded { get; private set; }

        public MaterialInventoryObjectInfo(Hashtable info) {
            this.ParseInfo(info);
        }

        public string Name {
            get {
                return this.name;
            }
        }

        public int Level {
            get {
                return this.level;
            }
        }

        public Workshop Workshop {
            get { return this.workshop; }
        }

        public InventoryObjectType Type {
            get { return InventoryObjectType.Material; }
        }

        public string Id {
            get { return this.id; }
        }

        public MaterialType MaterialType {
            get {
                return this.materialType;
            }
        }

        public string TemplateId {
            get {
                return this.templateId;
            }
        }

        public int placingType {
            get {
                return (int)PlacingType.Inventory;
            }
        }

        public Hashtable rawHash {
            get;
            private set;
        }

        public Hashtable GetInfo() {
            return rawHash;
        }

        public void ParseInfo(Hashtable info) {
            rawHash = info;
            this.id = info.GetValueString((int)SPC.Id);
            this.level = info.GetValueInt((int)SPC.Level);
            this.workshop = (Workshop)(byte)info.GetValueInt((int)SPC.Workshop, (int)Workshop.DarthTribe.toByte());
            this.name = info.GetValueString((int)SPC.Name);
            this.materialType = (MaterialType)(int)info.GetValueInt((int)SPC.MaterialType, (int)MaterialType.ore.toByte());
            this.templateId = info.GetValueString((int)SPC.Template);
            binded = info.GetValueBool((int)SPC.Binded);
        }

        public bool HasColor() {
            return false;
        }

        public ObjectColor MyColor() {
            return ObjectColor.white;
        }
    }
}
