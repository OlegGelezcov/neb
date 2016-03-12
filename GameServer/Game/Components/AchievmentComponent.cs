using Common;
using ExitGames.Logging;
using GameMath;
using Nebula.Database;
using Nebula.Engine;
using Nebula.Game.Utils;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System;
using ServerClientCommon;
using Nebula.Drop;

namespace Nebula.Game.Components {

    public class AchievmentComponent : NebulaBehaviour, IInfoSource {
        private static readonly ILogger s_Log = LogManager.GetCurrentClassLogger();
        private readonly ConcurrentDictionary<string, int> m_Variables = new ConcurrentDictionary<string, int>();
        private int m_Points;

        private bool m_Loaded = false;
        private MmoMessageComponent m_Message;
        private RaceableObject m_Raceable;

        private Vector3 m_PrevPos = Vector3.Zero;
        private float m_Distance = 0.0f;
        private readonly List<string> m_VisitedZones = new List<string>();

        /// <summary>
        /// all founded in space lore records
        /// </summary>
        private List<string> m_FoundedLoreRecords = new List<string>();

        public override int behaviourId {
            get {
                return (int)ComponentID.Achievments;
            }
        }

        private int points {
            get {
                return m_Points;
            }
        }

        public bool FoundLoreRecord(string id) {
            if (!string.IsNullOrEmpty(id)) {
                if (!m_FoundedLoreRecords.Contains(id)) {
                    m_FoundedLoreRecords.Add(id);
                    if (m_Message != null) {
                        m_Message.FoundLoreRecord(id);
                    }
                    return true;
                }
            }
            return false;
        }

        public override void Start() {
            base.Start();
            m_Message = GetComponent<MmoMessageComponent>();
            m_Raceable = GetComponent<RaceableObject>();
            OnZoneVisited(nebulaObject.mmoWorld().GetID());
        }

        public override void Update(float deltaTime) {
            base.Update(deltaTime);

            if(false == m_PrevPos.isZero) {
                m_Distance += (transform.position - m_PrevPos).magnitude;
                if(m_Distance >= 1000.0f ) {
                    AddVariable("total_distance", (int)m_Distance);
                    m_Distance = 0.0f;
                }
            }
            m_PrevPos = transform.position;
        }

        public void Load() {
            m_Variables.Clear();
            bool isNew = false;
            var character = GetComponent<PlayerCharacterObject>();
            var app = nebulaObject.mmoWorld().application;
            AchievmentSave save = AchievmentDatabase.instance(app).LoadAchievments(character.characterId, out isNew);
            if(save.variables != null ) {
                foreach(DictionaryEntry entry in save.variables) {
                    m_Variables.TryAdd((string)entry.Key, (int)entry.Value);
                }
            }

            m_VisitedZones.Clear();
            if(save.visitedZones != null ) {
                foreach(var vzone in save.visitedZones) {
                    m_VisitedZones.Add(vzone);
                }
            }

            m_Points = save.points;

            m_FoundedLoreRecords = save.loreRecords;
            if(m_FoundedLoreRecords == null ) {
                m_FoundedLoreRecords = new List<string>();
            }

            m_Loaded = true;
        }

        private bool loaded {
            get {
                return m_Loaded;
            }
        }

        public AchievmentSave GetSave() {
            Hashtable hash = GetInfo();
            return new AchievmentSave(hash[(int)SPC.Variables] as Hashtable, m_VisitedZones, points, m_FoundedLoreRecords);
        }

        public Hashtable GetInfo() {
            Hashtable hash = new Hashtable();
            foreach (var kvp in m_Variables) {
                hash.Add(kvp.Key, kvp.Value);
            }

            if(m_FoundedLoreRecords == null ) {
                m_FoundedLoreRecords = new List<string>();
            }

            Hashtable info = new Hashtable {
                {(int)SPC.Points, points },
                {(int)SPC.Variables, hash },
                {(int)SPC.LoreRecords, m_FoundedLoreRecords.ToArray() }
            };
            return info;
        }


        public void SetVariable(string variableName, int count) {
            int oldCount = GetVariable(variableName);
            if (oldCount < count) {
                if (m_Variables.ContainsKey(variableName)) {
                    int oc;
                    if(m_Variables.TryRemove(variableName, out oc)) {
                        if(m_Variables.TryAdd(variableName, count)) {
                            s_Log.InfoFormat("set achievment variable {0} = {1}".Color(LogColor.orange), variableName, count);
                            NotifyUnlockedTiers(variableName, oldCount, count);
                        }
                    }
                } else {
                    if(m_Variables.TryAdd(variableName, count)) {
                        NotifyUnlockedTiers(variableName, oldCount, count);
                    }
                }
            }
        }

