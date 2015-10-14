using Common;
using System;
using System.Xml.Linq;

namespace Nebula.Server.Components {
    public class StayAIComponentData : CombatBaseAIComponentData {

        public AttackMovingType battleMovingType { get; private set; }

        public StayAIComponentData( XElement e ) : base(e) {
            battleMovingType = (AttackMovingType)Enum.Parse(typeof(AttackMovingType), e.GetString("attack_moving_type"));
        }

        public StayAIComponentData(bool inAlignWithForwardDirection, float inRotationSpeed, AttackMovingType battleMovingType, bool useHitProbForAgro = false )
            : base(inAlignWithForwardDirection, inRotationSpeed, useHitProbForAgro) {
            this.battleMovingType = battleMovingType;
        }


        public override ComponentSubType subType {
            get {
                return ComponentSubType.ai_stay;
            }
        }
    }
}
