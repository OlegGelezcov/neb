
using Common;
using ServerClientCommon;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;

namespace Nebula.Client.Inventory.Objects {
    public class WeaponInventoryObjectInfo : IInventoryObjectInfo, ILeveledObjectInfo, IWorkshopObjectInfo {
        private string id;
        private string template;
        private int level;
        private ObjectColor color;
        private WeaponDamageType damageType;

        public float damage { get; private set; }
        public float baseCritChance { get; private set; }
        private float optimalDistance;
        public Workshop workshop { get; private set; }
        public bool binded { get; private set; }

        public float rocketDamage { get; private set; }
        public float laserDamage { get; private set; }
        public float acidDamage { get; private set; }
        public WeaponBaseType baseType { get; private set; }



        public Hashtable GetInfo() {
            return rawHash;
        }

        public bool isNew {
            get;
            private set;
        }


        public void ParseInfo(Hashtable info) {
            rawHash = info;
            this.id = info.GetValueString((int)SPC.Id);
            this.template = info.GetValueString((int)SPC.Template);
            this.level = info.GetValueInt((int)SPC.Level);
            this.damage = info.GetValueFloat((int)SPC.Damage);

            this.optimalDistance = info.GetValueFloat((int)SPC.OptimalDistance);

            this.color = (ObjectColor)(byte)info.GetValueInt((int)SPC.Color, (int)(byte)ObjectColor.white);
            this.damageType = (WeaponDamageType)(byte)info.GetValueByte((int)SPC.DamageType);
            this.baseCritChance = (float)info.GetValueFloat((int)SPC.CritChance);
            workshop = (Workshop)(byte)(int)info.GetValueInt((int)SPC.Workshop, (int)Workshop.DarthTribe);
            binded = info.GetValueBool((int)SPC.Binded, binded);
            isNew = info.GetValueBool((int)SPC.IsNew);

            rocketDamage = info.GetValueFloat((int)SPC.RocketDamage);
            laserDamage = info.GetValueFloat((int)SPC.LaserDamage);
            acidDamage = info.GetValueFloat((int)SPC.AcidDamage);

            baseType = (WeaponBaseType)info.GetValueInt((int)SPC.WeaponBaseType, (int)WeaponBaseType.Rocket);
        }

        public WeaponInventoryObjectInfo() { }

        public WeaponInventoryObjectInfo(Hashtable info) {
            ParseInfo(info);
        }

        public Workshop Workshop {
            get {
                return workshop;
            }
        }

        public string Id {
            get {
                return this.id;
            }
        }

        public InventoryObjectType Type {
            get {
                return InventoryObjectType.Weapon;
            }
        }

        public string Template {
            get {
                return this.template;
            }
        }

        public int Level {
            get {
                return this.level;
            }
        }


        public float OptimalDistance {
            get {
                return this.optimalDistance;
            }
        }


        public ObjectColor Color {
            get {
                return this.color;
            }
        }

        public WeaponDamageType DamagetType {
            get {
                return damageType;
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

        public ObjectColor MyColor() {
            return this.Color;
        }

        public override string ToString() {
            return string.Format("id: {0}", id.Substring(0, 3));
        }

        public bool HasColor() {
            return true;
        }
    }
}
