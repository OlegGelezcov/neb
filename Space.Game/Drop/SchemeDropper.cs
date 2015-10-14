using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using System.Collections;
using Space.Game.Inventory.Objects;
using Space.Game.Resources;
using Space.Game.Ship;
using GameMath;

namespace Space.Game.Drop
{
    /// <summary>
    /// Dropper for schemes
    /// </summary>
    public class SchemeDropper : Dropper
    {
        private Workshop workshop;
        private int level;
        private string name;
        private string templateId;
        private string set = string.Empty;
        private IRes resource;

        private SchemeCraftOreCount craftOreCount;
        private ObjectColor schemeColor;
        private Dictionary<string, int> craftingMaterials;

        public SchemeDropper(Workshop workshop, int level, IRes resource, ObjectColor color) {
            this.resource = resource;
            this.workshop = workshop;
            this.level = level;
            this.name = string.Empty;
            this.schemeColor = color;
            Init();
        }

        public SchemeDropper(Workshop workshop, int level, IRes resource, float remapWeight = 0f)
        {
            this.resource = resource;
            this.workshop = workshop;
            this.level = level;
            this.name = string.Empty;
            this.schemeColor = this.resource.ColorRes.GenColor(ColoredObjectType.Scheme, remapWeight).color;
            Init();

        }

        private void Init() {
            List<ModuleInfo> filteredSetModules = new List<ModuleInfo>();

            if (schemeColor == ObjectColor.green) {
                var moduleList = resource.ModuleTemplates.ModulesWithSet(workshop);

                foreach (var m in moduleList) {
                    foreach (var sm in m.allowedSets) {
                        var s = resource.Sets.Set(sm.Trim());
                        if (s.UnlockLevel <= level && s.Workshop == workshop) {
                            filteredSetModules.Add(m);
                        }
                    }
                }
                if (filteredSetModules.Count == 0) {
                    schemeColor = ObjectColor.yellow;
                }
            }

            //generate green module
            if (schemeColor == ObjectColor.green) {
                var targetModule = filteredSetModules[Rand.Int(0, filteredSetModules.Count - 1)];

                List<ModuleSetData> sets = new List<ModuleSetData>();

                foreach (var s in targetModule.allowedSets) {
                    var set = resource.Sets.Set(s);
                    if (set.UnlockLevel <= level && set.Workshop == workshop) {
                        sets.Add(set);
                    }
                }

                var defaultSet = sets.Where(s => s.IsDefault).FirstOrDefault();

                if (defaultSet == null) {
                    throw new Exception("default set not found for module " + targetModule.Id);
                }

                List<ModuleSetData> lstForArr = new List<ModuleSetData>();
                foreach (var s in sets) {
                    if (!s.IsDefault) {
                        lstForArr.Add(s);
                    }
                }
                lstForArr.Add(defaultSet);

                float[] weights = new float[lstForArr.Count];
                float acc = 0f;
                for (int i = 0; i < weights.Length - 1; i++) {
                    weights[i] = lstForArr[i].DropProb;
                    acc += weights[i];
                }
                weights[weights.Length - 1] = 1f - acc;
                int index = Rand.RandomIndex(weights);
                ModuleSetData finalSet = lstForArr[index];

                templateId = targetModule.Id;
                set = finalSet.Id;

            } else {
                //generate non green module
                var module = resource.ModuleTemplates.RandomModule(workshop);
                templateId = module.Id;
                set = string.Empty;
            }
            this.SetCraftOreCount(this.schemeColor);

            var moduleData = resource.ModuleTemplates.Module(templateId);
            if (moduleData != null && resource.CraftingMaterials.Contains(level, moduleData.Type)) {
                craftingMaterials = resource.CraftingMaterials.GetCraftingMaterials(level, moduleData.Type, schemeColor);
            } else {
                this.craftingMaterials = resource.Materials.GenOres(2, this.craftOreCount.GetCount());
            }
        }

        private void SetCraftOreCount(ObjectColor color)
        {
            switch(color)
            {
                case ObjectColor.white:
                    this.craftOreCount = new WhiteSchemeCraftOreCount();
                    break;
                case ObjectColor.blue:
                    this.craftOreCount = new BlueSchemeCraftOreCount();
                    break;
                case ObjectColor.yellow:
                    this.craftOreCount = new YellowSchemeCraftOreCount();
                    break;
                case ObjectColor.green:
                    this.craftOreCount = new GreenSchemeCraftOreCount();
                    break;
                case ObjectColor.orange:
                    this.craftOreCount = new OrangeSchemeCraftOreCount();
                    break;
                default:
                    throw new Exception("Invalid scheme color");
            }
        }

        public override IDroppable Drop()
        {


            var schemeInitData = new SchemeObject.SchemeInitData(Guid.NewGuid().ToString(), name, this.level, this.workshop, this.templateId, this.schemeColor, this.craftingMaterials, set);
            return new SchemeObject(schemeInitData);
        }

        public abstract class SchemeCraftOreCount
        {
            public abstract int GetCount();
        }

        public class WhiteSchemeCraftOreCount : SchemeCraftOreCount
        {
            public override int GetCount()
            {
                return 2;
            }
        }

        public class BlueSchemeCraftOreCount : SchemeCraftOreCount
        {
            public override int GetCount()
            {
                return 4;
            }
        }

        public class YellowSchemeCraftOreCount : SchemeCraftOreCount
        {
            public override int GetCount()
            {
                return 6;
            }
        }

        public class GreenSchemeCraftOreCount : SchemeCraftOreCount
        {
            public override int GetCount()
            {
                return 8;
            }
        }

        public class OrangeSchemeCraftOreCount : SchemeCraftOreCount
        {
            public override int GetCount()
            {
                return 10;
            }
        }
    }
}
