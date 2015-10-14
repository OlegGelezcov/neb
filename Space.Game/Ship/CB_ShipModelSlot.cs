using System;
using Common;

namespace Space.Game.Ship
{
    public class CB_ShipModelSlot : ShipModelSlotBase
    {
        public override ShipModelSlotType Type
        {
            get {
                return ShipModelSlotType.CB;
            }
        }
    }
}
