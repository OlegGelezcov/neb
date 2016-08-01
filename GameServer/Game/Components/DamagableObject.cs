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
using Nebula.Game.Events;
using Nebula.Drop;

namespace Nebula.Game.Components {
    public abstract class DamagableObject  : NebulaBehaviour{

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        private readonly GodState mGodState = new GodState();
        private const float NO_ABSORB = -1f;
        private PlayerBonuses mBonuses;
        private PlayerSkills m_Skills;
        private AchievmentComponent m_Achievments;
        private BotObject m_Bot;
        private MmoMessageComponent m_Mmo;

        public override Hashtable DumpHash() {
            var hash =  base.DumpHash();
            hash["health"] = health.ToString();
            hash["god?"] = god.ToString();
            hash["damagers_count"] = m_Damagers.count.ToString();
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
            //hash["difficulty_mult"] = myDifficultyMult.ToString();
            //hash["level"] = myLevel.ToString();
            return hash;
        }
        private float m_Health = 1000;

        public bool god {
            get {
                return mGodState.god;
            }
            private set {
                mGodState.SetGod(value);
            }
        }

        private readonly DamagerCollection m_Damagers = new DamagerCollection();

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
                return m_Health;
            }
            private set {
                m_Health = Mathf.Clamp(value, -100000, maximumHealth);
            }
        }

        private float m_HealthFromLastSend = 0f;
        private void SendUpdateCurrentHealth() {
            float hp = health;
            SetCurrentHealthProperty(hp);

            if (!Mathf.Approximately(m_HealthFromLastSend, hp)) {
                m_HealthFromLastSend = hp;
                if (m_Mmo != null) {
                    m_Mmo.SendPropertyUpdate(new Hashtable { { (byte)PS.CurrentHealth, health } });
                }
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

        public ConcurrentDictionary<string, DamageInfo> damagers {
            get {
                return m_Damagers.damagers;
            }
        }

        protected DamagerCollection damagerCollection {
            get {
                return m_Damagers;
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
                        //nebulaObject.mmoWorld().OnEvent()
                    } else {
                        mmoMessanger.SendKilled(EventReceiver.ItemSubscriber);
                    }
                }

                if(nebulaObject.IsPlayer()) {
                    log.InfoFormat("player was killed, send PlayerKilledEvent to world");
                    nebulaObject.mmoWorld().OnEvent(new PlayerKilledEvent(nebulaObject));
                } else if(nebulaObject.IsBot()) {
                    var botComponent = nebulaObject.GetComponent<BotObject>();
                    if(botComponent != null ) {
                        if(botComponent.botSubType == (byte)BotItemSubType.Outpost ||
                            botComponent.botSubType == (byte)BotItemSubType.MainOutpost ||
                            botComponent.botSubType == (byte)BotItemSubType.Turret ) {
                            log.InfoFormat("construction was killed, send ConstructionKilledEvent to world");
                            nebulaObject.mmoWorld().OnEvent(new ConstructionKilledEvent(nebulaObject));
                        }
                    }
                }
            }
        }

        public void SetAbsorbDamage(float absorb) {
            mAbsorbedDamage = absorb;
        }

        protected virtual void AbsorbDamage(InputDamage inputDamage) {
            float absorbed = 0;
            float ret = inputDamage.totalDamage;

            if(mAbsorbedDamage > 0) {
                mAbsorbedDamage -= inputDamage.totalDamage;
                if(mAbsorbedDamage >= 0f ) {
                    absorbed = inputDamage.totalDamage;
                    ret = 0f;

                } else {
                    float result = Mathf.Abs(mAbsorbedDamage);
                    mAbsorbedDamage = NO_ABSORB;
                    absorbed = Mathf.Abs(inputDamage.totalDamage - result);
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

            inputDamage.ClearAllDamages();
            inputDamage.SetBaseDamage(ret);
            //return inputDamage;
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
            return m_Damagers.Has(id);
        }

        public override void Start() {
            //damagers = new Dictionary<string, DamageInfo>();
            props.SetProperty((byte)PS.IgnoreDamage, ignoreDamage);
            props.SetProperty((byte)PS.IgnoreDamageTimer, mIgnoreDamageTimer);
            mIgnoreDamageTimer = ignoreDamageInterval;
            mBonuses = GetComponent<PlayerBonuses>();
            timedDamage = new TimedDamage(this);
            m_Skills = GetComponent<PlayerSkills>();
            m_Achievments = GetComponent<AchievmentComponent>();
            m_Bot = GetComponent<BotObject>();
            m_Mmo = GetComponent<MmoMessageComponent>();
        }

        private void SetCurrentHealthProperty(float val) {
            nebulaObject.properties.SetProperty((byte)PS.CurrentHealth, val);
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
            SendUpdateCurrentHealth();

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

            if (m_Bot != null && m_Bot.isConstruction) {

            } else {
                //restore health only when heal not blocked
                if (!healBlocked) {
                    RestoreHealthPerSec(deltaTime);
                }
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
            if(m_Bot != null && m_Bot.isConstruction ) {
                return;
            }
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

                float val = Mathf.Abs(heal.value) * mult;


                AddHealth(val, true);

                if(m_Achievments != null ) {
                    m_Achievments.OnHeal(val);
                }
            }
        }


        protected virtual void AddHealth(float val, bool byHeal) {
            health += val;
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

        /// <summary>
        /// Add new damager item to damagers collection list
        /// </summary>
        /// <param name="damagerID">ID of damager</param>
        /// <param name="damagerType">Type of damager item</param>
        /// <param name="damage">Damage value</param>
        /// <param name="workshop">Workshop of damager if has workshop</param>
        /// <param name="level">Level of damager if has level</param>
        /// <param name="race">Race of damager if has race</param>
        /// <param name="source">Source damager object (may be null)</param>
        public void AddDamager(string damagerID, byte damagerType, float damage, byte workshop, int level, byte race, NebulaObject source) {

            //send message to damager when he is start attacking
            if(source != null && source ) {
                if(!m_Damagers.Has(source.Id)) {
                    source.SendMessage(ComponentMessages.OnStartAttack, nebulaObject);
                }
            }

            var damageInfo = m_Damagers.Add(damagerID, damagerType, damage, workshop, level, race);
            if(damageInfo != null ) {
                //send message to self object when receive new damage
                nebulaObject.SendMessage(ComponentMessages.OnNewDamage, damageInfo);
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


        public void SetTimedDamage(float interval, float damagePerSecond, WeaponBaseType wbt) {
            if(timedDamage != null ) {
                timedDamage.StartDamage(interval, damagePerSecond, wbt);
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
                inputDamage.Mult((1.0f + mBonuses.inputDamagePcBonus));
                //inputDamage.SetDamage(inputDamage.damage * (1.0f + mBonuses.inputDamagePcBonus));
                ApplyReflection(inputDamage);
            }
            if(inputDamage.hasDamager) {
                var damagerBons = inputDamage.source.Bonuses();
                if(damagerBons) {
                    float vampPc = damagerBons.vampirismPcBonus;
                    float hp = inputDamage.totalDamage * vampPc;
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
                    float reflectedDamageVal = inputDamage.totalDamage * reflectValue;
                    var attackerDamagable = inputDamage.source.Damagable();
                    WeaponDamage reflectedDamage = new WeaponDamage(inputDamage.weaponBaseType);
                    reflectedDamage.SetBaseTypeDamage(reflectedDamageVal);
                    if (attackerDamagable) {
                        DamageParams damageParams = new DamageParams();
                        damageParams.SetReflrected(true);
                        attackerDamagable.ReceiveDamage(new InputDamage(nebulaObject, reflectedDamage, damageParams));
                    }
                }
            }
        }



        public virtual void Death() {
            int damagerCount = m_Damagers.playerDamagerCount;

            if (mWasKilled) {
                m_Damagers.OnOwnerKilled(nebulaObject);
            } else {
                if (nebulaObject.Type == (byte)ItemType.Avatar) {
                    log.InfoFormat("Player does was not killed exp not give [purple]");
                }
            }

        }





        public override int behaviourId {
            get {
                return (int)ComponentID.Damagable;
            }
        }



        public class TimedDamage {

            private DamagableObject mTarget;

            public TimedDamage(DamagableObject target) {
                mTarget = target;
            }


            public bool active { get; private set; }
            public float damagePerSecond { get; private set; }

            private float mTimer;
            private WeaponBaseType m_TimedDamageType = WeaponBaseType.Rocket;

            public void StartDamage(float interval, float dmgPerSec, WeaponBaseType wbt) {
                damagePerSecond = dmgPerSec;
                mTimer = interval;
                m_TimedDamageType = wbt;
                active = true;
            }


            public void Update(float deltaTime) {
                if(active) {
                    WeaponDamage dmgPerSec = new WeaponDamage(m_TimedDamageType);
                    dmgPerSec.SetBaseTypeDamage(damagePerSecond * deltaTime);
                    InputDamage inpDamage = new InputDamage(null, dmgPerSec);
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
