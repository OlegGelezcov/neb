using System;
using Common;

namespace Space.Game.Ship
{
    public class DF_ShipModelSlot : ShipModelSlotBase
    {
        public override ShipModelSlotType Type
        {
            get { return ShipModelSlotType.DF; }
        }
    }
}
