using ExitGames.Logging;
using GameMath;
using Nebula.Server;
using Nebula.Server.Components;
using System.Collections;

namespace Nebula.Game.Components.BotAI {

    public class FollowPathCombatAI  : CombatBaseAI {
        private static ILogger log = LogManager.GetCurrentClassLogger();
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

        public void Init(FollowPathCombatAIComponentData data) {
            base.Init(data);
            SetAIType(new FollowPathAIType { battleMovingType = data.battleMovingType, path = data.path });
            //log.InfoFormat("FollowPathCombatAI.Init(): path length = {0}", data.path.Length);
        }

        public override void Start() {
            base.Start();
            FollowPathAIType ai = aiType as FollowPathAIType;
            path = ai.path;
            targetIndex = 0;
            Move(transform.position, transform.rotation, transform.position, transform.rotation, 0);
        }

        public override void Update(float deltaTime) {
            base.Update(deltaTime);
        }

        protected override void OnDoIdle(float deltatTime) {
            if(path == null || path.Length == 0) {
                log.InfoFormat("no path at = {0}", nebulaObject.Id);
                return;
            }

            Vector3 direction = path[targetIndex] - transform.position;
            float distance = direction.magnitude;
            UpdateDistanceEPS(deltatTime);

            if( distance < mDistanceEPS ) {
                UpdateTargetIndex();
            }

            Vector3 nDirection = direction.normalized;
            var oldPos = transform.position;
            var oldRot = transform.rotation;
            var newPos = transform.position + nDirection * mMovable.speed * deltatTime;
            var newRot = ComputeRotation(nDirection, mRotationSpeed, deltatTime).eulerAngles; //Quat.Slerp(Quat.Euler(transform.rotation), Quat.LookRotation(nDirection), mRotationSpeed * deltatTime).eulerAngles;
            Move(oldPos, oldRot, newPos, newRot, mMovable.speed);
        }

        private void UpdateTargetIndex() {
            targetIndex++;
            if(targetIndex >= path.Length) {
                targetIndex = 0;
            }
        }

        private void UpdateDistanceEPS( float deltaTime ) {
            if(mMovable ) {
                if(mMovable.speed > 0f ) {
                    mDistanceEPS = mMovable.speed * 2f * deltaTime;
                    return;
                }
            }
            mDistanceEPS = 10f;
        }
    }
}
