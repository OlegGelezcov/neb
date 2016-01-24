using Common;
using GameMath;
using Nebula.Engine;
using Nebula.Server;
using Nebula.Server.Components;
using Space.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Nebula.Game.Components.BotAI {

    [REQUIRE_COMPONENT(typeof(MmoMessageComponent))]
    public class BaseAI  : NebulaBehaviour {


        protected MmoMessageComponent mMessage;

        public AIType aiType { get; private set; }
        protected bool mAlignWithForwardDirection = true;
        protected float mRotationSpeed = 0.5f;

        public override Hashtable DumpHash() {
            var hash = base.DumpHash();
            hash["align_with_forward_direction"] = mAlignWithForwardDirection.ToString();
            hash["rotation_speed"] = mRotationSpeed.ToString();
            return hash;
        }


        public void Init(BaseAIComponentData data) {
            mAlignWithForwardDirection = data.alignWithForwardDirection;
            mRotationSpeed = data.rotationSpeed;
        }

        public void SetAIType(AIType ai) {
            aiType = ai;
        }

        public override void Start() {
            if (aiType == null) {
                throw new System.Exception("aiType is null");
            }
            mMessage = GetComponent<MmoMessageComponent>();
        }

        public override void Update(float deltaTime) {
            OnDoIdle(deltaTime);
        }

        protected virtual void OnStartIdle(float deltaTime) {

        }

        protected virtual void OnDoIdle(float deltatTime) {

        }

        protected void Move(Vector3 oldPos, Vector3 oldRot, Vector3 pos, Vector3 rot, float speed) {

            (nebulaObject as Item).Move(pos.ToVector(), rot.ToVector());
            mMessage.PublishMove(oldPos.ToArray(), oldRot.ToArray(), transform.position.ToArray(), transform.rotation.ToArray(), speed);
        }

        public override int behaviourId {
            get {
                return (int)ComponentID.CombatAI;
            }
        }

        protected Quat ComputeRotation(Vector3 direction, float rotationSpeed, float deltaTime) {
            var newRot = (mAlignWithForwardDirection) ? Quat.Slerp(Quat.Euler(transform.rotation), Quat.LookRotation(direction), rotationSpeed * deltaTime) : Quat.Euler(transform.rotation);
            return newRot;
        }
        public static Type ResolveAI(MovingType type) {
            switch (type) {
                case MovingType.OrbitAroundPoint: return typeof(OrbitCombatAI);
                case MovingType.Patrol: return typeof(PatrolBetweenAreasCombatAI);
                case MovingType.None: return typeof(StayCombatAI);
                case MovingType.FreeFlyAtBox: return typeof(WanderCombatAI);
                case MovingType.FreeFlyNearPoint: return typeof(WanderAroundPointCombatAI);
                case MovingType.FollowPathCombat: return typeof(FollowPathCombatAI);
                case MovingType.FollowPathNonCombat: return typeof(FollowPathAI);
                default:
                    throw new Exception(string.Format("Unsupported AI type = {0}", type));
            }
        }
    }
}
