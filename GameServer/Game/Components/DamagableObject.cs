using Common;
using ExitGames.Logging;
using GameMath;
using Nebula.Engine;
using Nebula.Game.Bonuses;
using Space.Game;
using Space.Server;
using System;
using System.Collections.Concurrent;
using System.Collections;

namespace Nebula.Game.Components {
    public abstract class DamagableObject  : NebulaBehaviour{

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        private readonly GodState mGodState = new GodState();
        private const float NO_ABSORB = -1f;
        private PlayerBonuses mBonuses;
        private PlayerSkills m_Skills;

        public override Hashtable DumpHash() {
            var hash =  base.DumpHash();
            hash["health"] = health.ToString();
            hash["god?"] = god.ToString();
            hash["damagers_count"] = damagers.Count.ToString();
            hash["ignore_damage_at_spawn?"] = mIgnoreDamageTimer.ToString();
            hash["ignore_damage_at_spawn_interval"] = ignoreDamageInterval.ToString();
            hash["ignore_damage_timer"] = mIgnoreDamageTimer.ToString();
            hash["create_container_when_killed?"] = mCreateChestOnKilling.ToString();
            hash["killed?"] = mWasKilled.ToString();
            hash["reflect_damage%"] = mReflectDamageProb.ToString();
            hash["reflect_damage_timer"] = mReflectDamageTimer.ToString();
            hash["absorbed_damage"] = mAbsorbedDamage.ToString();
            hash["timed_damage"] = (timedDamage != null) ? timedDamage.damagePerSecond.ToString() : "0";
            hash["heal_blocked?"] = healBlocked.ToString();
            hash["difficulty_mult"] = myDifficultyMult.ToString();
            hash["level"] = myLevel.ToString();
            return hash;
        }
        private float mHealth = 1000;

        public bool god {
            get {
                return mGodState.god;
            }
            private set {
                mGodState.SetGod(value);
            }
        }

        public ConcurrentDictionary<string, DamageInfo> damagers { get; private set; } = new ConcurrentDictionary<string, DamageInfo>();

        public bool ignoreDamageAtStart { get; private set; }
        public float ignoreDamageInterval { get; private set; } = -1f;
        protected float mIgnoreDamageTimer = 0f;
        protected bool mCreateChestOnKilling = true;
        private bool mWasKilled = false;

        //private float mRestorPerSec = 0f;
        //private float mRestorPerSecTimer = -1f;

        private float mReflectDamageProb = 0f;
        private float mReflectDamageTimer = 0f;

        private float mAbsorbedDamage = NO_ABSORB;

        private TimedDamage timedDamage { get; set; }

        public bool healBlocked {
            get {
                if(!mBonuses) {
                    return false;
                }
                return (!Mathf.Approximately(0f, mBonuses.Value(BonusType.block_heal)));
            }
        }

        protected bool ignoreDamage {
            get {
                return god || ignoreDamageAtStart;
            }
        }

        public float health {
            get {
                return mHealth;
            }
            private set {
                mHealth = Mathf.Clamp(value, -100000, maximumHealth);
            }
        }

        public bool killed {
            get {
                return mWasKilled;
            }
        }

        public float reflectDamageProb {
            get {
                return mReflectDamageProb;
            }
        }

        public float reflectDamageTimer {
            get {
                return mReflectDamageTimer;
            }
        }

        public bool createChestOnKilling {
            get {
                return mCreateChestOnKilling;
            }
        }

        private float myDifficultyMult {
            get {
                float d = 1.0f;
                BotShip botShip = GetComponent<BotShip>();

                if (botShip != null) {
                    if (nebulaObject.HasTag((byte)PS.Difficulty)) {
                        d = nebulaObject.resource.GetDifficultyMult((Difficulty)(byte)nebulaObject.Tag((byte)PS.Difficulty));
                    }
                    //difficulty = nebulaObject.resource.GetDifficultyMult(GetComponent<BotShip>().difficulty);
                }
                return d;
            }
        }

