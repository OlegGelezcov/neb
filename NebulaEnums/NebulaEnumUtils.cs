using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common {
    public static class NebulaEnumUtils {
        public static NpcClass GetNpcClassForWorkshop(Workshop workshop) {
            switch(workshop) {
                case Workshop.DarthTribe:
                case Workshop.Phelpars:
                case Workshop.Yshuar:
                    return NpcClass.rdd;
                case Workshop.RedEye:
                case Workshop.Lerjees:
                case Workshop.KrolRo:
                    return NpcClass.tank;
                case Workshop.Equilibrium:
                case Workshop.Zoards:
                case Workshop.Arlen:
                    return NpcClass.healer;
                case Workshop.BigBang:
                case Workshop.Rakhgals:
                case Workshop.Dyneira:
                    return NpcClass.sdd;
                default:
                    return NpcClass.none;
            }
        }

        public static ObjectColor GetColorForDifficulty(Difficulty difficulty) {
            switch(difficulty) {
                case Difficulty.easy:
                case Difficulty.easy2:
                case Difficulty.starter:
                    return ObjectColor.white;
                case Difficulty.medium:
                case Difficulty.none:
                    return ObjectColor.blue;
                case Difficulty.hard:
                    return ObjectColor.yellow;
                case Difficulty.boss:
                    return ObjectColor.green;
                case Difficulty.boss2:
                    return ObjectColor.orange;
                default:
                    return ObjectColor.white;
            }
        }
    }
}
