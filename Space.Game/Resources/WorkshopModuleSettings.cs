using Common;
using System.Collections;
using System.Collections.Generic;

namespace Space.Game.Resources
{
    /*
    public class WorkshopModuleSettings
    {
        private readonly Workshop workshop;
        private readonly Dictionary<string, object> settings;
        private readonly Dictionary<ShipModelSlotType, ModuleTypeDropSettings> moduleTypeSettings;

        public WorkshopModuleSettings(Workshop workshop, Hashtable inputs, Dictionary<ShipModelSlotType, Hashtable> moduleTypeInputs)
        {
            this.workshop = workshop;
            this.settings = new Dictionary<string, object>();

            foreach(DictionaryEntry entry in inputs)
            {
                this.settings.Add(entry.Key.ToString(), entry.Value);
            }

            this.moduleTypeSettings = new Dictionary<ShipModelSlotType, ModuleTypeDropSettings>();
            foreach(var pair in moduleTypeInputs)
            {
                this.moduleTypeSettings.Add(pair.Key, new ModuleTypeDropSettings(pair.Key, pair.Value));
            }
        }

        public float HPBaseValue()
        {
            return (float)this.settings["HPBaseValue"];
        }

        public float SpeedBaseValue()
        {
            return (float)this.settings["SpeedBaseValue"];
        }

        public float EnergyBaseValue()
        {
            return (float)this.settings["EnergyBaseValue"];
        }

        public float EnergyRestorationBaseValue()
        {
            return (float)this.settings["EnergyRestorationBaseValue"];
        }

        public float HoldCountBase()
        {
            return (float)this.settings["HoldCountBase"];
        }

        public float HPBaseFactor()
        {
            return (float)this.settings["HPBaseFactor"];
        }

        public float SpeedBaseFactor()
        {
            return (float)this.settings["SpeedBaseFactor"];
        }

        public float EnergyFactor()
        {
            return (float)this.settings["EnergyFactor"];
        }

        public float EnergyRestoration()
        {
            return (float)this.settings["EnergyRestoration"];
        }

        public float HoldCountFactor()
        {
            return (float)this.settings["HoldCountFactor"];
        }

        public float RandMin()
        {
            return (float)this.settings["rand_min"];
        }

        public float RandMax()
        {
            return (float)this.settings["rand_max"];
        }

        public bool TryGetModuleTypeSettings(ShipModelSlotType slotType, out ModuleTypeDropSettings result)
        {
            return this.moduleTypeSettings.TryGetValue(slotType, out result);
        }
    }*/
}
