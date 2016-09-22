using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Nebula.Server.Components {
    public class SpecialShipWeaponComponentData : ShipWeaponComponentData {

        public WeaponGenList gen { get; private set; }

        public SpecialShipWeaponComponentData(XElement element) : base(element) {
            gen = new WeaponGenList {
                id = element.GetString("wid"),
                template = element.GetString("template"),
                level = element.GetInt("level"),
                optimalDistance = element.GetFloat("od"),
                color = (ObjectColor)Enum.Parse(typeof(ObjectColor), element.GetString("color")),
                critChance = element.GetFloat("crit_chance"),
                workshop = (Workshop)Enum.Parse(typeof(Workshop), element.GetString("workshop")),
                rocketDamage = element.GetFloat("rocket_dmg"),
                acidDamage = element.GetFloat("acid_dmg"),
                laserDamage = element.GetFloat("laser_dmg"),
                baseType = (WeaponBaseType)Enum.Parse(typeof(WeaponBaseType), element.GetString("base_type"))
            };
        }

        public override ComponentSubType subType {
            get {
                return ComponentSubType.special_ship_weapon;
            }
        }
    }
}
