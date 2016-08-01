using Common;
using ExitGames.Logging;
using GameMath;
using Nebula.Engine;
using Nebula.Game.Components;
using NebulaCommon.Group;
using Space.Game;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Game {
    public class DamagerCollection {
        private static readonly ILogger s_Log = LogManager.GetCurrentClassLogger();
        private readonly ConcurrentDictionary<string, DamageInfo> m_Damagers = new ConcurrentDictionary<string, DamageInfo>();

        public int count {
            get {
                return m_Damagers.Count;
            }
        }

        public int playerDamagerCount {
            get {
                int counter = 0;
                foreach(var kvp in m_Damagers) {
                    if(kvp.Value.DamagerType == Common.ItemType.Avatar) {
                        counter++;
                    }
                }
                return counter;
            }
        }

        public ConcurrentDictionary<string, DamageInfo> damagers {
            get {
                return m_Damagers;
            }
        }

        public DamageInfo Add(string damagerId, byte damagerType, float damage, byte workshop, int level, byte race) {
            if(m_Damagers.ContainsKey(damagerId)) {
                DamageInfo existingInfo;
                if(m_Damagers.TryGetValue(damagerId, out existingInfo)) {
                    existingInfo.AddDamage(damage);
                    return existingInfo;
                }
            } else {
                if(race != (byte)Race.None && damagerType == (byte)ItemType.Avatar ) {
                    var damageInfo = new DamageInfo(damagerId, damagerType, damage, workshop, level, race);
                    if(m_Damagers.TryAdd(damagerId, damageInfo)) {
                        return damageInfo;
                    }
                }
            }
            return null;
        }

        public bool Has(string itemId ) {
            return m_Damagers.ContainsKey(itemId);
        }

        public void Clear() {
            m_Damagers.Clear();
        }

        public void OnOwnerKilled(NebulaObject owner) {

            float difficulty = GetOwnerDifficulty(owner);
            float npcLevel = GetLevel(owner);
            float npcMaxHealth = MaxHealth(owner);

            List<NebulaObject> playerGroups = new List<NebulaObject>();

            foreach(var kvp in m_Damagers ) {
                if(kvp.Value.DamagerType == Common.ItemType.Avatar ) {
                    NebulaObject nebObject;
                    if(owner.world.TryGetObject((byte)kvp.Value.DamagerType, kvp.Value.DamagerId, out nebObject)) {
                        if (false == AddToGroup(nebObject, playerGroups)) {
                            GiveExpAndPvpPointsToPlayer(nebObject, owner, difficulty, npcLevel, npcMaxHealth);
                        }
                    }
                }
            }

            if(playerGroups.Count > 0 ) {
                foreach(NebulaObject pgo in playerGroups) {
                    GiveExpAndPvpPointsToPlayer(pgo, owner, difficulty, npcLevel, npcMaxHealth);
                }
            }
        }

        private bool AddToGroup(NebulaObject obj, List<NebulaObject> groupObjects) {
            var character = obj.GetComponent<PlayerCharacterObject>();
            if(character != null && character.hasGroup ) {
                if(false == ContainsGroupObject(groupObjects, obj)) {
                    groupObjects.Add(obj);
                }

                foreach(var member in character.group.members) {
                    NebulaObject go;
                    if(obj.mmoWorld().TryGetObject((byte)ItemType.Avatar, member.Value.gameRefID, out go)) {
                        if(false == ContainsGroupObject(groupObjects, go)) {
                            groupObjects.Add(go);
                        }
                    }
                }
                return true;
            }
            return false;
        }

        private bool ContainsGroupObject(List<NebulaObject> groupList, NebulaObject source) {
            foreach(var obj in groupList) {
                if(obj.Id == source.Id) {
                    return true;
                }
            }
            return false;
        }

        private void GiveExpAndPvpPointsToPlayer(NebulaObject playerObject, NebulaObject owner, float difficulty, float npcLevel, float npcMaxHealth ) {
            int baseExp = 20;
            float playerLevel = GetLevel(playerObject);
            float levelRat = npcLevel / playerLevel;
            float bexp = (difficulty * levelRat * (baseExp));
            float hpExp = difficulty * Mathf.ClampLess(npcMaxHealth - 1000, 0f) * 0.01f;
            if (levelRat < 1.0f) {
                hpExp *= levelRat;
            }
            int exp = (int)Math.Round(bexp + hpExp);
            s_Log.InfoFormat("sended exp = {0} for bot difficulty = {1}, hp exp bonus = {2}", exp, difficulty, (int)(difficulty * Mathf.ClampLess(npcMaxHealth - 1000, 0f) * 0.01f));
            playerObject.GetComponent<PlayerCharacterObject>().AddExp(exp);
            playerObject.SendMessage(ComponentMessages.OnEnemyDeath, owner);

            GivePvpPoints(playerObject, owner, npcLevel, (int)playerLevel);
        }

        private const int PVP_POINTS_FOR_TURRET = 5;
        private const int PVP_POINTS_FOR_FORTIFICATION = 10;
        private const int PVP_POINTS_FOR_MAINOUTPOST = 20;
        private const int PVP_POINTS_FOR_PLANET_BUILDING = 10;


        public void GivePvpPoints(NebulaObject playerObject, NebulaObject owner, float npcLevel, int playerLevel) {
            if (owner.Type == (byte)ItemType.Avatar) {
                int meLevel = (int)npcLevel;
                if (playerLevel <= (meLevel - 5)) {
                    GivePvpPointsToObject(playerObject, 10);
                } else if ((playerLevel > (meLevel - 5) && (playerLevel <= (meLevel + 5)))) {
                    GivePvpPointsToObject(playerObject, 5);
                }
            } else if (owner.Type == (byte)ItemType.Bot) {
                var bot = owner.GetComponent<BotObject>();
                if (bot != null) {
                    int points = 0;
                    switch ((BotItemSubType)bot.botSubType) {
                        case BotItemSubType.Turret:
                            points = PVP_POINTS_FOR_TURRET;
                            break;
                        case BotItemSubType.Outpost:
                            points = PVP_POINTS_FOR_FORTIFICATION;
                            break;
                        case BotItemSubType.MainOutpost:
                            points = PVP_POINTS_FOR_MAINOUTPOST;
                            break;
                        case BotItemSubType.PlanetBuilding:
                            points = PVP_POINTS_FOR_MAINOUTPOST;
                            break;
                    }

                    if (points > 0) {
                        GivePvpPointsToObject(playerObject, points);
                    }
                }
            }
        }

        private void GivePvpPointsToObject(NebulaObject playerObject, int points) {
            var playerCharacter = playerObject.GetComponent<PlayerCharacterObject>();
            if(playerCharacter != null ) {
                playerCharacter.AddPvpPoints(points);
            }
        }

        private float MaxHealth(NebulaObject obj) {
            var dmg = obj.Damagable();
            if(dmg != null ) {
                return dmg.maximumHealth;
            }
            return 0;
        }

        public float GetLevel(NebulaObject owner) {
            int level = 1;
            var character = owner.Character();
            if(character != null ) {
                level = character.level;
            }
            return level;
        }

        private float GetOwnerDifficulty(NebulaObject owner) {
            float d = 1.0f;
            BotShip botShip = owner.GetComponent<BotShip>();
            if(botShip != null ) {
                if(owner.HasTag((byte)PS.Difficulty)) {
                    d = owner.resource.GetDifficultyMult((Difficulty)(byte)owner.Tag((byte)PS.Difficulty));
                }
            }
            return d;
        }
    }
}
