using Common;
using ExitGames.Logging;
using GameMath;
using Nebula.Engine;
using Space.Game;
using Space.Game.Events;
using Space.Server;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Game.Components {
    public abstract class DamagableObject  : NebulaBehaviour{

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        private const float NO_ABSORB = -1f;

        private float mHealth = 1000;

        public bool god { get; private set; }

        public ConcurrentDictionary<string, DamageInfo> damagers { get; private set; } = new ConcurrentDictionary<string, DamageInfo>();

        public bool ignoreDamageAtStart { get; private set; }
        public float ignoreDamageInterval { get; private set; } = -1f;
        protected float mIgnoreDamageTimer = 0f;
        protected bool mCreateChestOnKilling = true;
        private bool mWasKilled = false;

        //private float mDamagePerSec = 0f;
        //private float mDamagePerSecTimer = -1f;

        private float mRestorPerSec = 0f;
        private float mRestorPerSecTimer = -1f;

        private float mReflectDamageProb = 0f;
        private float mReflectDamageTimer = 0f;

        private float mAbsorbedDamage = NO_ABSORB;

        private TimedDamage timedDamage { get; set; }

        private PlayerBonuses mBonuses;


        public bool healBlocked {
            get {
                if(!mBonuses) {
                    return false;
                }
                return (!Mathf.Approximately(0f, mBonuses.Value(BonusType.block_heal)));
            }
        }

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

        protected float AbsorbDamage(float inputDamage) {
            if(mAbsorbedDamage > 0) {
                mAbsorbedDamage -= inputDamage;
                if(mAbsorbedDamage >= 0f ) {
                    return 0f;
                } else {
                    float result = Mathf.Abs(mAbsorbedDamage);
                    mAbsorbedDamage = NO_ABSORB;
                    return result;

                }
            }
            return inputDamage;
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

        public override void Start() {
            //damagers = new Dictionary<string, DamageInfo>();
            props.SetProperty((byte)PS.IgnoreDamage, ignoreDamage);
            props.SetProperty((byte)PS.IgnoreDamageTimer, mIgnoreDamageTimer);
            mIgnoreDamageTimer = ignoreDamageInterval;
            mBonuses = GetComponent<PlayerBonuses>();
            timedDamage = new TimedDamage(this);
        }

        protected bool ignoreDamage {
            get {
                return god || ignoreDamageAtStart;
            }
        }

        public override void Update(float deltaTime) {

            if(nebulaObject.IAmBotAndNoPlayers()) {
                return;
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
            if (mRestorPerSecTimer > 0f) {
                mRestorPerSecTimer -= deltaTime;
                float addHP = mRestorPerSec * deltaTime;
                SetHealth(health + addHP);
                log.InfoFormat("restore self on value = {0:F1} yellow", addHP);
                var skills = GetComponent<PlayerSkills>();

                if(skills) {
                    skills.OnHealthRestored(nebulaObject, addHP);
                }

                if (mRestorPerSecTimer <= 0f) {
                    mRestorPerSec = 0f;
                }
            }
        }

        public void RestoreHealth(NebulaObject source, float hp) {
            if (!healBlocked) {
                var skills = nebulaObject.Skills();
                
                if (mBonuses) {
                    if (source.Id != nebulaObject.Id) {
                        hp *= (1.0f + mBonuses.healingSpeedPcBonus);
                    }
                }
                SetHealth(health + hp);
                
                if (skills) {
                    skills.OnHealthRestored(source, hp);
                }
            }
        }

        public float health {
            get {
                return mHealth;
            }
            private set {
                mHealth = Mathf.Clamp(value, -100, maximumHealth);
            }
        }





        public abstract float maximumHealth { get; }

        public abstract float baseMaximumHealth { get;  }

        public void SetHealth(float inHealth) {
            //when increase health - only when non blocked
            if(inHealth > health) {
                if(!healBlocked) {
                    health = inHealth;
                }
            } else {
                health = inHealth;
            }
        }

        //forcing set health to value, ignore block healing abilities
        public void ForceSetHealth(float inHealth) {
            health = inHealth;
        }

        public void SetGod(bool inGod) {
            god = inGod;
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

        public void OnReturnStateChanged(object ret) {
            bool bRet = (bool)ret;
            log.InfoFormat("Bot return state changed = {0} green", bRet);
            if(bRet) {
                SetHealth(maximumHealth);
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

        public void SetRestoreHPPerSec(float restorePerSec, float restoreTimer) {
            mRestorPerSec = restorePerSec;
            mRestorPerSecTimer = restoreTimer;
        }
        public virtual float ReceiveDamage(byte damagerType, string damagerID, float damage, byte workshop, int level, byte race) {
            nebulaObject.SetInvisibility(false);
            return 0f;
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
                    log.InfoFormat("player was killed and give exp [purple]");
                }
                foreach (var damager in damagers) {
                    if (damager.Value.DamagerType == ItemType.Avatar) {
                        NebulaObject damagerObject;
                        if (nebulaObject.world.TryGetObject((byte)damager.Value.DamagerType, damager.Value.DamagerId, out damagerObject)) {

                            int baseExp = 20;
                            float difficulty = 1;
                            float npcLevel = 1;
                            float playerLevel = 1;

                            if (GetComponent<BotShip>()) {
                                if (nebulaObject.HasTag((byte)PS.Difficulty)) {
                                    difficulty = nebulaObject.resource.GetDifficultyMult((Difficulty)(byte)nebulaObject.Tag((byte)PS.Difficulty));
                                }
                                //difficulty = nebulaObject.resource.GetDifficultyMult(GetComponent<BotShip>().difficulty);
                            }


                            if (GetComponent<CharacterObject>()) {
                                npcLevel = GetComponent<CharacterObject>().level;
                            }

                            if (damagerObject.GetComponent<PlayerCharacterObject>()) {
                                playerLevel = damagerObject.GetComponent<PlayerCharacterObject>().level;
                            }

                            int exp = (int)(baseExp * difficulty * (npcLevel / playerLevel));
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
                    mTarget.ReceiveDamage((byte)ItemType.Bot, string.Empty, damagePerSecond * deltaTime, (byte)Workshop.Arlen, 1, (byte)Race.Criptizoids);
                    mTimer -= deltaTime;
                    if(mTimer <= 0f ) {
                        active = false;
                    }
                }
            }
        }
    }
}
