using Common;
using GameMath;
using System;
using System.Xml.Linq;

namespace Nebula.Server.Components {
    public class WanderAIComponentData : CombatBaseAIComponentData {

        public MinMax corners;
        public AttackMovingType battleMovingType { get; private set; }

        public WanderAIComponentData(XElement e) : base(e)  {
            Vector3 min = e.GetFloatArray("min").ToVector3();
            Vector3 max = e.GetFloatArray("max").ToVector3();
            corners = new MinMax(min, max);
            battleMovingType = (AttackMovingType)Enum.Parse(typeof(AttackMovingType), e.GetString("attack_moving_type"));
        }

        public WanderAIComponentData(bool inAlignWithForwardDirection, float inRotationSpeed, MinMax corners, AttackMovingType battleMovingType, bool useHitProbForAgro = false)
            : base(inAlignWithForwardDirection, inRotationSpeed, useHitProbForAgro) {
            this.corners = corners;
            this.battleMovingType = battleMovingType;
        }

        public override ComponentSubType subType {
            get {
                return ComponentSubType.ai_wander_cube;
            }
        }
    }
}
