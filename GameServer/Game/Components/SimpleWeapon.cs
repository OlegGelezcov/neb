using System.Collections;
using ExitGames.Logging;
using Nebula.Server.Components;
using Space.Game;
using GameMath;
using Nebula.Engine;
using System;

namespace Nebula.Game.Components {
    public class SimpleWeapon : BaseWeapon, IDatabaseObject {

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        private float mOptimalDistance;
        private float mDamage;
        private float mCooldown;
        private bool mUseTargetHP = false;
        private float mTargetHPPercent = 0f;
        private float mTimer = 0f;
        private bool mReady = true;
        private PlayerBonuses mBonuses;
        private IDatabaseComponentData mInitData;

        public override void Start() {
            base.Start();
            mBonuses = GetComponent<PlayerBonuses>();
        }

        public void Init(SimpleWeaponComponentData data) {
            mInitData = data;
            mOptimalDistance = data.optimalDistance;
            mDamage = data.damage;
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


        public override float GetDamage(bool isCrit) {
            if (!mUseTargetHP) {
                return mDamage;
            } else {
                if(!cachedTarget.targetObject) {
                    return 0f;
                }
                var targetDamagable = cachedTarget.targetObject.GetComponent<DamagableObject>();
                if(!targetDamagable) {
                    return 0f;
                }
                return targetDamagable.maximumHealth * mTargetHPPercent;
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
