using System.Collections;
using Common;
using ExitGames.Logging;
using GameMath;
using Nebula.Engine;
using Nebula.Game.Pets;
using Nebula.Server.Components;
using Space.Game;

namespace Nebula.Game.Components {

    public class ShipBasedDamagableObject : DamagableObject {
        private BaseShip mShip;
        private PlayerBonuses mBonuses;
        
        private MmoActor player;
        private PlayerTarget mTarget;
        private PlayerSkills mSkills;
        private PassiveBonusesComponent mPassiveBonuses;
        private MmoMessageComponent m_Message;
        //private EventedObject mEventedObject;

        private static ILogger log = LogManager.GetCurrentClassLogger();

        public override Hashtable DumpHash() {
            var hash = base.DumpHash();
            hash["increase_regen_speed_timer"] = mIncreaseRegenSpeedTimer.ToString();
            hash["increase_regen_speed_mult"] = mIncreaseRegenSpeedMultiplier.ToString();
            hash["regen_hp_in_non_battle %"] = mpcHPRegenNonCombatPerSec.ToString();
            hash["maximum_hp"] = maximumHealth.ToString();
            hash["base_maximum_hp"] = baseMaximumHealth.ToString();
            return hash;
        }
        private float mIncreaseRegenSpeedTimer;
        private float mIncreaseRegenSpeedMultiplier;
        
        private float mpcHPRegenNonCombatPerSec;

        public override float maximumHealth {
            get {
                float maxHp = baseMaximumHealth;
                if (mBonuses != null) {
                    maxHp = baseMaximumHealth * (1.0f + mBonuses.maxHpPcBonus) + mBonuses.maxHpCntBonus;
                }
                maxHp = ApplyMaximumHpPassiveBonus(maxHp);
                return maxHp;
            }
        }
        public override float baseMaximumHealth {
            get {

                return mShip.shipModel.hp;
            }
        }

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
            ForceSetHealth(initHealth);
            mpcHPRegenNonCombatPerSec = nebulaObject.resource.ServerInputs.GetValue<float>("hp_regen_non_combat");
            player = GetComponent<MmoActor>();
            mTarget = GetComponent<PlayerTarget>();
            mSkills = GetComponent<PlayerSkills>();
            mPassiveBonuses = GetComponent<PassiveBonusesComponent>();
            //mEventedObject = GetComponent<EventedObject>();
            m_Message = GetComponent<MmoMessageComponent>();
        }

        public void Respawn() {
            ForceSetHealth(baseMaximumHealth);
        }
        
        private float ApplyMaximumHpPassiveBonus(float inputMaxHp) {
            if(nebulaObject.IsPlayer()) {
                if(mPassiveBonuses != null && mPassiveBonuses.maxHPTier > 0 ) {
                    return inputMaxHp * (1.0f + mPassiveBonuses.maxHPBonus);
                }
            }
            return inputMaxHp;
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
                        Heal(new InputHeal(ApplyRestoreHpPassiveBonus(mpcHPRegenNonCombatPerSec) * increaseRegenMultiplier * deltaTime * maximumHealth));
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

        //private float ApplyResistPassiveBonus(float inputResist) {
        //    if(nebulaObject.IsPlayer()) {
        //        if(mPassiveBonuses != null && mPassiveBonuses.resistTier > 0 ) {
        //            return Mathf.Clamp01(inputResist + mPassiveBonuses.resistBonus);
        //        }
        //    }
        //    return inputResist;
        //}

        public override InputDamage ReceiveDamage(InputDamage inputDamage) {

            //firs call base behaviour
            inputDamage = base.ReceiveDamage(inputDamage);

            if (!nebulaObject) {
                inputDamage.ClearAllDamages();
                //inputDamage.SetDamage(0.0f);
                return inputDamage;
            }

            nebulaObject.SendMessage(ComponentMessages.InCombat);

            if (ignoreDamageAtStart || god) {
                //if (GetComponent<MmoActor>()) {
                //    log.Info("player damage ignored");
                //}
                //return 0f;
                inputDamage.ClearAllDamages();
                //inputDamage.SetDamage(0.0f);
                return inputDamage;
            }
            //if(god) {
            //    //log.Info("ShipBasedDamagableObject is GOD, return 0 damage");
            //    return 0f;
            //}

            if(mShip == null ) {
                mShip = GetComponent<BaseShip>();
            }
            float resist = 0f;
            float acidResist = 0.0f;
            float laserResist = 0.0f;
            float rocketResist = 0.0f;
            if (mShip != null) {
                resist = mShip.commonResist;
                acidResist = mShip.acidResist;
                laserResist = mShip.laserResist;
                rocketResist = mShip.rocketResist;
            }
            //resist = ApplyResistPassiveBonus(resist);

            inputDamage.Mult(1.0f - Mathf.Clamp01(resist));

            inputDamage.Mult(WeaponBaseType.Acid, 1.0f - Mathf.Clamp01(acidResist));
            inputDamage.Mult(WeaponBaseType.Laser, 1.0f - Mathf.Clamp01(laserResist));
            inputDamage.Mult(WeaponBaseType.Rocket, 1.0f - Mathf.Clamp01(rocketResist));

            //inputDamage.SetDamage(inputDamage.damage * (1.0f - Mathf.Clamp01(resist)));
            AbsorbDamage(inputDamage);
            //inputDamage.CopyValues(AbsorbDamage(inputDamage.damage));

            if (!god) {
                if(mBonuses) {
                    if(mBonuses.isImmuneToDamage) {
                        inputDamage.ClearAllDamages();
                        //inputDamage.SetDamage(0f);
                    }
                }

                if(nebulaObject.IsPlayer()) {
                    mTarget.MoveDamageToSubscriber(inputDamage);
                }
                SubHealth(inputDamage.totalDamage);
            }

            if (inputDamage.hasDamager) {
                AddDamager(inputDamage.sourceId, inputDamage.sourceType, inputDamage.totalDamage, (byte)inputDamage.workshop, inputDamage.level, (byte)inputDamage.race);
            }

            //if(mEventedObject != null && inputDamage.hasDamager) {
            //    mEventedObject.ReceiveDamage(new DamageInfo(inputDamage.sourceId, inputDamage.sourceType, inputDamage.damage, (byte)inputDamage.workshop, inputDamage.level, (byte)inputDamage.race));
            //}

            if(health <= 0f) {
                if (NotRespawnBySkill()) {
                    SetWasKilled(true);
                } else {
                    if(nebulaObject.Type == (byte)ItemType.Avatar ) {
                        if(m_Message) {
                            m_Message.ResurrectBySkillEffect();
                        }
                    }
                }
            }
            return inputDamage;
        }

        private bool NotRespawnBySkill() {
            if(mSkills) {
                bool notRespawned =  (false == mSkills.RespawnBySkill());
                if(notRespawned) {
                    return NotRespawnByPets();
                }
            }
            return true;
        }

        private bool NotRespawnByPets() {
            return (false == RespawnByPets());
        }

        private bool RespawnByPets() {
            var playerCharacter = GetComponent<PlayerCharacterObject>();
            if (!playerCharacter) {
                return false;
            }
            if(playerCharacter.hasGroup) {
                var memberPlayers = playerCharacter.GroupMemberPlayers(300);
                if(memberPlayers.Count == 0 ) {
                    return false;
                }

                foreach(var player in memberPlayers) {
                    var petManager = player.GetComponent<PetManager>();
                    if(petManager) {
                        if(petManager.HasPetWithValidActiveSkill(11)) {
                            if(petManager.UseExplicit(11, nebulaObject)) {
                                ForceSetHealth(maximumHealth);
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
    }
}
