using Common;
using ServerClientCommon;
using ExitGames.Client.Photon;
using System.Collections.Generic;
using Nebula.Client.Utils;

namespace Nebula.Client.Inventory.Objects {
    public class SchemeInventoryObjectInfo : IInventoryObjectInfo {
        private string id;
        private string name;
        private int level;
        private Workshop workshop;
        private InventoryObjectType type;
        private ObjectColor color;
        public string set { get; private set; }
        public bool binded { get; private set; }

        private string targetTemplateId;

        private Dictionary<string, int> craftMaterials;

        public SchemeInventoryObjectInfo() { }

        public SchemeInventoryObjectInfo(Hashtable info) {
            this.ParseInfo(info);
        }

        public void ParseInfo(Hashtable info) {
            rawHash = info;
            this.id = info.GetValueString((int)SPC.Id);
            this.name = info.GetValueString((int)SPC.Name);
            this.level = info.GetValueInt((int)SPC.Level, 0);
            this.workshop = info.GetValueByte((int)SPC.Workshop).toEnum<Workshop>();
            this.type = info.GetValueByte((int)SPC.ItemType).toEnum<InventoryObjectType>();
            this.targetTemplateId = info.GetValueString((int)SPC.Template);
            this.color = (ObjectColor)(byte)info.GetValueInt((int)SPC.Color);

            if (info.ContainsKey((int)SPC.Set)) {
                set = info.GetValueString((int)SPC.Set);
            } else {
                set = string.Empty;
            }

            Hashtable craftMaterialsHash = info.GetValueHash((int)SPC.CraftMaterials);
            this.craftMaterials = craftMaterialsHash.toDict<string, int>();
            binded = info.GetValueBool((int)SPC.Binded);
        }



        public InventoryObjectType Type {
            get { return type; }
        }

        public string Id {
            get { return id; }
        }

        public string Name {
            get { return name; }
        }

        public int Level {
            get { return this.level; }
        }

        public Workshop Workshop {
            get { return this.workshop; }
        }

        public string TargetTemplateId {
            get { return this.targetTemplateId; }
        }

        public ObjectColor Color {
            get {
                return this.color;
            }
        }

        public ObjectColor MyColor() {
            return this.Color;
        }

        public Dictionary<string, int> CraftMaterials {
            get {
                return this.craftMaterials;
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

        public bool HasColor() {
            return true;
        }
    }
}
