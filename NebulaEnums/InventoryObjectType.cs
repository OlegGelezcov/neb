using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Common
{
    public enum InventoryObjectType : byte
    {
        Weapon,
        Scheme,
        Material,
        DrillScheme, //scheme for setting drill in space
        Module,
        None,
        personal_beacon,
        repair_kit,
        repair_patch,
        fort_upgrade,
        out_upgrade,
        turret,
        fortification,
        outpost,
        mining_station,
        nebula_element,
        //pass elenment
        pass
    }
}
