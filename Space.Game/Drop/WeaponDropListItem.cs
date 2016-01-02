using Common;
using Space.Game;
using Space.Game.Drop;
using Space.Game.Inventory;
using Space.Game.Inventory.Objects;
using Space.Game.Resources;
using System;
using System.Xml.Linq;

namespace Nebula.Drop {
    public class WeaponDropListItem : DropListItem {

        public WeaponDropListItem(InventoryObjectType category, string colorList) :
            base(category, colorList) { }

        public WeaponDropListItem(XElement element) : base(element) { }

        public override ServerInventoryItem Roll(IRes resource, int level, Workshop workshop) {
            DropManager dropManager = DropManager.Get(resource);
            var cl = resource.colorLists.GetList(colorList);
            ObjectColor color = cl.Roll();

            WeaponDropper.WeaponDropParams dropParams = new WeaponDropper.WeaponDropParams(
                resource,
                level,
                workshop,
                WeaponDamageType.damage,
                Difficulty.none
            );
            ColorInfo colorInfo = resource.ColorRes.Color(ColoredObjectType.Weapon, color);

            WeaponDropper weaponDropper = dropManager.GetWeaponDropper(dropParams);
            WeaponObject weapon = weaponDropper.DropWeapon(colorInfo);
            return new ServerInventoryItem(weapon, 1);
        }
    }
}
