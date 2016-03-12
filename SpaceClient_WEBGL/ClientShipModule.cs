using Common;
using ServerClientCommon;
using System.Collections.Generic;
using Nebula.Client.Inventory;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;

namespace Nebula.Client {
    public class ClientShipModule : IInventoryObjectInfo {
        public string id;
        public ShipModelSlotType type;
        public int level;
        public string name;
        public Workshop workshop;
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

        public bool isNew {
            get;
            private set;
        }

        public bool IsBelongToSet {
            get {
                return (false == string.IsNullOrEmpty(set));
            }
        }

        public bool HasPrefab() {
            return (false == string.IsNullOrEmpty(prefab));
        }

        public string Id {
            get { return this.id; }
        }

        public InventoryObjectType Type {
            get { return InventoryObjectType.Module; }
        }

        public Hashtable GetInfo() {
            return mRaw;
        }


        public void ParseInfo(Hashtable info) {
            mRaw = info;

            id = info.GetValueString((int)SPC.Id);                                                     //1
            type = info.GetValueByte((int)SPC.Type, (byte)ShipModelSlotType.CB).toEnum<ShipModelSlotType>();         //2
            level = info.GetValueInt((int)SPC.Level, 0);                                                             //3
            name = info.GetValueString((int)SPC.Name);                                                 //4
            workshop = info.GetValueByte((int)SPC.Workshop, (byte)Workshop.DarthTribe).toEnum<Workshop>();           //5
            set = info.GetValueString((int)SPC.Set);                                                   //6
            skill = info.GetValueInt((int)SPC.Skill, -1);                                               //8
            hp = info.GetValueFloat((int)SPC.Health);                                                              //9
            speed = info.GetValueFloat((int)SPC.Speed);                                                        //10
            resist = info.GetValueFloat((int)SPC.Resist);                                                      //11
            hold = info.GetValueInt((int)SPC.Hold);                                                               //12
            damageBonus = info.GetValueFloat((int)SPC.DamageBonus);                                           //13
            color = info.GetValueByte((int)SPC.Color, (byte)ObjectColor.white).toEnum<ObjectColor>();                //16
            templateId = info.GetValueString((int)SPC.Template);                                     //17
            prefab = info.GetValueString((int)SPC.Prefab);                                             //18
            critChance = info.GetValueFloat((int)SPC.CritChance);                                             //19
            critDamage = info.GetValueFloat((int)SPC.CritDamage);                                             //20
            energyBonus = info.GetValueFloat((int)SPC.EnergyBonus);
            speedBonus = info.GetValueFloat((int)SPC.SpeedBonus);
            holdBonus = info.GetValueFloat((int)SPC.HoldBonus);

            Hashtable craftMaterialsHash = info.GetValueHash((int)SPC.CraftMaterials);
            this.craftMaterials = craftMaterialsHash.toDict<string, int>();                                                     //23
            binded = info.GetValue<bool>((int)SPC.Binded, false);
            isNew = info.GetValueBool((int)SPC.IsNew);
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
        public bool HasSkill {
            get {
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
