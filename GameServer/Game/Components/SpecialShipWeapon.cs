using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using Nebula.Server.Components;
using Space.Game.Inventory.Objects;
using ExitGames.Logging;

namespace Nebula.Game.Components {
    public class SpecialShipWeapon : ShipWeapon {

        private static readonly ILogger s_Log = LogManager.GetCurrentClassLogger();

        protected override void GenerateWeaponObject(Difficulty difficulty) {
            if(componentData != null && (componentData is SpecialShipWeaponComponentData)) {
                SpecialShipWeaponComponentData cdata = componentData as SpecialShipWeaponComponentData;
                WeaponObject wObj = new WeaponObject(cdata.gen);
                SetWeapon(wObj);
                s_Log.InfoFormat("generate weapon object special".Purple());

            } else {
                base.GenerateWeaponObject(difficulty);
            }
        }
    }
}
