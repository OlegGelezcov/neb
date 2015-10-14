using Common;
using GameMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Space.Game.Resources
{
    public class ModuleTypeSlotLevelRange
    {
        private ShipModelSlotType slotType;
        private List<ModuleSlotsCountLevelRangeEntry> rangeEntries;

        public ModuleTypeSlotLevelRange(ShipModelSlotType type, List<ModuleSlotsCountLevelRangeEntry> rangeEntries)
        {
            this.slotType = type;
            this.rangeEntries = rangeEntries;
        }

        public Range<int> SlotRange(int level)
        {
            foreach(var entry in this.rangeEntries )
            {
                if(entry.LevelRange().InRange(level))
                {
                    return entry.SlotsRange();
                }
            }
            return null;
        }
    }
}
