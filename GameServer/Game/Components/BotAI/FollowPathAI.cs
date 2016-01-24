using System.Collections;
using ExitGames.Logging;
using GameMath;
using Nebula.Engine;
using Nebula.Server;
using Nebula.Server.Components;

namespace Nebula.Game.Components.BotAI {

    [REQUIRE_COMPONENT(typeof(MovableObject))]
    public class FollowPathAI : BaseAI  {
        private static ILogger log = LogManager.GetCurrentClassLogger();

        private MovableObject mMovable;

        private Vector3[] path;
        private int targetIndex;
        private float mDistanceEPS = 10f;

        public override Hashtable DumpHash() {
            var hash = base.DumpHash();
            hash["path_count"] = (path != null) ? path.Length.ToString() : "0";
            hash["target_path_index"] = targetIndex.ToString();
            hash["distance_epsilon"] = mDistanceEPS.ToString();
            return hash;
        }

        public void Init(FollowPathNonCombatAIComponentData data) {
            base.Init(data);
            SetAIType(new FollowPathNonCombatAIType { path = data.path });
            log.InfoFormat("FollowPathAI.Init(): path length = {0}", data.path.Length);
        }

        public override void Start() {
            base.Start();
            mMovable = GetComponent<MovableObject>();

            FollowPathNonCombatAIType ai = aiType as FollowPathNonCombatAIType;
            path = ai.path;
            targetIndex = 0;
            Move(transform.position, transform.rotation, transform.position, transform.rotation, 0);
        }

        public override void Update(float deltaTime) {
            base.Update(deltaTime);
        }

        protected override void OnDoIdle(float deltatTime) {
            Vector3 direction = path[targetIndex] - transform.position;
            float distance = direction.magnitude;
            UpdateDistanceEPS(deltatTime);

            if (distance < mDistanceEPS) {
                UpdateTargetIndex();
            }

            Vector3 nDirection = direction.normalized;
            var oldPos = transform.position;
            var oldRot = transform.rotation;
            var newPos = transform.position + nDirection * mMovable.speed * deltatTime;
            var newRot = ComputeRotation(nDirection, mRotationSpeed, deltatTime).eulerAngles; //Quat.Slerp(Quat.Euler(transform.rotation), Quat.LookRotation(nDirection), ROTATION_SPEED * deltatTime).eulerAngles;
            Move(oldPos, oldRot, newPos, newRot, mMovable.speed);
        }

        private void UpdateTargetIndex() {
            targetIndex++;
            if (targetIndex >= path.Length) {
                targetIndex = 0;
            }
        }

        private void UpdateDistanceEPS(float deltaTime) {
            if (mMovable.speed > 0f) {
                mDistanceEPS = mMovable.speed * 2f * deltaTime;
                return;
            }
            mDistanceEPS = 10f;
        }
    }
}
