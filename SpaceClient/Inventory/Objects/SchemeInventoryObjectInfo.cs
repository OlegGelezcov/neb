using Common;
using ServerClientCommon;
using System.Collections;
using System.Collections.Generic;

namespace Nebula.Client.Inventory.Objects {
    public class SchemeInventoryObjectInfo : IInventoryObjectInfo
    {
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

        public SchemeInventoryObjectInfo(Hashtable info)
        {
            this.ParseInfo(info);
        }

        public void ParseInfo(System.Collections.Hashtable info)
        {
            rawHash = info;
            this.id = info.GetValue<string>((int)SPC.Id, string.Empty);
            this.name = info.GetValue<string>((int)SPC.Name, string.Empty);
            this.level = info.GetValue<int>((int)SPC.Level, 0);
            this.workshop = info.GetValue<byte>((int)SPC.Workshop, 0).toEnum<Workshop>();
            this.type = info.GetValue<byte>((int)SPC.ItemType, 0).toEnum<InventoryObjectType>();
            this.targetTemplateId = info.GetValue<string>((int)SPC.Template, string.Empty);
            this.color = (ObjectColor)(byte)info.GetValue<int>((int)SPC.Color, 0);

            if(info.ContainsKey((int)SPC.Set)) {
                set = info.GetValue<string>((int)SPC.Set, string.Empty);
            } else {
                set = string.Empty;
            }

            Hashtable craftMaterialsHash = info.GetValue<Hashtable>((int)SPC.CraftMaterials, new Hashtable());
            this.craftMaterials = craftMaterialsHash.toDict<string, int>();
            binded = info.GetValue<bool>((int)SPC.Binded, false);
        }

        

        public InventoryObjectType Type
        {
            get { return type; }
        }

        public string Id
        {
            get { return id; }
        }

        public string Name
        {
            get { return name; }
        }

        public int Level
        {
            get { return this.level; }
        }

        public Workshop Workshop
        {
            get { return this.workshop; }
        }

        public string TargetTemplateId
        {
            get { return this.targetTemplateId; }
        }

        public ObjectColor Color
        {
            get
            {
                return this.color;
            }
        }

        public ObjectColor MyColor() {
            return this.Color;
        }

        public Dictionary<string, int> CraftMaterials
        {
            get
            {
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

        public Hashtable GetInfo()
        {
            return rawHash;
        }

        public bool HasColor() {
            return true;
        }
    }
}
