using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;

namespace Space.Game.Ship
{
    /// <summary>
    /// hold info about sets on ship
    /// </summary>
    public class ShipModelSets
    {
        private IRes resource;
        private int skill;

        public int Skill
        {
            get
            {
                return skill;
            }
        }

        private void ClearBonuses()
        {
            this.skill = -1;
        }

        public ShipModelSets(IRes resource)
        {
            SetResource(resource);
        }

        public void SetResource(IRes resource) {
            this.resource = resource;
            this.ClearBonuses();
        }

        private IRes Resource()
        {
            return this.resource;
        }

        private string GetSetOnModules(ShipModelSlotBase[] slots) {
            string set0 = string.Empty;
            if(slots[0].HasModule) {
                set0 = slots[0].Module.Set;
            }
            if (string.IsNullOrEmpty(set0)) {
                return string.Empty;
            }

            int setCount = 1;
            for(int i = 1; i < slots.Length; i++) {
                if(slots[i].HasModule) {
                    if(set0 == slots[i].Module.Set) {
                        setCount++;
                    }
                }
            }

            if(setCount == 5 ) {
                return set0;
            } else {
                return string.Empty;
            }
        }

        /// <summary>
        /// update sets when slots changed
        /// </summary>
        public void UpdateSets(ShipModelSlotBase[] slots)
        {
            Dictionary<string, int> setsOnShip = new Dictionary<string, int>();
            this.ClearBonuses();

            string setID = GetSetOnModules(slots);
            if(string.IsNullOrEmpty(setID)) {
                skill = -1;
            } else {
                var setData = Resource().Sets.Set(setID);
                if(setData == null ) {
                    skill = -1;
                } else {
                    skill = setData.Skill;
                }
            }
        }
    }
}
