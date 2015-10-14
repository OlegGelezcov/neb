using System;
using System.Collections;
using Common;
using System.Collections.Generic;
using Space.Game.Resources;

namespace Space.Game.Ship
{
    public class ShipModel : IInfoSource
    {
        private readonly ShipModelSlotBase es;
        private readonly ShipModelSlotBase cb;
        private readonly ShipModelSlotBase df;
        private readonly ShipModelSlotBase cm;
        private readonly ShipModelSlotBase dm;

        private ShipModelSlotBase[] slots;
        private ShipModelSets sets;

        private IRes resource;

        public const float resistance_aux_number = 15000;
        public const float critical_chance_aux_number = 10000;
        public const float bonus_aux_number = 10000;


        public ShipModel(IRes resource) 
        {
            this.resource = resource;

            es = new ES_ShipModelSlot();
            cb = new CB_ShipModelSlot();
            df = new DF_ShipModelSlot();
            cm = new CM_ShipModelSlot();
            dm = new DM_ShipModelSlot();

            slots = new ShipModelSlotBase[] { es, cb, df, cm, dm };
            this.sets = new ShipModelSets(resource);
        }

        public void Replace(ShipModel other) {
            es.Replace(other.es);
            cb.Replace(other.cb);
            df.Replace(other.df);
            cm.Replace(other.cm);
            dm.Replace(other.dm);
            Update();
        }

        public ShipModelSlotBase SlotForSkillIndex(int index) {
            switch (index) {
                case 0:
                    return CB;
                case 1:
                    return DF;
                case 2:
                    return DM;
                case 3:
                    return CM;
                case 4:
                    return ES;
                default:
                    return null;
            }
        }


        public void SetResource(Res resource) {
            this.resource = resource;
            if(this.sets != null ) {
                this.sets.SetResource(resource);
            }
            this.Update();
        }

        private IRes Resource()
        {
            return this.resource;
        }

        public ShipModelSlotBase Slot(ShipModelSlotType type)
        {
            switch (type)
            {
                case ShipModelSlotType.CB: return cb;
                case ShipModelSlotType.CM: return cm;
                case ShipModelSlotType.DF: return df;
                case ShipModelSlotType.DM: return dm;
                case ShipModelSlotType.ES: return es;
                default:
                    return null;
            }
        }

        public Hashtable GetModulePrefabs()
        {
            Hashtable info = new Hashtable();
            foreach(var slot in this.slots)
            {
                if(slot.HasModule)
                {
                    info.Add(slot.Type.toByte(), slot.Module.Prefab);
                }
            }
            return info;
        }

        public Hashtable ModelHash() {
            Hashtable info = new Hashtable();
            foreach(var slot in Slots) {
                if(slot.HasModule) {
                    info.Add((int)slot.Type.toByte(), slot.Module.TemplateModuleId);
                }
            }
            return info;
        }

        public int HoldCountWhenInstalledThatModule(ShipModule newModule)
        {
            ModuleSettingData moduleSetting;
            resource.ModuleSettings.TeyGetWorkshopData(newModule.Workshop, out moduleSetting);

            ShipModel testModel = new ShipModel(Resource());
            ShipModule prevModule = null;
            foreach(var slot in this.Slots)
            {
                if(slot.HasModule && slot.Type != newModule.SlotType)
                {
                    testModel.SetModule(slot.Module, out prevModule);
                }
            }
            testModel.SetModule(newModule, out prevModule);
            testModel.Update();
            int totalHoldCount = testModel.cargo;
            return totalHoldCount;
        }


        public ShipModelSlotBase ES {
            get {
                return es;
            }
        }

        public ShipModelSlotBase CB {
            get {
                return cb;
            }
        }

        public ShipModelSlotBase DF {
            get {
                return df;
            }
        }

        public ShipModelSlotBase CM {
            get {
                return cm;
            }
        }

        public ShipModelSlotBase DM {
            get {
                return dm;
            }
        }

        public ShipModelSlotBase[] Slots {
            get {
                return slots;
            }
        }

        public int[] skills {
            get {
                List<int> skills = new List<int>();
                foreach(var slot in Slots) {
                    if(slot.HasModule) {
                        skills.Add(slot.Module.Skill);
                    }
                }
                return skills.ToArray();
            }
        }


        /// <summary>
        /// set module
        /// </summary>
        /// <param name="module"></param>
        /// <param name="prevModule"></param>
        /// <returns></returns>
        public bool SetModule(ShipModule module, out ShipModule prevModule) {
            prevModule = null;
            if (module != null) {
                var slot = Slot(module.SlotType);
                if (slot != null) {
                    bool result =  slot.SetModule(module, out prevModule);
                    sets.UpdateSets(this.slots);
                    return result;
                }
            }
            return false;
        }

        public void Update()
        {
            if(sets != null )
            {
                sets.UpdateSets(this.slots);
            }
        }

