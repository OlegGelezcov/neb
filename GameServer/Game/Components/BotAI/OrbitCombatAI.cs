using Common;
using Nebula.Server.Components;
using GameMath;
using Nebula.Server;
using ExitGames.Logging;

namespace Nebula.Game.Components.BotAI {
    public class OrbitCombatAI  : CombatBaseAI {
        private static ILogger log = LogManager.GetCurrentClassLogger();

        private Vector3 mRotationCenter;
        private float mOrbitRadius;
        private float mPhiSpeed;
        private float mThetaSpeed;

        private SphericalCoord mSphericalCoord = new SphericalCoord(0, 0, 0);

        public void Init(OrbitAIComponentData data) {
            base.Init(data);

            SetAIType(new OrbitAroundPointAIType {
                battleMovingType = data.battleMovingType,
                phiSpeed = data.phiSpeed,
                radius = data.radius,
                thetaSpeed = data.thetaSpeed
            });
            log.InfoFormat("OrbitCombatAI.Init(): battleType = {0}, phi = {1}, theta = {2}, radius = {3}",
                data.battleMovingType, data.phiSpeed, data.thetaSpeed, data.radius);
        }

        public override void Start() {
            base.Start();

            mRotationCenter = transform.position;
            OrbitAroundPointAIType aiType = base.aiType as OrbitAroundPointAIType;

            mOrbitRadius = aiType.radius; //mNpcType.Settings.Value<float>("orbit_radius");
            mPhiSpeed = aiType.phiSpeed;//mNpcType.Settings.Value<float>("phi_speed");
            mThetaSpeed = aiType.thetaSpeed;  //mNpcType.Settings.Value<float>("theta_speed");
            transform.SetPosition(transform.position + new Vector3(mOrbitRadius, 0, 0));
            Move(transform.position, transform.rotation, transform.position, transform.rotation, 0);
        }

        public override void Update(float deltaTime) {
            base.Update(deltaTime);
        }

        protected override void OnStartIdle(float deltaTime) {
            Vector3 relativePos = transform.position - mRotationCenter;
            mSphericalCoord = Geometry.Cartesian2Spherical(new CVec(relativePos.X, relativePos.Y, relativePos.Z));
            mSphericalCoord.R = mOrbitRadius;
        }

        protected override void OnDoIdle(float deltatTime) {
            mSphericalCoord.Thteta += mThetaSpeed * deltatTime;
            mSphericalCoord.Phi += mPhiSpeed * deltatTime;
            CVec vec = Geometry.Spherical2Cartesian(mSphericalCoord);
            var targetPosition = new Vector3(
                mRotationCenter.X + vec.x,
                mRotationCenter.Y + vec.y,
                mRotationCenter.Z + vec.z
                );
            var direction = (targetPosition - transform.position).normalized;
            var oldPos = transform.position;
            var oldRot = transform.rotation;
            var newPos = transform.position + direction * mMovable.speed * deltatTime;
            var newRot = ComputeRotation(direction, mRotationSpeed, deltatTime).eulerAngles; //Quat.Slerp(Quat.Euler(transform.rotation), Quat.LookRotation(direction), mRotationSpeed * deltatTime).eulerAngles;
            Move(oldPos, oldRot, newPos, newRot, mMovable.speed);
        }
    }
}