        private float myLevel {
            get {
                float level = 1.0f;
                var character = GetComponent<CharacterObject>();
                if (character != null) {
                    level = character.level;
                }
                return level;
            }
        }

        public abstract float maximumHealth { get; }

        public abstract float baseMaximumHealth { get; }


        public void SetIgnoreDamageAtStart(bool ignore) {

            ignoreDamageAtStart = ignore;
            props.SetProperty((byte)PS.IgnoreDamage, ignoreDamage);
        }

        public void SetIgnoreDamageInterval(float ignoreInterval) {
            ignoreDamageInterval = ignoreInterval;
        }

        public void SetReflectParameters(float reflectProb, float reflectTimer ) {
            mReflectDamageProb = reflectProb;
            mReflectDamageTimer = reflectTimer;
        }

        protected void SetWasKilled(bool inWasKilled) {
            bool oldKilled = mWasKilled;
            mWasKilled = inWasKilled;
            if(!oldKilled && mWasKilled) {
                var mmoMessanger = GetComponent<MmoMessageComponent>();
                if(mmoMessanger) {
                    if(GetComponent<MmoActor>()) {
                        mmoMessanger.SendKilled(EventReceiver.OwnerAndSubscriber);
                    } else {
                        
                        mmoMessanger.SendKilled(EventReceiver.ItemSubscriber);
                    }
                }
            }
        }

        public void SetAbsorbDamage(float absorb) {
            mAbsorbedDamage = absorb;
        }

        protected virtual float AbsorbDamage(float inputDamage) {
            float absorbed = 0;
            float ret = inputDamage;

            if(mAbsorbedDamage > 0) {
                mAbsorbedDamage -= inputDamage;
                if(mAbsorbedDamage >= 0f ) {
                    absorbed = inputDamage;
                    ret = 0f;

                } else {
                    float result = Mathf.Abs(mAbsorbedDamage);
                    mAbsorbedDamage = NO_ABSORB;
                    absorbed = Mathf.Abs(inputDamage - result);
                    ret = result;

                }
            }

            if(mBonuses) {
                if(ret > 0 ) {
                    float absorbPC = mBonuses.absrodDamagePcBonus;
                    float dmgToAbsorbe = ret * absorbPC;
                    ret -= dmgToAbsorbe;

                    float restoreHpPc = mBonuses.convertAbsorbedDamageToHpPcBonus;
                    float hp = (absorbed + dmgToAbsorbe) * restoreHpPc;
                    Heal(new InputHeal(hp));
                }
            }

            return ret;
        }

        /// <summary>
        /// Set ignore damage at any time
        /// </summary>
        /// <param name="interval"></param>
        public void SetIgnoreDamage(float interval) {
            SetIgnoreDamageAtStart(true);
            SetIgnoreDamageInterval(interval);
            mIgnoreDamageTimer = ignoreDamageInterval;
        }

        public void SetCreateChestOnKilling(bool create) {
            mCreateChestOnKilling = create;
        }

        public bool HasDamager(string id ) {
            if(damagers != null ) {
                if(damagers.ContainsKey(id)) {
                    return true;
                }
            }
            return false;
        }

        public override void Start() {
            //damagers = new Dictionary<string, DamageInfo>();
            props.SetProperty((byte)PS.IgnoreDamage, ignoreDamage);
            props.SetProperty((byte)PS.IgnoreDamageTimer, mIgnoreDamageTimer);
            mIgnoreDamageTimer = ignoreDamageInterval;
            mBonuses = GetComponent<PlayerBonuses>();
            timedDamage = new TimedDamage(this);
            m_Skills = GetComponent<PlayerSkills>();
        }

