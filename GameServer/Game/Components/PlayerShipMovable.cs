using System;
using Common;
using ExitGames.Logging;
using GameMath;
using Nebula.Engine;
using Nebula.Server.Components;

namespace Nebula.Game.Components {

    [REQUIRE_COMPONENT(typeof(PlayerShip))]
    [REQUIRE_COMPONENT(typeof(AIState))]
    [REQUIRE_COMPONENT(typeof(PlayerBonuses))]
    public class PlayerShipMovable : MovableObject{

        private AIState mAI;
        private PlayerShip mShip;
        private PlayerBonuses mBonuses;
        private float mShiftSpeedFactor;

        private static ILogger log = LogManager.GetCurrentClassLogger();

        public void Init(PlayerShipMovableComponentData data) {

        }

        public override void Start() {
            mAI = GetComponent<AIState>();
            mShip = GetComponent<PlayerShip>();
            mBonuses = GetComponent<PlayerBonuses>();
            mShiftSpeedFactor = nebulaObject.world.Resource().ServerInputs.GetValue<float>("accelerated_motion_speed_factor");
        }

        public override float normalSpeed {
            get {
                if(mShip.shipModel == null ) { return 0f; }

                return maximumSpeed;
            }
        }

        public override float maximumSpeed {
            get {
                return mShip.shipModel.speed;
            }
        }

        public override float speed {
            get {
                if (mAI.controlState == PlayerState.Idle) {
                    return 0f;
                }
                float speedFromModel = normalSpeed;
                float resultSpeed = Mathf.ClampLess(speedFromModel * (1.0f + mBonuses.speedPcBonus) + mBonuses.speedCntBonus, 0f);
                if (mAI.shiftState.keyPressed) {
                    resultSpeed *= mShiftSpeedFactor;
                }
                return resultSpeed;
            }
        }

        public override void Update(float deltaTime) {
            base.Update(deltaTime);
            //if(GetComponent<MmoActor>()) {
            //    log.InfoFormat("player speed = {0}", speed);
            //}
        }

        //public override void Update(float deltaTime) {
        //    nebulaObject.properties.SetProperty((byte)PS.CurrentLinearSpeed, speed);
        //    nebulaObject.properties.SetProperty((byte)PS.MaxLinearSpeed, speed);
        //}
    }
}
