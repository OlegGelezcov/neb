using GameMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Space.Game.Resources
{
    public class ModuleSlotsCountLevelRangeEntry
    {
        private Range<int> levelRange;
        private Range<int> slotsRange;
        

        public ModuleSlotsCountLevelRangeEntry(int minLevel, int maxLevel, int minSlots, int maxSlots)
        {
            this.levelRange = new Range<int>(minLevel, maxLevel);
            this.slotsRange = new Range<int>(minSlots, maxSlots);
        }

        public Range<int> LevelRange()
        {
            return this.levelRange;
        }

        public Range<int> SlotsRange()
        {
            return this.slotsRange;
        }
    }
}