        public override void Update(float deltaTime) {

            if (nebulaObject.IAmBot()) {
                //update god timer for infinite god state error only on bots (not players)
                mGodState.Update(deltaTime);

                if (nebulaObject.IAmBotAndNoPlayers()) {
                    return;
                }
            }


            nebulaObject.properties.SetProperty((byte)PS.MaxHealth, maximumHealth);
            nebulaObject.properties.SetProperty((byte)PS.CurrentHealth, health);

            bool destoyed = (!nebulaObject);

            //log.InfoFormat("Destroyed: {0}, health: {1} maximum health: {2}", destoyed, health, maximumHealth);
            nebulaObject.properties.SetProperty((byte)PS.Destroyed, destoyed);

            if (ignoreDamageAtStart) {
                if (ignoreDamageInterval > 0f) {
                    mIgnoreDamageTimer -= deltaTime;
                    if (mIgnoreDamageTimer <= 0f) {
                        SetIgnoreDamageAtStart(false);
                        props.SetProperty((byte)PS.IgnoreDamage, ignoreDamage);
                        //log.InfoFormat("Object [{0}:{1}] now not ignore damage", (ItemType)nebulaObject.Type, nebulaObject.Id);
                    }
                    props.SetProperty((byte)PS.IgnoreDamageTimer, mIgnoreDamageTimer);
                }
            } else {
                timedDamage.Update(deltaTime);

            }

            //restore health only when heal not blocked
            if (!healBlocked) {
                RestoreHealthPerSec(deltaTime);
            }

            if(mReflectDamageTimer > 0f ) {
                mReflectDamageTimer -= deltaTime;
                if(mReflectDamageTimer <= 0f ) {
                    mReflectDamageProb = 0f;
                }
            }

            if (health <= 0f) {
                (nebulaObject as Item).OnGameLogicDeath();
            }
        }

        private void RestoreHealthPerSec(float deltaTime) {
            if(mBonuses) {
                float hpCntAtSec = mBonuses.restoreHpAtSecCntBonus;
                float hpPcAtSec = mBonuses.restoreHpAtSecPcBonus * baseMaximumHealth;
                float restHp = (hpCntAtSec + hpPcAtSec) * deltaTime;

                if(Mathf.Approximately(restHp, 0.0f)) {
                    restHp = 0.0f;
                }

                if (restHp > 0.0f) {
                    Heal(new InputHeal(restHp));
                    log.InfoFormat("restore self on value = {0:F1} yellow", restHp);
                    if (m_Skills) {
                        m_Skills.OnHealthRestored(nebulaObject, restHp);
                    }
                }
            }
        }

        public void RestoreHealth(NebulaObject source, float hp) {
            if (!healBlocked) {
                Heal(new InputHeal(hp, source));
                if (m_Skills) {
                    m_Skills.OnHealthRestored(source, hp);
                }
            }
        }

        public void Heal(InputHeal heal) {
            if (!healBlocked && (!killed)) {
                float mult = 1.0f;
                if(mBonuses) {
                    mult = (1.0f + mBonuses.healingSpeedPcBonus);
                }
                if(heal.hasSource) {
                    if(heal.source.Id == nebulaObject.Id ) {
                        mult = 1.0f;
                    }
                }
                health += Mathf.Abs(heal.value) * mult;
            }
        }
        public void SubHealth(float hp) {
            health -= Mathf.Abs( hp );
        }

        //forcing set health to value, ignore block healing abilities
        public void ForceSetHealth(float inHealth) {
            if (!killed) {
                health = inHealth;
            }
        }

        public void SetGod(bool inGod) {
            god = inGod;
        }

        public void OnReturnStateChanged(object ret) {
            bool bRet = (bool)ret;
            log.InfoFormat("Bot return state changed = {0} green", bRet);
            if(bRet) {
                ForceSetHealth(maximumHealth);
                SetGod(true);
            } else {
                SetGod(false);
            }
        }

        
        public void AddDamager(string damagerID, byte damagerType, float damage, byte workshop, int level, byte race) {
            //nebulaObject.SendMessage("InCombat");
            //if(damagers == null ) {
            //    damagers = new Dictionary<string, DamageInfo>();
            //}
            if(damagers.ContainsKey(damagerID)) {
                damagers[damagerID].AddDamage(damage);
                nebulaObject.SendMessage(ComponentMessages.OnNewDamage, damagers[damagerID]);
            } else {
                if (race != (byte)Race.None && damagerType == (byte)ItemType.Avatar) {
                    var damageInfo = new DamageInfo(damagerID, damagerType, damage, workshop, level, race);
                    damagers.TryAdd(damagerID, damageInfo);
                    nebulaObject.SendMessage(ComponentMessages.OnNewDamage, damageInfo);
                }
            }
        }

