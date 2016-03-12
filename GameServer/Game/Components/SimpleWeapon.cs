using System.Collections;
using ExitGames.Logging;
using Nebula.Server.Components;
using Space.Game;
using GameMath;
using Nebula.Engine;
using System;
using Nebula.Drop;
using Common;

namespace Nebula.Game.Components {
    public class SimpleWeapon : BaseWeapon, IDatabaseObject {

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        private float mOptimalDistance;
        private float m_DamageVal;
        private float mCooldown;
        private bool mUseTargetHP = false;
        private float mTargetHPPercent = 0f;
        private float mTimer = 0f;
        private bool mReady = true;
        private PlayerBonuses mBonuses;
        private IDatabaseComponentData mInitData;
        private RaceableObject m_Raceable;

        public override void Start() {
            base.Start();
            mBonuses = GetComponent<PlayerBonuses>();
            m_Raceable = GetComponent<RaceableObject>();
        }

        public override WeaponBaseType myWeaponBaseType {
            get {
                if(m_Raceable != null ) {
                    return CommonUtils.Race2WeaponBaseType((Race)m_Raceable.race);
                }
                return WeaponBaseType.Rocket;
            }
        }

        public void Init(SimpleWeaponComponentData data) {
            mInitData = data;
            mOptimalDistance = data.optimalDistance;
            m_DamageVal = data.damage;
            mCooldown = data.cooldown;
            mUseTargetHP = data.useTargetHPForDamage;
            mTargetHPPercent = data.targetHPPercentDamage;
        }

        public override float optimalDistance {
            get {
                
                float result =  mOptimalDistance;
                if(mBonuses) {
                    result = Mathf.ClampLess(result * (1f + mBonuses.optimalDistancePcBonus) + mBonuses.optimalDistanceCntBonus, 0f);
                }
                return result;
            }
        }

        public override float criticalChance {
            get {
                return 0;
            }
        }


        public override WeaponDamage GetDamage(bool isCrit) {
            if (!mUseTargetHP) {
                return new WeaponDamage(Common.WeaponBaseType.Rocket, m_DamageVal, 0, 0);
            } else {
                if(!cachedTarget.targetObject) {
                    return new WeaponDamage(Common.WeaponBaseType.Rocket);
                }
                var targetDamagable = cachedTarget.targetObject.GetComponent<DamagableObject>();
                if(!targetDamagable) {
                    return new WeaponDamage(Common.WeaponBaseType.Rocket);
                }
                return new WeaponDamage(Common.WeaponBaseType.Rocket, targetDamagable.maximumHealth * mTargetHPPercent, 0, 0);
            }
        }

        public override bool ready {
            get {
                return mReady;
            }
        }

        public override Hashtable Fire(NebulaObject targetObject, out WeaponHitInfo hit, int skillID = -1, float damageMult = 1, bool forceShot = false, bool useDamageMultSelfAsDamage = false) {
            return base.Fire(targetObject, out hit, skillID, damageMult, forceShot, useDamageMultSelfAsDamage);
        }


        public override void Update(float deltaTime) {
            base.Update(deltaTime);

            if(!ready) {
                mTimer += deltaTime;
                if(mTimer >= mCooldown ) {
                    mTimer = 0f;
                    mReady = true;
                }
            }
        }

        public Hashtable GetDatabaseSave() {
            if(mInitData != null ) {
                return mInitData.AsHash();
            }
            return new Hashtable();
        }
    }
}
