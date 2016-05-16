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

        public static Workshop GetWorkshopForRaceAndClass(Race race, NpcClass npcClass ) {
            switch(race) {
                case Race.Humans: {
                        switch(npcClass) {
                            case NpcClass.rdd: {
                                    return Workshop.DarthTribe;
                                }
                            case NpcClass.healer: {
                                    return Workshop.Equilibrium;
                                }
                            case NpcClass.sdd: {
                                    return Workshop.BigBang;
                                }
                            case NpcClass.tank: {
                                    return Workshop.RedEye;
                                }
                        }
                    }
                    break;
                case Race.Borguzands: {
                        switch(npcClass) {
                            case NpcClass.rdd: {
                                    return Workshop.Phelpars;
                                }
                            case NpcClass.healer: {
                                    return Workshop.Zoards;
                                }
                            case NpcClass.sdd: {
                                    return Workshop.Rakhgals;
                                }
                            case NpcClass.tank: {
                                    return Workshop.Lerjees;
                                }
                        }
                    }
                    break;
                case Race.Criptizoids: {
                        switch(npcClass) {
                            case NpcClass.rdd: {
                                    return Workshop.Yshuar;
                                }
                            case NpcClass.healer: {
                                    return Workshop.Arlen;
                                }
                            case NpcClass.sdd: {
                                    return Workshop.Dyneira;
                                }
                            case NpcClass.tank: {
                                    return Workshop.KrolRo;
                                }
                        }
                    }
                    break;
            }
            return Workshop.Arlen;
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
