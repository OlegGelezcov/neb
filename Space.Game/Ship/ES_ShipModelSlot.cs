using System;
using Common;

namespace Space.Game.Ship
{
    public class ES_ShipModelSlot : ShipModelSlotBase
    {
        public override Common.ShipModelSlotType Type
        {
            get {
                return Common.ShipModelSlotType.ES;
            }
        }
    }
}
