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
            string playerName = player.characterName;
            string coalitionName = player.guildName;
            if (coalitionName == null) {
                coalitionName = string.Empty;
            }

            
            return string.Format("scm1:zone={0};chname={1};chrace={2};chcoal={3}",
                world.Name,
                playerName,
                playerRaceable.race,
                coalitionName
                );
        }

        public string GetSetFortificationMessage(PlayerCharacterObject player) {
            MmoWorld world = player.nebulaObject.mmoWorld();
            RaceableObject playerRaceable = player.GetComponent<RaceableObject>();
            //string systemName = world.Resource().Zones.GetZoneName(world.Name);
            string playerName = player.characterName;
            //string playerRaceName = world.Resource().Zones.GetRaceName(playerRaceable.getRace());
            string coalitionName = player.guildName;
            if (coalitionName == null) {
                coalitionName = string.Empty;
            }

            
            return string.Format("scm2:zone={0};chname={1};chrace={2};chcoal={3}",
                world.Name,
                playerName,
                playerRaceable.race,
                coalitionName
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

            
            return string.Format("scm3:zone={0};chname={1};chrace={2};chcoal={3}",
                systemName,
                playerName,
                playerRaceable.race,
                coalitionName
                );
        }

        public string GetStartAttackMessage(PlayerCharacterObject player, NebulaObject enemy) {

            MmoWorld world = player.nebulaObject.mmoWorld();
            RaceableObject playerRaceable = player.GetComponent<RaceableObject>();
            CharacterObject enemyCharacter = enemy.GetComponent<CharacterObject>();
            BotObject botObject = enemy.GetComponent<BotObject>();

            string systemName = world.Resource().Zones.GetZoneName(world.Name);
            string playerName = player.characterName;
            //string playerRaceName = world.Resource().Zones.GetRaceName(playerRaceable.getRace());
            string npcName = GenerateStandardNpcName(systemName, enemy.Id);
            int npcLevel = enemyCharacter.level;

            string coalitionName = player.guildName;
            if (coalitionName == null) {
                coalitionName = string.Empty;
            }


            switch (botObject.getSubType()) {
                case BotItemSubType.Drill:
                    return string.Format("scm4:zone={0};chname={1};chrace={2};chcoal={3};enname={4};enlvl={5}",
                        world.Name, playerName,
                        playerRaceable.race, coalitionName,
                        npcName, npcLevel);
                case BotItemSubType.Outpost:
                    return string.Format("scm5:zone={0};chname={1};chrace={2};chcoal={3};enname={4};enlvl={5}",
                        world.Name, playerName,
                        playerRaceable.race, coalitionName,
                        npcName, npcLevel);
                case BotItemSubType.MainOutpost:
                    return string.Format("scm6:zone={0};chname={1};chrace={2};chcoal={3};enname={4};enlvl={5}",
                        world.Name, playerName,
                        playerRaceable.race, coalitionName,
                        npcName, npcLevel);
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
                    return string.Format("scm7:zone={0};chname={1};chrace={2};chcoal={3};enname={4};enlvl={5}",
                        world.Name, playerName, playerRaceable.race, coalitionName, npcName, npcLevel);
                case BotItemSubType.Drill:
                    return string.Format("scm8:zone={0};chname={1};chrace={2};chcoal={3};enname={4};enlvl={5}",
                        world.Name, playerName, playerRaceable.race, coalitionName, npcName, npcLevel);
                case BotItemSubType.Outpost:
                    return string.Format("scm9:zone={0};chname={1};chrace={2};chcoal={3};enname={4};enlvl={5}",
                        world.Name, playerName, playerRaceable.race, coalitionName, npcName, npcLevel);
                case BotItemSubType.MainOutpost:
                    return string.Format("scm10:zone={0};chname={1};chrace={2};chcoal={3};enname={4};enlvl={5}",
                        world.Name, playerName, playerRaceable.race, coalitionName, npcName, npcLevel);
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

            return string.Format("scm11:zone={0};chname={1};chrace={2};chcoal={3};enname={4};enrace={5};encoal={6}",
                world.Name, playerName, playerRaceable.race, sourceCoalitionName,
                enemyName, enemyRaceable.race, enemyCoalitionName);
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
