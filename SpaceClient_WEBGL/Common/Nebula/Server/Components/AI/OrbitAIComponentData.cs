using Common;
using System;
#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif

namespace Nebula.Server.Components {
    public class OrbitAIComponentData  : CombatBaseAIComponentData {

        public float phiSpeed { get; private set; }
        public float thetaSpeed { get; private set; }
        public float radius { get; private set; }
        public AttackMovingType battleMovingType { get; private set; }
#if UP
        public OrbitAIComponentData(UPXElement e) : base(e) {
            phiSpeed = e.GetFloat("phi_speed");
            thetaSpeed = e.GetFloat("theta_speed");
            radius = e.GetFloat("radius");
            battleMovingType = (AttackMovingType)Enum.Parse(typeof(AttackMovingType), e.GetString("attack_moving_type"));
        }
#else
        public OrbitAIComponentData(XElement e) : base(e) {
            phiSpeed = e.GetFloat("phi_speed");
            thetaSpeed = e.GetFloat("theta_speed");
            radius = e.GetFloat("radius");
            battleMovingType = (AttackMovingType)Enum.Parse(typeof(AttackMovingType), e.GetString("attack_moving_type"));
        }
#endif
        public OrbitAIComponentData(bool inAlignWithForwardDirection, float inRotationSpeed, float phiSpeed, float thetaSpeed, float radius, AttackMovingType battleMovingType,
            bool useHitProbForAgro  = false )
            : base(inAlignWithForwardDirection, inRotationSpeed, useHitProbForAgro) {
            this.phiSpeed = phiSpeed;
            this.thetaSpeed = thetaSpeed;
            this.radius = radius;
            this.battleMovingType = battleMovingType;
        }

        public override ComponentSubType subType {
            get {
                return ComponentSubType.ai_orbit;
            }
        }


    }
}
