using System;
using System.Collections.Generic;
using Common;
using System.Text;
using System.Linq;

namespace Space.Game.Ship
{
    /// <summary>
    /// Describe all set in ship
    /// </summary>
    public class SetInfo
    {
        /// <summary>
        /// Bonus factor for item from set
        /// </summary>
        public class BonusFactor
        {
            public SetBonusType BonusType { get; set; }
            public float Factor { get; set; }
        }
        public string Id { get; set; }
        public string Name { get; set; }
        public int UnlockLevel { get; set; }
        public string Skill { get; set; }

        public Dictionary<int, BonusFactor> Bonuses { get; set; }

        public BonusFactor BonusFor(int count)
        {
            switch (count)
            {
                case 1:
                    return Bonuses[1];
                case 2:
                    return Bonuses[2];
                case 4:
                    return Bonuses[4];
                default:
                    throw new ArgumentException(string.Format("invalid key: {0}, allowed[1, 2, 4]", count));
            }
        }
    }
}
