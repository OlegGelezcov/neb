using Common;
using ServerClientCommon;
using Space.Game.Drop;
using System.Collections;
using System.Collections.Generic;
using System;
using GameMath;

namespace Space.Game.Inventory.Objects
{
    public class SchemeObject : IInventoryObject, ITransformable
    {
        private string id;
        private string name;
        private int level;
        private Workshop workshop;
        private string moduleTemplateId;
        private Dictionary<string, int> craftingMaterials;
        private ObjectColor color;
        private string setID;
        private Hashtable mRaw;

        public bool binded { get; private set; } = false;

        public void Bind() { binded = true; }

        public class SchemeInitData
        {
            public string Id { get; private set; }
            public string Name { get; private set; }
            public int Level {get; private set;}
            public Workshop Workshop {get; private set;}
            public string TemplateModuleId { get; private set; }
            public ObjectColor Color { get; private set; }
            public Dictionary<string, int> CraftingMaterials { get; private set; }
            public string setID = string.Empty;

            public SchemeInitData(string id, string name, int level, Workshop workshop, string templateModuleId, ObjectColor color,
                Dictionary<string, int> craftingMaterials, string inSetID)
            {
                this.Id = id;
                this.Name = name;
                this.Level = level;
                this.Workshop = workshop;
                this.TemplateModuleId = templateModuleId;
                this.Color = color;
                this.CraftingMaterials = craftingMaterials;
                setID = inSetID;
            }
        }

        public SchemeObject(SchemeInitData schemeInitData)
        {
            this.id = schemeInitData.Id;
            this.name = schemeInitData.Name;
            this.level = schemeInitData.Level;
            this.workshop = schemeInitData.Workshop;
            this.moduleTemplateId = schemeInitData.TemplateModuleId;
            this.color = schemeInitData.Color;
            this.craftingMaterials = schemeInitData.CraftingMaterials;
            setID = schemeInitData.setID;
            mRaw = GetInfo();
        }

        public void TryChangeColor(float prob) {
            if(color == ObjectColor.white ) {
                ObjectColor[] allowedColors = new ObjectColor[] { ObjectColor.yellow, ObjectColor.blue, ObjectColor.orange };
                if(Rand.Float01() < prob) {
                    int index = Rand.Int(2);
                    color = allowedColors[index];
                }
            }
        }



        /*
        private void InitCraftMaterials()
        {
            this.PrepareMaterials();

            string firstOre = this.resource.Materials.RandomOre().Id;
            string secondOre = this.resource.Materials.RandomOreExcept(new List<string> { firstOre }).Id;
            this.craftMaterials.Add(firstOre, 4);
            this.craftMaterials.Add(secondOre,4);
        }

        private void PrepareMaterials()
        {
            if (this.craftMaterials == null)
                this.craftMaterials = new Dictionary<string, int>();
            else
                this.craftMaterials.Clear();
        }*/

        public SchemeObject(Hashtable info )
        {
            this.ParseInfo(info);
        }


        public string Name
        {
            get { return this.name; }
        }

        public Hashtable GetInfo()
        {
            Hashtable result = new Hashtable();
            result.Add((int)SPC.Id, this.id);
            result.Add((int)SPC.Name, this.name);
            result.Add((int)SPC.Level, this.level);
            result.Add((int)SPC.Workshop, (int)this.workshop.toByte());
            result.Add((int)SPC.ItemType, (int)this.Type.toByte());
            result.Add((int)SPC.Template, this.moduleTemplateId);
            result.Add((int)SPC.Color, (int)(byte)this.color);
            result.Add((int)SPC.CraftMaterials, this.craftingMaterials.toHash<string, int>());
            result.Add((int)SPC.PlacingType, placingType);
            result.Add((int)SPC.Set, setID);
            result.Add((int)SPC.Binded, binded);
            result.Add((int)SPC.Splittable, splittable);

            return result;
        }

        public void ParseInfo(Hashtable info)
        {
            mRaw = info;
            this.id = info.GetValue<string>((int)SPC.Id, string.Empty);
            this.name = info.GetValue<string>((int)SPC.Name, string.Empty);
            this.level = info.GetValue<int>((int)SPC.Level, 0);
            this.workshop = (Workshop)(byte)info.GetValue<int>((int)SPC.Workshop, Workshop.DarthTribe.toByte());
            this.moduleTemplateId = info.GetValue<string>((int)SPC.Template, string.Empty);
            this.color = (ObjectColor)(byte)(int)info.GetValue<int>((int)SPC.Color, 0);
            
            Hashtable ht = info.GetValue<Hashtable>((int)SPC.CraftMaterials, new Hashtable());
            this.craftingMaterials = ht.toDict<string, int>();

            if(info.ContainsKey((int)SPC.Set)) {
                setID = info.GetValue<string>((int)SPC.Set, string.Empty);
            } else {
                setID = string.Empty;
            }
            binded = info.GetValue<bool>((int)SPC.Binded, false);
        } 

        public int Level
        {
            get { return this.level; }
        }

        public Common.Workshop Workshop
        {
            get { return this.workshop; }
        }

        public InventoryObjectType Type
        {
            get { return InventoryObjectType.Scheme; }
        }

        public string Id
        {
            get { return this.id; }
        }

        public string ModuleId
        {
            get { return this.moduleTemplateId; }
        }


        public Dictionary<string, int> CraftMaterials
        {
            get
            {
                return this.craftingMaterials;
            }
        }

        public void ReplaceCraftingMaterials(Dictionary<string, int> crMaterials) {
            craftingMaterials = crMaterials;
            mRaw = GetInfo();
        }

        public ObjectColor Color
        {
            get
            {
                return this.color;
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

        public string SetID {
            get {
                return setID;
            }
        }

        public bool splittable {
            get {
                return false;
            }
        }

        public object Transform(DropManager dropManager)
        {
            return dropManager.TransformScheme(this);
        }

    }
}