        public bool RemoveModule(ShipModelSlotType type, out ShipModule prevModule) 
        {
            prevModule = null;
            if (Slot(type) != null) {
                Slot(type).RemoveModule(out prevModule);
            }
            sets.UpdateSets(this.slots);
            if (prevModule != null)
                return true;
            else
                return false;
        }

        public bool HasModule(ShipModelSlotType type) {
            if (Slot(type) != null) {
                return Slot(type).HasModule;
            }
            return false;
        }

        public ShipModule Module(ShipModelSlotType type) {
            if (Slot(type) != null)
            {
                return Slot(type).Module;
            }
            return null;
        }



        public Hashtable GetInfo()
        {
            Hashtable result = new Hashtable();
            foreach(var slot in this.slots)
            {
                result.Add(slot.Type.toByte(), slot.GetInfo());
            }
            return result;
        }

        public ShipModelSets Sets
        {
            get
            {
                return this.sets;
            }
        }

        public Dictionary<ShipModelSlotType, string> AsDictionary()
        {
            Dictionary<ShipModelSlotType, string> mDict = new Dictionary<ShipModelSlotType, string>
            {
                { ShipModelSlotType.CB, this.cb.HasModule ? this.cb.Module.TemplateModuleId : string.Empty },
                {ShipModelSlotType.CM, this.cm.HasModule ? this.cm.Module.TemplateModuleId : string.Empty },
                {ShipModelSlotType.DF, this.df.HasModule ? this.df.Module.TemplateModuleId : string.Empty },
                {ShipModelSlotType.DM, this.dm.HasModule ? this.dm.Module.TemplateModuleId : string.Empty },
                {ShipModelSlotType.ES, this.es.HasModule ? this.es.Module.TemplateModuleId : string.Empty }
            };
            return mDict;
        }

        #region MODEL PARAMETERS

        /// <summary>
        /// FINAL HP
        /// </summary>
        public float hp {
            get {
                float res = 0f;
                foreach(var slot in Slots) {
                    if(slot.HasModule) { res += slot.Module.HP; }
                }
                return res;
            }
        }

        /// <summary>
        /// FINAL CARGO
        /// </summary>
        /// <param name="bonus_aux_number"></param>
        /// <returns></returns>
        public int cargo {
            get {
                return (int)(cargoSum * CargoBonus(bonus_aux_number));
            }
        }

        /// <summary>
        /// FINAL SPEED
        /// </summary>
        /// <param name="bonus_aux_number"></param>
        /// <returns></returns>
        public float speed {
            get {
                return speedSum * SpeedBonus(bonus_aux_number);
            }
        }


        public float resistance {
            get {
                float sum = 0f;
                foreach (var s in Slots) {
                    if (s.HasModule) {
                        sum += s.Module.Resist;
                    }
                }
                return sum / (sum + resistance_aux_number);
            }
        }

        public float damageBonus {
            get {
                float sum = 0f;
                foreach (var s in Slots) {
                    if (s.HasModule) {
                        sum += s.Module.DamageBonus;
                    }
                }
                return (1f + sum / (sum + bonus_aux_number));
            }
        }

        public float energyBonus {
            get {
                float sum = 0f;
                foreach (var s in Slots) {
                    if (s.HasModule) {
                        sum += s.Module.EnergyBonus;
                    }
                }
                return (1.0f + sum / (sum + bonus_aux_number));
            }
        }

        public float critChance{
            get {
                float sum = 0f;
                foreach (var s in Slots) {
                    if (s.HasModule) {
                        sum += s.Module.CritChance;
                    }
                }
                return sum / (sum + critical_chance_aux_number);
            }
        }

        public float critDamageBonus {
            get {
                float sum = 0f;
                foreach(var s in Slots) {
                    if(s.HasModule) {
                        sum += s.Module.CritDamage;
                    }
                }
                return (2f + sum * 0.0001F);
            }
        }
        #endregion

        private int cargoSum {
            get {
                int res = 0;
                foreach(var s in Slots) {
                    if(s.HasModule) { res += s.Module.Hold; }
                }
                return res;
            }
        }

        private float CargoBonus(float bonus_aux_number) {
            float sum = 0f;
            foreach(var s in Slots) {
                if(s.HasModule) {
                    sum += s.Module.HoldBonus;
                }
            }
            return (1.0f + sum / (sum + bonus_aux_number));
        }

        private float speedSum {
            get {
                float sum = 0f;
                foreach(var s in Slots) {
                    if(s.HasModule) { sum += s.Module.Speed; }
                }
                return sum;
            }
        }

        private float SpeedBonus(float bonus_aux_number) {
            float sum = 0f;
            foreach(var s in Slots) {
                if(s.HasModule) {
                    sum += s.Module.SpeedBonus;
                }
            }
            return (1.0f + sum / (sum + bonus_aux_number));
        }


    }

    
}