        public void AddVariable(string variableName, int count) {
            if (loaded) {
                if (m_Variables.ContainsKey(variableName)) {
                    int oldCount = 0;
                    if (m_Variables.TryRemove(variableName, out oldCount)) {
                        s_Log.InfoFormat("to variable: {0} added: {1}".Color(LogColor.yellow), variableName, count);
                        int newCount = oldCount + count;
                        m_Variables.TryAdd(variableName, newCount);
                        NotifyUnlockedTiers(variableName, oldCount, newCount);
                    }
                } else {
                    s_Log.InfoFormat("to variable: {0} added: {1}".Color(LogColor.yellow), variableName, count);
                    m_Variables.TryAdd(variableName, count);
                    NotifyUnlockedTiers(variableName, 0, count);
                }
            }
        }


        public int GetVariable(string variableName ) {
            if(loaded) {
                int cnt;
                if(m_Variables.TryGetValue(variableName, out cnt)) {
                    return cnt;
                }
            }
            return 0;
        }

        private void NotifyUnlockedTiers(string variableName, int oldCount, int newCount) {
            if (m_Message != null) {
                var achievments = resource.achievments.GetAchievmentsForVariable(variableName);
                foreach (var achievment in achievments) {
                    var unlockedTiers = achievment.GetUnlockedByCountTiers(oldCount, newCount);
                    foreach (var tier in unlockedTiers) {
                        AddPoints(tier.points);
                        m_Message.ReceiveAchievmentUnlocked(achievment.id, tier.id);
                    }
                }
            }
        }

        private void AddPoints(int pt) {
            m_Points += pt;
        }

        public void OnZoneVisited(string id) {
            if(!m_VisitedZones.Contains(id)) {
                m_VisitedZones.Add(id);
                AddVariable("system_visited", 1);
            }
        }

        public void OnTurretCreated() {
            AddVariable("turret_created", 1);
        }

        public void OnFortificationCreated() {
            AddVariable("fort_created", 1);
        }

        public void OnOutpostCreated() {
            AddVariable("outpost_created", 1);
        }

        public void OnContractCompleted( ) {
            AddVariable("contract_completed", 1);
        }

        public void OnCollectNebulaElement(int count) {
            AddVariable("total_collect_nebula_element", count);
        }

        public void OnOreCollected(int count) {
            AddVariable("total_collect_ore", count);
        }

        public void OnModuleCraft() {
            AddVariable("total_module_craft", 1);
        }
        public void OnWorldCaptured() {
            AddVariable("total_captured", 1);
        }

        public void OnMakeDamage(WeaponDamage val) {
            AddVariable("total_damage", (int)val.totalDamage);
        }

        public void OnHeal(float val ) {
            AddVariable("total_heal", (int)val);
        }

        public void OnEnemyDeath(NebulaObject enemy) {
            if(enemy.Type == (byte)ItemType.Avatar) {
                AddVariable("total_pvp_kill", 1);
                if(m_Raceable != null ) {
                    if((Race)m_Raceable.race == nebulaObject.mmoWorld().ownedRace ) {
                        AddVariable("pvp_kill_at_friend_system", 1);
                    } else {
                        AddVariable("pvp_kill_at_enemy_system", 1);
                    }
                }
            } else {
                var bot = enemy.GetComponent<BotObject>();
                if(bot != null && bot.botSubType == (byte)BotItemSubType.StandardCombatNpc) {
                    var raceable = enemy.GetComponent<RaceableObject>();
                    if(raceable != null ) {
                        switch((Race)raceable.race) {
                            case Race.Humans:
                                AddVariable("total_kill_npc_human", 1);
                                break;
                            case Race.Borguzands:
                                AddVariable("total_kill_npc_borguzand", 1);
                                break;
                            case Race.Criptizoids:
                                AddVariable("total_kill_npc_criptizid", 1);
                                break;
                        }
                    }
                }
            }
        }
    }

    public class AchievmentSave {
        public Hashtable variables { get; private set; }
        public List<string> visitedZones { get; private set; }
        public int points { get; private set; }
        public List<string> loreRecords { get; private set; }

        public AchievmentSave(Hashtable hash, List<string> visZones, int points, List<string> lorRecs) {
            variables = hash;
            visitedZones = visZones;
            this.points = points;
            loreRecords = lorRecs;
        }
    }
}
