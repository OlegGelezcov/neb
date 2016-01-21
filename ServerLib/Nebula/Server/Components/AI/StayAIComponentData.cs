using Common;
using ServerClientCommon;
using System;
using System.Collections;
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

        public StayAIComponentData(Hashtable hash)
            : base(hash) {
            battleMovingType = (AttackMovingType)hash.GetValue<int>((int)SPC.AttackMovingType, (int)AttackMovingType.AttackStay);
        }

        public override ComponentSubType subType {
            get {
                return ComponentSubType.ai_stay;
            }
        }

        public Hashtable AsHash() {
            return new Hashtable {
                { (int)SPC.AlignWithForwardDirection, alignWithForwardDirection },
                { (int)SPC.RotationSpeed, rotationSpeed },
                { (int)SPC.AttackMovingType, (int)battleMovingType },
                { (int)SPC.UseHitProbForAgro, useHitProbForAgro },
                { (int)SPC.SubType, (int)subType }
            };
        }
    }
}
