using System;
using Common;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System.Collections;
using Space.Game.Ship;
using System.IO;
using Space.Game.Inventory.Objects;
using GameMath;
using Space.Game.Resources;

namespace Space.Game.Drop
{
    public class DropManager
    {

        private IRes resources;

        private DropManager(IRes resources ) 
        {
            this.resources = resources;
        }

        public ModuleDropper GetModuleDropper(ModuleDropper.ModuleDropParams dropParams)
        {
            return new ModuleDropper(dropParams);
        }

        public WeaponDropper GetWeaponDropper(WeaponDropper.WeaponDropParams dropParams, float remapWeight = 0f)
        {
            return new WeaponDropper(dropParams, remapWeight);
        }

        public SchemeDropper GetSchemeDropper(Workshop w, int level, float remapWeight = 0f)
        {
            return new SchemeDropper(w, level,  this.resources, remapWeight);
        }

        public static DropManager Get( IRes resources)
        {
            return new DropManager(resources);
        }

        public IRes Resources
        {
            get
            {
                return this.resources;
            }
        }

        public ShipModule TransformScheme(SchemeObject scheme)
        {
            ModuleDropper.ModuleDropParams dropParams = new ModuleDropper.ModuleDropParams(this.resources, scheme.ModuleId, scheme.Level, Difficulty.none, scheme.CraftMaterials, scheme.Color, scheme.SetID);
            ModuleDropper dropper = new ModuleDropper(dropParams);
            return (ShipModule)dropper.Drop();
        }

        public class GeneratedSetInfo {
            public string setId { get; set; }
            public string moduleTemplateId { get; set; }
        }

        public GeneratedSetInfo GenerateSet(int level, Workshop workshop, ShipModelSlotType slotType) {
            List<ModuleInfo> filteredModules = new List<ModuleInfo>();

            var moduleList = resources.ModuleTemplates.ModulesWithSet(workshop, slotType);

            foreach(var module in moduleList ) {
                foreach(var moduleSet in module.allowedSets ) {
                    var setResource = resources.Sets.Set(moduleSet.Trim());
                    if(setResource.UnlockLevel <= level && setResource.Workshop == workshop ) {
                        filteredModules.Add(module);
                    }
                }
            }

            if(filteredModules.Count == 0 ) {
                return new GeneratedSetInfo {
                    setId = string.Empty,
                    moduleTemplateId = string.Empty
                };
            }

            var targetModule = filteredModules[Rand.Int(0, filteredModules.Count - 1)];

            List<ModuleSetData> setList = new List<ModuleSetData>();

            foreach(var set in targetModule.allowedSets) {
                var checkedSet = resources.Sets.Set(set);
                if(checkedSet.UnlockLevel <= level && checkedSet.Workshop == workshop ) {
                    setList.Add(checkedSet);
                }
            }

            var defaultSet = setList.Where(set => set.IsDefault).FirstOrDefault();
            if(defaultSet == null ) {
                return new GeneratedSetInfo {
                    setId = string.Empty,
                    moduleTemplateId = string.Empty
                };
            }

            List<ModuleSetData> tempList = new List<ModuleSetData>();
            foreach(var set in setList ) {
                if(false == set.IsDefault ) {
                    tempList.Add(set);
                }
            }
            tempList.Add(defaultSet);

            float[] weights = new float[tempList.Count];
            float acc = 0f;
            for (int i = 0; i < weights.Length - 1; i++) {
                weights[i] = tempList[i].DropProb;
                acc += weights[i];
            }
            weights[weights.Length - 1] = Mathf.ClampLess(1f - acc, 0f);
            int index = Rand.RandomIndex(weights);
            ModuleSetData finalSetModule = tempList[index];
            return new GeneratedSetInfo {
                setId = finalSetModule.Id,
                moduleTemplateId = targetModule.Id
            };
        }
    }
}