        public bool TryReflectDamage() {
            if(reflectDamageTimer > 0f && reflectDamageProb > 0f ) {
                if( Rand.Float01() < reflectDamageProb ) {
                    return true;
                }
            }
            return false;
        }

        //public float ReceiveDamage(byte damagerType, string damagerID, float damage) {
        //    return ReceiveDamage(damagerType, damagerID, damage, (byte)Workshop.Arlen, 1);
        //}


        public void SetTimedDamage(float interval, float damagePerSecond) {
            if(timedDamage != null ) {
                timedDamage.StartDamage(interval, damagePerSecond);
            }
        }

        public void SetRestoreHPPerSec(float restorePerSec, float restoreTimer, string buffId) {
            if(mBonuses) {
                Buff buff = new Buff(buffId, null, BonusType.restore_hp_at_sec_on_cnt, restoreTimer, restorePerSec);
                mBonuses.SetBuff(buff);
            }
            //mRestorPerSec = restorePerSec;
            //mRestorPerSecTimer = restoreTimer;
        }

        public virtual InputDamage ReceiveDamage(InputDamage inputDamage) {
            nebulaObject.SetInvisibility(false);
            if(mBonuses) {
                inputDamage.SetDamage(inputDamage.damage * (1.0f + mBonuses.inputDamagePcBonus));
                ApplyReflection(inputDamage);
            }
            if(inputDamage.hasDamager) {
                var damagerBons = inputDamage.source.Bonuses();
                if(damagerBons) {
                    float vampPc = damagerBons.vampirismPcBonus;
                    float hp = inputDamage.damage * vampPc;
                    var dDamagable = inputDamage.source.Damagable();
                    if(dDamagable) {
                        dDamagable.Heal(new InputHeal(hp));
                    }
                }
            }
            return inputDamage;
        }

        private void ApplyReflection(InputDamage inputDamage) {
            if ( (false == inputDamage.reflected) && inputDamage.hasDamager) {
                var reflectValue = mBonuses.reflectionPc;
                if (false == Mathf.Approximately(reflectValue, 0f)) {
                    float reflectedDamage = inputDamage.damage * reflectValue;
                    var attackerDamagable = inputDamage.source.Damagable();
                    if (attackerDamagable) {
                        attackerDamagable.ReceiveDamage(new InputDamage(nebulaObject, reflectedDamage, new DamageParams { reflected = true }));
                    }
                }
            }
        }



        private float DamagerPlayerLevel(NebulaObject damager) {
            var playerCharacter = damager.GetComponent<PlayerCharacterObject>();
            if(playerCharacter != null ) {
                return playerCharacter.level;
            }
            return 1.0f;
        }

        public virtual void Death() {
            int damagerCount = 0;
            foreach (var damager in damagers) {
                if (damager.Value.DamagerType == ItemType.Avatar) {
                    damagerCount++;
                }
            }

            if (mWasKilled) {
                if(nebulaObject.Type == (byte)ItemType.Avatar) {
                    log.InfoFormat("player was killed and give pvp points [purple]");
                    GivePvpPoints();

                }

                float difficulty = myDifficultyMult;
                float npcLevel = myLevel;

                foreach (var damager in damagers) {
                    if (damager.Value.DamagerType == ItemType.Avatar) {
                        NebulaObject damagerObject;
                        if (nebulaObject.world.TryGetObject((byte)damager.Value.DamagerType, damager.Value.DamagerId, out damagerObject)) {
                            int baseExp = 20;
                            float playerLevel = DamagerPlayerLevel(damagerObject);

                            float levelRat = npcLevel / playerLevel;

                            float bexp = (difficulty * levelRat * (baseExp));
                            float hpExp = difficulty * Mathf.ClampLess(maximumHealth - 1000, 0f) * 0.01f;
                            if(levelRat < 1.0f ) {
                                hpExp *= levelRat;
                            }
                            int exp = (int)Math.Round(bexp + hpExp);

                            log.InfoFormat("sended exp = {0} for bot difficulty = {1}, hp exp bonus = {2}", 
                                exp, difficulty, (int)(difficulty * Mathf.ClampLess(maximumHealth - 1000, 0f) * 0.01f));
                            damagerObject.GetComponent<PlayerCharacterObject>().AddExp(exp);
                            damagerObject.SendMessage(ComponentMessages.OnEnemyDeath, nebulaObject);
                        }
                    }
                }
            } else {
                if (nebulaObject.Type == (byte)ItemType.Avatar) {
                    log.InfoFormat("Player does was not killed exp not give [purple]");
                }
            }

        }


