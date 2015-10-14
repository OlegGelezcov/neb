namespace Nebula.Resources {
    using Common;
    using ServerClientCommon;
    using UnityEngine;

    public static class ColorCache {

        public static Color GetGuildStatusColor(GuildMemberStatus status) {
            switch (status) {
                case GuildMemberStatus.Member:
                    return Color.white;
                case GuildMemberStatus.Moderator:
                    return Color.green;
                case GuildMemberStatus.Owner:
                    return Utils.RGB(253, 150, 28);
            }
            return Color.white;
        }

        public static Color RaceColor(Race race) {
            switch (race) {
                case Race.Humans:
                    return Color.green;
                case Race.Borguzands:
                    return Color.red;
                case Race.Criptizoids:
                    return Utils.RGB(15, 169, 219);
                default:
                    return Color.white;
            }
        }

        public static Color RaceCommandStatusColor(int commandStatus) {
            switch (commandStatus) {
                case RaceCommandKey.COMMANDER:
                    return Utils.RGB(253, 150, 28);
                case RaceCommandKey.ADMIRAL1:
                case RaceCommandKey.ADMIRAL2:
                    return Color.green;
                default:
                    return Color.white;
            }
        }
    }
}
