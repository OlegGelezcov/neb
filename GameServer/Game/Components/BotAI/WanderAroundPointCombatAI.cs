using Nebula.Server.Components;
using ExitGames.Logging;
using GameMath;
using Nebula.Server;
using System;
using System.Collections;

namespace Nebula.Game.Components.BotAI {
    public class WanderAroundPointCombatAI : CombatBaseAI, IDatabaseObject{

        private static ILogger log = LogManager.GetCurrentClassLogger();

        private Vector3 mSpawnPosition;
        private float mRadius;
        //private float mReachDistance;
        private Vector3 mIdlePoint;
        private IDatabaseComponentData mInitData;

        public override Hashtable DumpHash() {
            var hash = base.DumpHash();
            hash["spawn_position"] = mSpawnPosition.ToString();
            hash["radius"] = mRadius.ToString();
            hash["idle_point"] = mIdlePoint.ToString();
            return hash;
        }

        public void Init(FreeFlyNearPointComponentData data) {
            mInitData = data;
            base.Init(data);

            SetAIType(new FreeFlyNearPointAIType { battleMovingType = data.battleMovingType, radius = data.radius });
            //log.InfoFormat("WanderAoundPointCombatAI.Init(): battle type = {0}, radius = {1}", data.battleMovingType, data.radius);
        }

        public override void Start() {
            base.Start();

            mSpawnPosition = transform.position;
            FreeFlyNearPointAIType aiType = base.aiType as FreeFlyNearPointAIType;
            mRadius = aiType.radius; //mNpcType.Settings.Value<float>("wander_radius");
            //mReachDistance = mNpcType.Settings.Value<float>("reach_distance");
            //log.InfoFormat("wander radius = {0},  center = {1}", mRadius, mSpawnPosition);

            mIdlePoint = mSpawnPosition + Rand.UnitVector3() * mRadius;
            transform.SetPosition(mIdlePoint);
            mIdlePoint = mSpawnPosition + Rand.UnitVector3() * mRadius;

            Move(transform.position, transform.rotation, transform.position, transform.rotation, 0);
        }
        public override void Update(float deltaTime) {
            base.Update(deltaTime);
        }

        protected override void OnDoIdle(float deltatTime) {
            var direction = mIdlePoint - transform.position;
            float oldDistance = direction.magnitude;
            //if(distance < mReachDistance) {
            //    mIdlePoint = mSpawnPosition + Rand.UnitVector3() * mRadius;
            //    log.InfoFormat("change target point to = {0}", mIdlePoint);
            //}
            var normalDirection = direction.normalized;

            var oldPos = transform.position;
            var oldRot = transform.rotation;
            var newPos = transform.position + normalDirection * mMovable.speed * deltatTime;

            var newRot = ComputeRotation(normalDirection, mRotationSpeed, deltatTime).eulerAngles; //Quat.Slerp(Quat.Euler(transform.rotation), Quat.LookRotation(normalDirection), mRotationSpeed * deltatTime).eulerAngles;

            float newDistance = (newPos - mIdlePoint).magnitude;
            if(newDistance > oldDistance) {
                mIdlePoint = mSpawnPosition + Rand.UnitVector3() * mRadius;
                //log.InfoFormat("change target point to = {0}", mIdlePoint);
            }
            Move(oldPos, oldRot, newPos, newRot, mMovable.speed);
        }

        public Hashtable GetDatabaseSave() {
            if (mInitData != null) {
                return mInitData.AsHash();
            }
            return new Hashtable();
        }
    }
}
