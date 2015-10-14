using System;
using Common;

namespace Space.Game.Ship
{
    public class CM_ShipModelSlot : ShipModelSlotBase
    {
        public override ShipModelSlotType Type
        {
            get { return ShipModelSlotType.CM; }
        }
    }
}
