using Common;
using ServerClientCommon;
using System.Collections;
using System.Collections.Generic;
using System;
using Nebula.Client.Inventory;

namespace Nebula.Client {
    public class ClientShipModule : IInventoryObjectInfo
    {
        public  string id;
        public  ShipModelSlotType type;
        public  int level;
        public  string name;
        public  Workshop workshop;
        public ObjectColor color;
        public string templateId;

        public float hp;
        public float speed;
        public int hold;

        public float resist;
        public float damageBonus;
        public float energyBonus;
        public float critChance;
        public float critDamage;
        public float speedBonus;
        public float holdBonus;

        public Dictionary<string, int> craftMaterials;
        public string prefab;

        public int skill;
        public string set;

        private Hashtable mRaw;
        public bool binded { get; private set; }


        public ClientShipModule(Hashtable info) {

            this.ParseInfo(info);


        }

        public bool IsBelongToSet
        {
            get
            {
                return (false == string.IsNullOrEmpty(set));
            }
        }

        public bool HasPrefab()
        {
            return (false == string.IsNullOrEmpty(prefab));
        }

        public string Id
        {
            get { return this.id; }
        }

        public InventoryObjectType Type
        {
            get { return InventoryObjectType.Module; }
        }

        public Hashtable GetInfo()
        {
            return mRaw;
        }


        public void ParseInfo(Hashtable info)
        {
            mRaw = info;

            id = info.GetValue<string>((int)SPC.Id, string.Empty);                                                     //1
            type = info.GetValue<byte>((int)SPC.Type, (byte)ShipModelSlotType.CB).toEnum<ShipModelSlotType>();         //2
            level = info.GetValue<int>((int)SPC.Level, 0);                                                             //3
            name = info.GetValue<string>((int)SPC.Name, string.Empty);                                                 //4
            workshop = info.GetValue<byte>((int)SPC.Workshop, (byte)Workshop.DarthTribe).toEnum<Workshop>();           //5
            set = info.GetValue<string>((int)SPC.Set, string.Empty);                                                   //6
            skill = info.GetValue<int>((int)SPC.Skill, -1);                                               //8
            hp = info.GetValue<float>((int)SPC.Health, 0.0f);                                                              //9
            speed = info.GetValue<float>((int)SPC.Speed, 0.0f);                                                        //10
            resist = info.GetValue<float>((int)SPC.Resist, 0.0f);                                                      //11
            hold = info.GetValue<int>((int)SPC.Hold, 0);                                                               //12
            damageBonus = info.GetValue<float>((int)SPC.DamageBonus, 0.0f);                                           //13
            color = info.GetValue<byte>((int)SPC.Color, (byte)ObjectColor.white).toEnum<ObjectColor>();                //16
            templateId = info.GetValue<string>((int)SPC.Template, string.Empty);                                     //17
            prefab = info.GetValue<string>((int)SPC.Prefab, string.Empty);                                             //18
            critChance = info.GetValue<float>((int)SPC.CritChance, 0.0f);                                             //19
            critDamage = info.GetValue<float>((int)SPC.CritDamage, 0.0f);                                             //20
            energyBonus = info.GetValue<float>((int)SPC.EnergyBonus, 0f);
            speedBonus = info.GetValue<float>((int)SPC.SpeedBonus, 0f);
            holdBonus = info.GetValue<float>((int)SPC.HoldBonus, 0f);

            Hashtable craftMaterialsHash = info.GetValue<Hashtable>((int)SPC.CraftMaterials, new Hashtable());
            this.craftMaterials = craftMaterialsHash.toDict<string, int>();                                                     //23
            binded = info.GetValue<bool>((int)SPC.Binded, false);
        }

        public bool HasColor() {
            return true;
        }

        public ObjectColor MyColor() {
            return color;
        }


        /// <summary>
        /// Has or no skill on this module
        /// </summary>
        public bool HasSkill
        {
            get
            {
                return (skill != -1);
            }
        }

        public int placingType {
            get {
                return (int)PlacingType.Station;
            }
        }

        public Hashtable rawHash {
            get {
                return mRaw;
            }
        }
    }
}
