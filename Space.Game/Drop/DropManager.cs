using System;
using Common;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System.Collections;
using Space.Game.Ship;
using System.IO;
using Space.Game.Inventory.Objects;


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
    }
}
