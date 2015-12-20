using Common;
using System;
#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif

namespace Nebula.Server.Components {
    public class StayAIComponentData : CombatBaseAIComponentData {

        public AttackMovingType battleMovingType { get; private set; }
#if UP
        public StayAIComponentData(UPXElement e) : base(e) {
            battleMovingType = (AttackMovingType)Enum.Parse(typeof(AttackMovingType), e.GetString("attack_moving_type"));
        }
#else
        public StayAIComponentData( XElement e ) : base(e) {
            battleMovingType = (AttackMovingType)Enum.Parse(typeof(AttackMovingType), e.GetString("attack_moving_type"));
        }
#endif
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
