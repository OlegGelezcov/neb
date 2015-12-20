using Common;
using GameMath;
using System;
#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif

namespace Nebula.Server.Components {
    public class PatrolAIComponentData : CombatBaseAIComponentData  {

        public Vector3 firstPoint { get; private set; }
        public Vector3 secondPoint { get; private set; }
        public AttackMovingType battleMovingType { get; private set; }
#if UP
        public PatrolAIComponentData(UPXElement e) : base(e) {
            firstPoint = e.GetFloatArray("first_point").ToVector3();
            secondPoint = e.GetFloatArray("second_point").ToVector3();
            battleMovingType = (AttackMovingType)Enum.Parse(typeof(AttackMovingType), e.GetString("attack_moving_type"));
        }
#else
        public PatrolAIComponentData(XElement e) : base(e) {
            firstPoint = e.GetFloatArray("first_point").ToVector3();
            secondPoint = e.GetFloatArray("second_point").ToVector3();
            battleMovingType = (AttackMovingType)Enum.Parse(typeof(AttackMovingType), e.GetString("attack_moving_type"));
        }
#endif
        public PatrolAIComponentData(bool inAlignWithForwardDirection, float inRotationSpeed, Vector3 firstPoint, Vector3 secondPoint, AttackMovingType battleMovingType,
            bool useHitProbForAgro )
            : base(inAlignWithForwardDirection, inRotationSpeed, useHitProbForAgro) {
            this.firstPoint = firstPoint;
            this.secondPoint = secondPoint;
            this.battleMovingType = battleMovingType;
        }

        public override ComponentSubType subType {
            get {
                return ComponentSubType.ai_patrol;
            }
        }
    }
}
