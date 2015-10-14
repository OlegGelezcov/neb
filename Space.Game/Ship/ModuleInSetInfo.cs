using System;
using Common;

namespace Space.Game.Ship
{
    /// <summary>
    /// Info for module in set
    /// </summary>
    public class ModuleInSetInfo
    {
        private readonly ShipModelSlotType type;
        private readonly int slotsCount;

        public ModuleInSetInfo(ShipModelSlotType type, int slotsCount) {
            this.type = type;
            this.slotsCount = slotsCount;
        }

        public ShipModelSlotType Type {
            get {
                return type;
            }
        }

        public int SlotsCount {
            get {
                return slotsCount;
            }
        }

        public override string ToString()
        {
            return string.Format("type: {0}, slots count: {1}", type, slotsCount);
        }
    }
}
