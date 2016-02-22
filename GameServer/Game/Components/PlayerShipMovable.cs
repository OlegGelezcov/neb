using System;
using System.Collections;
using Common;
using ExitGames.Logging;
using Nebula.Engine;
using Nebula.Server.Components;
using ServerClientCommon;

namespace Nebula.Game.Components {

    [REQUIRE_COMPONENT(typeof(PlayerShip))]
    [REQUIRE_COMPONENT(typeof(AIState))]
    [REQUIRE_COMPONENT(typeof(PlayerBonuses))]
    public class PlayerShipMovable : ShipMovable {

        private readonly static ILogger log = LogManager.GetCurrentClassLogger();

        private AIState mAI;
        private float mShiftSpeedFactor;
        private PassiveBonusesComponent m_PassiveBonuses;

        public void Init(PlayerShipMovableComponentData data) {}

        public override void Start() {
            base.Start();
            mAI = GetComponent<AIState>();
            mShiftSpeedFactor = nebulaObject.world.Resource().ServerInputs.GetValue<float>("accelerated_motion_speed_factor");
            m_PassiveBonuses = GetComponent<PassiveBonusesComponent>();
        }

        public override float normalSpeed {
            get {
                if(ship.shipModel == null ) {
                    return 0f;
                }
                return base.normalSpeed;
            }
        }

        protected override void UpdateDetail() {
            base.UpdateDetail();
            m_SpeedDetail.SetControlMult(mAI.controlState);
            m_SpeedDetail.SetPassiveAbilitiesAdd(GetSpeedWithPassiveBonus(m_SpeedDetail.total) - m_SpeedDetail.total);
            m_SpeedDetail.SetAccelerationEffect(GetShiftSpeed(m_SpeedDetail.total) - m_SpeedDetail.total);
        }

        private float GetSpeedWithPassiveBonus(float inputSpeed) {
            if (m_PassiveBonuses != null && m_PassiveBonuses.speedTier > 0) {
                inputSpeed *= (1.0f + m_PassiveBonuses.speedBonus);
            }
            return inputSpeed;
        }

        private float GetShiftSpeed(float inputSpeed) {
            if(mAI.shiftState.keyPressed) {
                return inputSpeed * mShiftSpeedFactor;
            }
            return inputSpeed;
        }

        public override void Update(float deltaTime) {
            base.Update(deltaTime);
        }

        public Hashtable GetSpeedDetail() {
            return m_SpeedDetail.GetInfo();
        }
    }

    public class SpeedDetail : IInfoSource {
        public int controlMult { get; private set; }
        public int stopMult { get; private set; }
        public float modelSpeed { get; private set; }
        public float bonusesAdd { get; private set; }
        public float passiveAbilitiesAdd { get; private set; }
        public float accelerationAdd { get; private set; }

        private float m_Total = 0;

        private void UpdateTotalSpeed() {
            m_Total = controlMult * stopMult * (modelSpeed + bonusesAdd + passiveAbilitiesAdd + accelerationAdd);
        }

        public Hashtable GetInfo() {
            return new Hashtable {
                { (int)SPC.SPEED_ControlIsMoving, controlMult },
                { (int)SPC.SPEED_IsStopped, stopMult },
                { (int)SPC.SPEED_ModelSpeed, modelSpeed },
                { (int)SPC.SPEED_BonusesAdd, bonusesAdd },
                { (int)SPC.SPEED_PassiveAbilitiesAdd, passiveAbilitiesAdd },
                { (int)SPC.SPEED_AccelerationAdd, accelerationAdd }
            };
        }

        public void Reset() {
            controlMult = 1;
            stopMult = 1;
            modelSpeed = 0;
            bonusesAdd = 0;
            passiveAbilitiesAdd = 0;
            accelerationAdd = 0;
        }

        public void SetModelSpeed(float sp) {
            modelSpeed = sp;
            UpdateTotalSpeed();
        } 
        public void SetBonusAdd(float add) {
            bonusesAdd = add;
            UpdateTotalSpeed();
        }
        public void SetPassiveAbilitiesAdd(float add) {
            passiveAbilitiesAdd = add;
            UpdateTotalSpeed();
        } 
        public void SetAccelerationEffect(float add ) {
            accelerationAdd = add;
            UpdateTotalSpeed();
        }
        public void SetStopMult(bool stopped) {
            if(stopped) {
                stopMult = 0;
            } else {
                stopMult = 1;
            }
            UpdateTotalSpeed();
        }

        public void SetControlMult(PlayerState state) {
            if(state == PlayerState.Idle) {
                controlMult = 0;
            } else {
                controlMult = 1;
            }
            UpdateTotalSpeed();
        }



        public float total {
            get {
                return m_Total;
            }
        }
    }
}
