using System.Collections;
using ExitGames.Logging;
using GameMath;
using Nebula.Server;
using Nebula.Server.Components;

namespace Nebula.Game.Components.BotAI {
    public class WanderCombatAI : CombatBaseAI {

        private static ILogger log = LogManager.GetCurrentClassLogger();

        private Vector3 mAreaMin;
        private Vector3 mAreaMax;
        //private float mReachRadius;
        private Vector3 mTargetPoint;

        public override Hashtable DumpHash() {
            var hash = base.DumpHash();
            hash["area_min"] = mAreaMin.ToString();
            hash["area_max"] = mAreaMax.ToString();
            hash["target_point"] = mTargetPoint.ToString();
            return hash;
        }

        public void Init(WanderAIComponentData data) {
            base.Init(data);

            SetAIType(new FreeFlyAtBoxAIType { battleMovingType = data.battleMovingType, corners = data.corners });
            log.InfoFormat("WanderCombatAI.Init(): battle type = {0}, min = {1}, max = {2}", data.battleMovingType, data.corners.min, data.corners.max);
        }

        public override void Start() {
            base.Start();

            FreeFlyAtBoxAIType aiType = base.aiType as FreeFlyAtBoxAIType;

            mAreaMin = aiType.corners.min; //mNpcType.Settings.Value<float[]>("area_min", new float[] { 0, 0, 0 }).ToVector3();
            mAreaMax = aiType.corners.max;//mNpcType.Settings.Value<float[]>("area_max", new float[] { 0, 0, 0 }).ToVector3();
            //mReachRadius = mNpcType.Settings.Value<float>("reach_radius", 0);
            mTargetPoint = Rand.RandomBetweenTwoVectors(mAreaMin, mAreaMax);
            Move(transform.position, transform.rotation, transform.position, transform.rotation, 0);
        }

        public override void Update(float deltaTime) {
            base.Update(deltaTime);
        }

        protected override void OnDoIdle(float deltatTime) {
            Vector3 direction = mTargetPoint - transform.position;
            float distance = direction.magnitude;
            if(distance < 10) {
                mTargetPoint = Rand.RandomBetweenTwoVectors(mAreaMin, mAreaMax);
            }
            Vector3 normalDirection = direction.normalized;

            var oldPos = transform.position;
            var oldRot = transform.rotation;
            var newPos = transform.position + normalDirection * mMovable.speed * deltatTime;
            var newRot = ComputeRotation(normalDirection, mRotationSpeed, deltatTime).eulerAngles; //Quat.Slerp(Quat.Euler(transform.rotation), Quat.LookRotation(normalDirection), mRotationSpeed * deltatTime).eulerAngles;
            Move(oldPos, oldRot, newPos, newRot, mMovable.speed);
        }
    }
}
