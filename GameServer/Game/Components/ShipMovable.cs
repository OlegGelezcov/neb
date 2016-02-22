using System;
using System.Collections;
using GameMath;
using Nebula.Engine;
using Nebula.Server.Components;

namespace Nebula.Game.Components {

    [REQUIRE_COMPONENT(typeof(BaseShip))]
    [REQUIRE_COMPONENT(typeof(PlayerBonuses))]
    public class ShipMovable : MovableObject {

        private BaseShip m_Ship;
        private PlayerBonuses m_Bonuses;
        protected readonly SpeedDetail m_SpeedDetail = new SpeedDetail();


        public void Init(BotShipMovableComponentData data ) { }

        public override void Start() {
            m_SpeedDetail.Reset();
            m_Ship = GetComponent<BaseShip>();
            m_Bonuses = GetComponent<PlayerBonuses>();
            
        }

        public override Hashtable DumpHash() {
            var hash = base.DumpHash();
            hash["normal_speed"] = normalSpeed.ToString();
            hash["maximum_speed"] = maximumSpeed.ToString();
            hash["speed"] = speed.ToString();
            return hash;
        }

        public override float normalSpeed {
            get {
                return maximumSpeed;
            }
        }

        public override float maximumSpeed {
            get {
                return ship.shipModel.speed;
            }
        }

        public override float speed {
            get {
                UpdateDetail();
                return m_SpeedDetail.total;
            }
        }

        protected virtual void UpdateDetail() {
            m_SpeedDetail.Reset();
            m_SpeedDetail.SetStopMult(stopped);
            m_SpeedDetail.SetModelSpeed(normalSpeed);
            m_SpeedDetail.SetBonusAdd(GetSpeedWithBonuses(m_SpeedDetail.modelSpeed) - m_SpeedDetail.modelSpeed);

        }

        protected float GetSpeedWithBonuses(float inputSpeed) {
            return Mathf.ClampLess(inputSpeed * (1.0f + bonuses.speedPcBonus) + bonuses.speedCntBonus, 0.0f);
        }


        protected BaseShip ship {
            get {
                return m_Ship;
            }
        }

        protected PlayerBonuses bonuses {
            get {
                return m_Bonuses;
            }
        }
    }
}
