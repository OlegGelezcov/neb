using System;
using Common;

namespace Space.Game.Ship
{
    public class DM_ShipModelSlot : ShipModelSlotBase
    {
        public override ShipModelSlotType Type
        {
            get { return ShipModelSlotType.DM; }
        }
    }
}
