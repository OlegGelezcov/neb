using Common;
using Nebula.Engine;
using Nebula.Game.Components;
using Space.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Game {
    public class BroadcastChatMessageComposer {

        private class Colors {
            public static readonly string green = "green";
            public static readonly string yellow = "yellow";
            public static readonly string red = "red";
            public static readonly string lightblue = "lightblue";
            public static readonly string white = "white";
        }

        public string GetSetMiningStationMessage(PlayerCharacterObject player) {

            MmoWorld world = player.nebulaObject.mmoWorld();
            RaceableObject playerRaceable = player.GetComponent<RaceableObject>();
            string systemName = world.Resource().Zones.GetZoneName(world.Name);
            string playerName = player.characterName;
            string playerRaceName = world.Resource().Zones.GetRaceName(playerRaceable.getRace());
            string coalitionName = player.guildName;
            if (coalitionName == null) {
                coalitionName = string.Empty;
            }

            return string.Format("System {0}: Player {1}[{2}:{3}] set new mining station",
                systemName,
                ColorTag(playerName, RaceColor(playerRaceable.getRace() ) ),
                ColorTag(playerRaceName, RaceColor(playerRaceable.getRace())),
                ColorTag(coalitionName, RaceColor(playerRaceable.getRace()))
                );
        }

        public string GetSetFortificationMessage(PlayerCharacterObject player) {
            MmoWorld world = player.nebulaObject.mmoWorld();
            RaceableObject playerRaceable = player.GetComponent<RaceableObject>();
            string systemName = world.Resource().Zones.GetZoneName(world.Name);
            string playerName = player.characterName;
            string playerRaceName = world.Resource().Zones.GetRaceName(playerRaceable.getRace());
            string coalitionName = player.guildName;
            if (coalitionName == null) {
                coalitionName = string.Empty;
            }

            return string.Format("System {0}: Player {1}[{2}:{3}] set new fortification",
                systemName,
                ColorTag(playerName, RaceColor(playerRaceable.getRace())),
                ColorTag(playerRaceName, RaceColor(playerRaceable.getRace())),
                ColorTag(coalitionName, RaceColor(playerRaceable.getRace()))
                );
        }

        public string GetSetOutpostMessage(PlayerCharacterObject player) {
            MmoWorld world = player.nebulaObject.mmoWorld();
            RaceableObject playerRaceable = player.GetComponent<RaceableObject>();
            string systemName = world.Resource().Zones.GetZoneName(world.Name);
            string playerName = player.characterName;
            string playerRaceName = world.Resource().Zones.GetRaceName(playerRaceable.getRace());
            string coalitionName = player.guildName;
            if (coalitionName == null) {
                coalitionName = string.Empty;
            }

            return string.Format("System {0}: Player {1}[{2}:{3}] set new outpost",
                systemName,
                ColorTag(playerName, RaceColor(playerRaceable.getRace())),
                ColorTag(playerRaceName, RaceColor(playerRaceable.getRace())),
                ColorTag(coalitionName, RaceColor(playerRaceable.getRace()))
                );
        }

        public string GetStartAttackMessage(PlayerCharacterObject player, NebulaObject enemy) {

            MmoWorld world = player.nebulaObject.mmoWorld();
            RaceableObject playerRaceable = player.GetComponent<RaceableObject>();
            CharacterObject enemyCharacter = enemy.GetComponent<CharacterObject>();
            BotObject botObject = enemy.GetComponent<BotObject>();

            string systemName = world.Resource().Zones.GetZoneName(world.Name);
            string playerName = player.characterName;
            string playerRaceName = world.Resource().Zones.GetRaceName(playerRaceable.getRace());
            string npcName = GenerateStandardNpcName(systemName, enemy.Id);
            int npcLevel = enemyCharacter.level;

            string coalitionName = player.guildName;
            if (coalitionName == null) {
                coalitionName = string.Empty;
            }

            switch (botObject.getSubType()) {
                case BotItemSubType.Drill:
                    return string.Format("System {0}: Player {1}[{2}:{5}] start attack mining station {3}[lvl. {4}]",
                        systemName, ColorTag(playerName, RaceColor(playerRaceable.getRace())),
                        ColorTag(playerRaceName, RaceColor(playerRaceable.getRace())), npcName, npcLevel,
                        ColorTag(coalitionName, RaceColor(playerRaceable.getRace())));
                case BotItemSubType.Outpost:
                    return string.Format("System {0}: Player {1}[{2}:{5}] start attack fortification {3}[lvl. {4}]",
                        systemName, ColorTag(playerName, RaceColor(playerRaceable.getRace())),
                        ColorTag(playerRaceName, RaceColor(playerRaceable.getRace())), npcName, npcLevel,
                        ColorTag(coalitionName, RaceColor(playerRaceable.getRace())));
                case BotItemSubType.MainOutpost:
                    return string.Format("System {0}: Player {1}[{2}:{5}] start attack outpost {3}[lvl. {4}]",
                        systemName, ColorTag(playerName, RaceColor(playerRaceable.getRace())),
                        ColorTag(playerRaceName, RaceColor(playerRaceable.getRace())), npcName, npcLevel,
                        ColorTag(coalitionName, RaceColor(playerRaceable.getRace())));
                default:
                    return string.Empty;
            }
        }

        public string GetKillStandardNPCMessage(PlayerCharacterObject player, NebulaObject enemy) {
            MmoWorld world = player.nebulaObject.mmoWorld();
            RaceableObject playerRaceable = player.GetComponent<RaceableObject>();
            CharacterObject enemyCharacter = enemy.GetComponent<CharacterObject>();
            BotObject enemyBot = enemy.GetComponent<BotObject>();

            string systemName = world.Resource().Zones.GetZoneName(world.Name);
            string playerName = player.characterName;
            string playerRaceName = world.Resource().Zones.GetRaceName(playerRaceable.getRace());
            string npcName = GenerateStandardNpcName(systemName, enemy.Id);
            int npcLevel = enemyCharacter.level;

            string coalitionName = player.guildName;
            if(coalitionName == null ) {
                coalitionName = string.Empty;
            }
            switch(enemyBot.getSubType()) {
                case BotItemSubType.StandardCombatNpc:
                    return string.Format("System {0}: Player {1}[{2}:{5}] destroy special ship {3}[lvl. {4}]", 
                        systemName, ColorTag(playerName, RaceColor(playerRaceable.getRace())), 
                        ColorTag(playerRaceName, RaceColor(playerRaceable.getRace())), npcName, npcLevel, 
                        ColorTag(coalitionName, RaceColor(playerRaceable.getRace())));
                case BotItemSubType.Drill:
                    return string.Format("System {0}: Player {1}[{2}:{5}] destroy mining station {3}[lvl. {4}]", 
                        systemName, 
                        ColorTag(playerName, RaceColor(playerRaceable.getRace())),
                        ColorTag(playerRaceName, RaceColor(playerRaceable.getRace())), 
                        npcName, npcLevel, 
                        ColorTag(coalitionName, RaceColor(playerRaceable.getRace())));
                case BotItemSubType.Outpost:
                    return string.Format("System {0}: Player {1}[{2}:{5}] destroy fortification {3}[lvl. {4}]", 
                        systemName,
                        ColorTag(playerName, RaceColor(playerRaceable.getRace())),
                        ColorTag(playerRaceName, RaceColor(playerRaceable.getRace())), npcName, npcLevel,
                        ColorTag(coalitionName, RaceColor(playerRaceable.getRace())));
                case BotItemSubType.MainOutpost:
                    return string.Format("System {0}: Player {1}[{2}:{5}] destroy outpost {3}[lvl. {4}]", 
                        systemName,
                        ColorTag(playerName, RaceColor(playerRaceable.getRace())),
                        ColorTag(playerRaceName, RaceColor(playerRaceable.getRace())), npcName, npcLevel,
                        ColorTag(coalitionName, RaceColor(playerRaceable.getRace())));
                default:
                    return string.Empty;
            }
        }

        private string RaceColor(Race race) {
            switch(race) {
                case Race.Humans:
                    return Colors.green;
                case Race.Criptizoids:
                    return Colors.lightblue;
                case Race.Borguzands:
                    return Colors.red;
                default:
                    return Colors.white;
            }
        }
        private string ColorTag(string text, string color) {
            return string.Format("<color={0}>{1}</color>", color, text);
        }

       

        public string GetKillOtherPlayerMessage(PlayerCharacterObject player, NebulaObject enemy) {
            MmoWorld world = player.nebulaObject.mmoWorld();
            RaceableObject playerRaceable = player.GetComponent<RaceableObject>();

            PlayerCharacterObject enemyCharacter = enemy.GetComponent<PlayerCharacterObject>();
            RaceableObject enemyRaceable = enemy.GetComponent<RaceableObject>();

            string systemName = world.Resource().Zones.GetZoneName(world.Name);
            string playerName = player.characterName;
            string playerRaceName = world.Resource().Zones.GetRaceName(playerRaceable.getRace());

            string sourceCoalitionName = player.guildName;
            if(sourceCoalitionName == null ) {
                sourceCoalitionName = string.Empty;
            }

            string enemyName = enemyCharacter.characterName;
            string enemyRaceName = world.Resource().Zones.GetRaceName(enemyRaceable.getRace());

            string enemyCoalitionName = enemyCharacter.guildName;
            if(enemyCoalitionName == null ) {
                enemyCoalitionName = string.Empty;
            }

            return string.Format("System {0}: Player {1}[{2}:{5}] destroy ship of player {3}[{4}:{6}]", 
                systemName, 
                ColorTag(playerName, RaceColor(playerRaceable.getRace())), 
                ColorTag(playerRaceName, RaceColor(playerRaceable.getRace())), 
                ColorTag(enemyName, RaceColor(enemyRaceable.getRace())), 
                ColorTag(enemyRaceName, RaceColor(enemyRaceable.getRace())), 
                ColorTag(sourceCoalitionName, RaceColor(playerRaceable.getRace())), 
                ColorTag(enemyCoalitionName, RaceColor(enemyRaceable.getRace())));
        }


        /// <summary>
        /// Generate some name for bot from system name and id in form: 2symbols of sys name - 3 digits in id
        /// </summary>
        private string GenerateStandardNpcName(string systemName, string npcId ) {
            string sysPrefix = systemName.Substring(0, 2);
            string idSuffix = string.Empty;
            foreach(char c in npcId) {
                if(char.IsDigit(c)) {
                    idSuffix += c;
                    if(idSuffix.Length >= 3 ) {
                        break;
                    }
                }
            }
            return sysPrefix + "-" + idSuffix;
        }
    }
}
