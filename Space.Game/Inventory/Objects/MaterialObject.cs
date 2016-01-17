using Common;
using Space.Game.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using ServerClientCommon;

namespace Space.Game.Inventory.Objects
{
    public class MaterialObject : IInventoryObject, ISplittable
    {
        private string id;
        private MaterialType materialType;
        private string templateId;
        private string name;
        private int level;
        private Workshop workshop;
        private Hashtable mRaw;

        public bool binded { get; private set; } = false;

        public void Bind() { binded = true; }

        public MaterialObject(string id) {
            this.id = id;
            this.workshop = Workshop.Arlen;
            this.level = 1;
            this.materialType = MaterialType.ore;
            this.name = string.Empty;
            this.templateId = id;
        }

        public MaterialObject(string id, Workshop workshop, int level, MaterialData data)
        {
            this.id = id;
            this.workshop = workshop;
            this.level = level;
            this.materialType = data.Type;
            this.name = data.Name;
            this.templateId = data.Id;
        }

        public MaterialObject(string id, Workshop workshop, int level, MaterialType materialType, string name, string templateId) {
            this.id = id;
            this.workshop = workshop;
            this.level = level;
            this.materialType = materialType;
            this.name = name;
            this.templateId = templateId;
        }

        public IInventoryObject splittedCopy {
            get {
                return new MaterialObject(Guid.NewGuid().ToString(), workshop, level, materialType, name, templateId);
            }
        }

        public MaterialObject(Hashtable info)
        {
            this.ParseInfo(info);
        }

        public string Name
        {
            get {
                return this.name;
            }
        }

        public int Level
        {
            get {
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
            get { return this.id;  }
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
            get {
                return mRaw;
            }
        }

        public bool splittable {
            get {
                return true;
            }
        }



        public Hashtable GetInfo()
        {
            Hashtable result = new Hashtable();
            result.Add((int)SPC.Id, this.Id);
            result.Add((int)SPC.Level, this.Level);
            result.Add((int)SPC.Workshop, this.Workshop.toByte());
            result.Add((int)SPC.ItemType, (int)this.Type.toByte());
            result.Add((int)SPC.Name, this.Name);
            result.Add((int)SPC.MaterialType, this.MaterialType.toByte());
            result.Add((int)SPC.Template, this.TemplateId);
            result.Add((int)SPC.PlacingType, placingType);
            result.Add((int)SPC.Binded, binded);
            result.Add((int)SPC.Splittable, splittable);
            return result;
        }

        public void ParseInfo(Hashtable info)
        {
            mRaw = info;
            this.id = info.GetValue<string>((int)SPC.Id, string.Empty);
            this.level = info.GetValue<int>((int)SPC.Level, 0);
            this.workshop = (Workshop)info.GetValue<byte>((int)SPC.Workshop, Workshop.DarthTribe.toByte());
            this.name = info.GetValue<string>((int)SPC.Name, string.Empty);
            this.materialType = (MaterialType)info.GetValue<byte>((int)SPC.MaterialType, MaterialType.ore.toByte());
            this.templateId = info.GetValue<string>((int)SPC.Template, string.Empty);
            binded = info.GetValue<bool>((int)SPC.Binded, false);
        }
    }
}
