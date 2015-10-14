using Common;
using ExitGames.Logging;
using GameMath;
using Nebula.Server;
using Nebula.Server.Components;

namespace Nebula.Game.Components.BotAI {

    public class PatrolBetweenAreasCombatAI : CombatBaseAI {

        private static ILogger log = LogManager.GetCurrentClassLogger();

        private Vector3 mFirstCenter;
        private Vector3 mSecondCenter;
        //private float mReachRadius;
        private int mDirection;
        private Vector3 mTargetPoint;

        public void Init(PatrolAIComponentData data) {
            base.Init(data);

            SetAIType(new PatrolAIType { battleMovingType = data.battleMovingType, firstPoint = data.firstPoint, secondPoint = data.secondPoint });
            log.InfoFormat("PatrolBetweenAreasCombatAI.Init(): battle type = {0}, first point = {1}, second point = {2}",
                data.battleMovingType, data.firstPoint.ToString(), data.secondPoint.ToString());
        }


        public override void Start() {
            base.Start();

            PatrolAIType aiType = base.aiType as PatrolAIType;

            mFirstCenter = aiType.firstPoint; //mNpcType.Settings.Value<float[]>("first_area_center", new float[] { 0, 0, 0 }).ToVector3();
            mSecondCenter = aiType.secondPoint; ///mNpcType.Settings.Value<float[]>("second_area_center", new float[] { 0, 0, 0 }).ToVector3();
            //mReachRadius = mNpcType.Settings.Value<float>("patrol_area_reach_radius", 0f);
            mDirection = 1;
            ToggleTargetPoint();
            transform.SetPosition(mTargetPoint);
            Move(transform.position, transform.rotation, transform.position, transform.rotation, 0);
        }

        public override void Update(float deltaTime) {
            base.Update(deltaTime);
        }

        private void ToggleTargetPoint() {
            if(mDirection == 0 ) {
                mDirection = 1;
            } else {
                mDirection = 0;
            }
            if(mDirection == 0) {
                mTargetPoint = mSecondCenter + Rand.Float01() * 10 * CommonUtils.RandomUnitVector3().ToVector3();
            } else {
                mTargetPoint = mFirstCenter + Rand.Float01() * 10 * CommonUtils.RandomUnitVector3().ToVector3();
            }  
        }

        protected override void OnDoIdle(float deltatTime) {
            var direction = mTargetPoint - transform.position;
            float distance = direction.magnitude;
            if (distance < 10 ) {
                ToggleTargetPoint();
            }

            var normalDirection = direction.normalized;
            var oldPos = transform.position;
            var oldRot = transform.rotation;
            var newPos = transform.position + normalDirection * mMovable.speed * deltatTime;
            var newRot = ComputeRotation(normalDirection, mRotationSpeed, deltatTime).eulerAngles; //Quat.Slerp(Quat.Euler(transform.rotation), Quat.LookRotation(normalDirection), mRotationSpeed * deltatTime).eulerAngles;
            Move(oldPos, oldRot, newPos, newRot, mMovable.speed);
        }

    }
}