        //send pvp points to players, only called on players
        protected void GivePvpPoints() {
            //if i am player
            if (nebulaObject.Type == (byte)ItemType.Avatar) {
                log.InfoFormat("give php point from avatar... [red]");
                int meLevel = nebulaObject.Character().level;

                foreach (var pDamage in damagers) {
                    var damager = pDamage.Value;
                    if (damager.DamagerType == ItemType.Avatar) {
                        if (damager.level <= (meLevel - 5)) {
                            GivePvpPointsToDamager(damager, 10);
                        } else if ((damager.level > (meLevel - 5) && (damager.level <= (meLevel + 5)))) {
                            GivePvpPointsToDamager(damager, 5);
                        }
                    }
                }
            } else if(nebulaObject.Type == (byte)ItemType.Bot ) {

                //give pvp points from killing turrets, fortifications, outposts
                var botObject = nebulaObject.GetComponent<BotObject>();
                if(botObject != null ) {
                    int points = 0;
                    switch((BotItemSubType)botObject.botSubType) {
                        case BotItemSubType.Turret:
                            points = 5;
                            break;
                        case BotItemSubType.Outpost:
                            points = 10;
                            break;
                        case BotItemSubType.MainOutpost:
                            points = 20;
                            break;
                    }

                    if (points > 0) {
                        foreach (var pDamage in damagers) {
                            var damager = pDamage.Value;
                            if (damager.DamagerType == ItemType.Avatar) {
                                GivePvpPointsToDamager(damager, points);
                            }
                        }
                    }
                }
            }
        }

        private void GivePvpPointsToDamager(DamageInfo damager, int points ) {
            NebulaObject damagerObject = null;
            if(nebulaObject.mmoWorld().TryGetObject((byte)ItemType.Avatar, damager.DamagerId, out damagerObject)) {
                var damagerCharacter = damagerObject.Character() as PlayerCharacterObject;
                if(damagerCharacter != null ) {
                    damagerCharacter.AddPvpPoints(points);
                }
            }
        }

        public override int behaviourId {
            get {
                return (int)ComponentID.Damagable;
            }
        }

        private void ClearDamagers() {
            //if(damagers == null ) {
            //    damagers = new Dictionary<string, DamageInfo>();
            //}
            damagers.Clear();
        }

        //Handler of message which sended from PlayerTarget component, when object bot exit from combat state
        //public void ExitCombat() {
        //    //log.InfoFormat("bot exit from Combat state yellow");
        //    //ClearDamagers();
        //    //SetHealth(maximumHealth);
        //}

        public class TimedDamage {

            private DamagableObject mTarget;

            public TimedDamage(DamagableObject target) {
                mTarget = target;
            }


            public bool active { get; private set; }
            public float damagePerSecond { get; private set; }

            private float mTimer;

            public void StartDamage(float interval, float dmgPerSec) {
                damagePerSecond = dmgPerSec;
                mTimer = interval;
                active = true;
            }


            public void Update(float deltaTime) {
                if(active) {
                    InputDamage inpDamage = new InputDamage(null, damagePerSecond * deltaTime);
                    mTarget.ReceiveDamage(inpDamage);
                    mTimer -= deltaTime;
                    if(mTimer <= 0f ) {
                        active = false;
                    }
                }
            }
        }
    }
}
