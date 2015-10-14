using System;
using GameMath;
using Nebula.Engine;
using Nebula.Server.Components;

namespace Nebula.Game.Components {

    [REQUIRE_COMPONENT(typeof(BaseShip))]
    [REQUIRE_COMPONENT(typeof(PlayerBonuses))]
    public class ShipMovable : MovableObject {

        private BaseShip mShip;
        private PlayerBonuses mBonuses;
        private PassiveBonusesComponent mPassiveBonuses;

        public void Init(BotShipMovableComponentData data ) { }

        public override void Start() {
            mShip = GetComponent<BaseShip>();
            mBonuses = GetComponent<PlayerBonuses>();
            mPassiveBonuses = GetComponent<PassiveBonusesComponent>();
        }

        public override float normalSpeed {
            get {
                if(stopped) {
                    return 0;
                }
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
                if(stopped) {
                    return 0;
                }
                float speedFromModel = normalSpeed;
                float resultSpeed = speedFromModel;
                if (mBonuses) {
                    resultSpeed = Mathf.ClampLess(speedFromModel * (1.0f + mBonuses.speedPcBonus) + mBonuses.speedCntBonus, 0f);
                }
                resultSpeed = ApplyPassiveBonuses(resultSpeed);

                return resultSpeed;
            }
        }

        private float ApplyPassiveBonuses(float speed) {
            if(nebulaObject.IsPlayer()) {
                if(mPassiveBonuses != null && mPassiveBonuses.speedTier > 0 ) {
                    return speed * (1.0f + mPassiveBonuses.speedBonus);
                }
            }
            return speed;
        }
    }
}
