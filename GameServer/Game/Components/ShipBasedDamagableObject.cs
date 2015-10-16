using Common;
using ExitGames.Logging;
using GameMath;
using Nebula.Engine;
using Nebula.Game.Skills;
using Nebula.Server.Components;
using Space.Game;

namespace Nebula.Game.Components {

    public class ShipBasedDamagableObject : DamagableObject {

        

        private BaseShip mShip;
        private PlayerBonuses mBonuses;
        private float mpcHPRegenNonCombatPerSec;
        private MmoActor player;
        private PlayerTarget mTarget;
        private PlayerSkills mSkills;
        private PassiveBonusesComponent mPassiveBonuses;

        private static ILogger log = LogManager.GetCurrentClassLogger();

        private float mIncreaseRegenSpeedTimer;
        private float mIncreaseRegenSpeedMultiplier;
        private EventedObject mEventedObject;
        

        public void Init(ShipDamagableComponentData data) {
            SetCreateChestOnKilling(data.createChestOnKilling);
        }

        public override void Start() {
            base.Start();
            mShip = RequireComponent<BaseShip>();
            mBonuses = RequireComponent<PlayerBonuses>();

            if (GetComponent<PlayerLoaderObject>()) {
                log.Info("load before destructable");
                GetComponent<PlayerLoaderObject>().Load();
            }

            float initHealth = maximumHealth;
            //log.InfoFormat("Set health at start to {0}", initHealth);
            SetHealth(initHealth);
            mpcHPRegenNonCombatPerSec = nebulaObject.resource.ServerInputs.GetValue<float>("hp_regen_non_combat");
            player = GetComponent<MmoActor>();
            mTarget = GetComponent<PlayerTarget>();
            mSkills = GetComponent<PlayerSkills>();
            mPassiveBonuses = GetComponent<PassiveBonusesComponent>();
            mEventedObject = GetComponent<EventedObject>();
        }

        public void Respawn() {
            SetHealth(baseMaximumHealth);
        }
        

        

        public override float maximumHealth {
            get {
                
                float maxHp =  baseMaximumHealth * (1.0f + mBonuses.maxHpPcBonus) + mBonuses.maxHpCntBonus;
                maxHp = ApplyMaximumHpPassiveBonus(maxHp);
                return maxHp;
            }
        }

        private float ApplyMaximumHpPassiveBonus(float inputMaxHp) {
            if(nebulaObject.IsPlayer()) {
                if(mPassiveBonuses != null && mPassiveBonuses.maxHPTier > 0 ) {
                    return inputMaxHp * (1.0f + mPassiveBonuses.maxHPBonus);
                }
            }
            return inputMaxHp;
        }

        public override float baseMaximumHealth {
            get {
                
                return mShip.shipModel.hp;
            }
        }

        public override void Update(float deltaTime) {
            base.Update(deltaTime);

            //regenerate health of player when it in non combat
            if(player) {
                if(mIncreaseRegenSpeedTimer > 0f) {
                    mIncreaseRegenSpeedTimer -= deltaTime;
                }
                if(!mTarget.inCombat) {
                    if(health < maximumHealth) {
                        SetHealth(health + ApplyRestoreHpPassiveBonus(mpcHPRegenNonCombatPerSec) * increaseRegenMultiplier * deltaTime * maximumHealth);
                    }
                }
            }
            
        }

        private float ApplyRestoreHpPassiveBonus(float inputRestoreHp) {
            if(nebulaObject.IsPlayer()) {
                if(mPassiveBonuses != null && mPassiveBonuses.restoreHPTier > 0 ) {
                    return inputRestoreHp * (1.0f + mPassiveBonuses.restoreHPBonus);
                }
            }
            return inputRestoreHp;
        }

        private float increaseRegenMultiplier {
            get {
                if(mIncreaseRegenSpeedTimer > 0 ) {
                    return mIncreaseRegenSpeedMultiplier;
                }
                return 1f;
            }
        }

        public void SetIncreaseRegenMultiplier(float mult, float time) {
            mIncreaseRegenSpeedMultiplier = mult;
            mIncreaseRegenSpeedTimer = time;
        }

        private float ApplyResistPassiveBonus(float inputResist) {
            if(nebulaObject.IsPlayer()) {
                if(mPassiveBonuses != null && mPassiveBonuses.resistTier > 0 ) {
                    return Mathf.Clamp01(inputResist + mPassiveBonuses.resistBonus);
                }
            }
            return inputResist;
        }

        public override float ReceiveDamage(byte damagerType, string damagerID, float damage, byte workshop, int level, byte race) {

            //firs call base behaviour
            base.ReceiveDamage(damagerType, damagerID, damage, workshop, level, race);
            if (!nebulaObject) { return 0f; }
            nebulaObject.SendMessage(ComponentMessages.InCombat);

            if (ignoreDamageAtStart) {
                if (GetComponent<MmoActor>()) {
                    log.Info("player damage ignored");
                }
                return 0f;
            }
            if(god) {
                log.Info("ShipBasedDamagableObject is GOD, return 0 damage");
                return 0f;
            }

            if(mShip == null ) {
                mShip = GetComponent<BaseShip>();
            }
            float resist = 0f;
            if (mShip != null) {
                resist = mShip.damageResistance;
            }
            resist = ApplyResistPassiveBonus(resist);

            damage *= (1.0f - Mathf.Clamp01(resist));

            damage = AbsorbDamage(damage);

            if (!god) {
                if(mBonuses) {
                    if(mBonuses.isImmuneToDamage) {
                        damage = 0f;
                    }
                }

                if(nebulaObject.IsPlayer()) {
                    damage = mTarget.MoveDamageToSubscriber(damage);
                }
                SetHealth(health - damage);
            }
            AddDamager(damagerID, damagerType, damage, workshop, level, race);

            if(mEventedObject != null ) {
                mEventedObject.ReceiveDamage(new DamageInfo(damagerID, damagerType, damage, workshop, level, race));
            }

            if(health <= 0f) {
                if (NotRespawnBySkill()) {
                    SetWasKilled(true);
                }
            }
            return damage;
        }


        
        private bool NotRespawnBySkill() {
            if(mSkills) {
                return (!mSkills.RespawnBySkill());
            }
            return true;
        }




    }
}
