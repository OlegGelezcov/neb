using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using System.Collections;
using ServerClientCommon;

namespace Nebula.Client.Inventory.Objects
{
    public class MaterialInventoryObjectInfo : IInventoryObjectInfo
    {
        private string id;
        private MaterialType materialType;
        private string templateId;
        private string name;
        private int level;
        private Workshop workshop;
        
        public bool binded { get; private set; }

        public MaterialInventoryObjectInfo(Hashtable info )
        {
            this.ParseInfo(info);
        }

        public string Name
        {
            get
            {
                return this.name;
            }
        }

        public int Level
        {
            get
            {
                return this.level;
            }
        }

        public Workshop Workshop
        {
            get { return this.workshop; }
        }

        public InventoryObjectType Type
        {
            get { return InventoryObjectType.Material; }
        }

        public string Id
        {
            get { return this.id; }
        }

        public MaterialType MaterialType
        {
            get
            {
                return this.materialType;
            }
        }

        public string TemplateId
        {
            get
            {
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

        public Hashtable GetInfo()
        {
            return rawHash;
        }

        public void ParseInfo(Hashtable info)
        {
            rawHash = info;
            this.id             =   info.GetValue<string>((int)SPC.Id, string.Empty);
            this.level          =   info.GetValue<int>((int)SPC.Level, 0);
            this.workshop       =   (Workshop)(byte)info.GetValue<int>((int)SPC.Workshop, (int)Workshop.DarthTribe.toByte());
            this.name           =   info.GetValue<string>((int)SPC.Name, string.Empty);
            this.materialType   =   (MaterialType)(int)info.GetValue<int>((int)SPC.MaterialType, (int)MaterialType.ore.toByte());
            this.templateId     =   info.GetValue<string>((int)SPC.Template, string.Empty);
            binded = info.GetValue<bool>((int)SPC.Binded, false);
        }

        public bool HasColor() {
            return false;
        }

        public ObjectColor MyColor() {
            return ObjectColor.white;
        }
    }
}
