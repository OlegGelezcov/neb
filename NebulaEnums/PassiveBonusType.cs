using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common {
    public enum PassiveBonusType : int {
        Speed = 1,
        DamageDron = 2,
        HealDron = 3,
        CritChance = 4,
        CritDamage = 5,
        Damage = 6,
        Resist = 7,
        MaxHP = 8,
        MaxEnergy = 9,
        ColoredLoot = 10,
        RestoreHPSpeed = 11,
        RestoreEnergy = 12,
        OptimalDistance = 13,
        ChanceNotDropLootAtDeath = 14,
        ChanceCraftColoredModule = 15
    }
}
