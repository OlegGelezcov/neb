using Common;
using System;
using System.Xml.Linq;
using System.Collections;
using ServerClientCommon;

namespace Nebula.Server.Components {
    public class FreeFlyNearPointComponentData : CombatBaseAIComponentData, IDatabaseComponentData  {

        public float radius { get; private set; }
        public AttackMovingType battleMovingType { get; private set; }

        public FreeFlyNearPointComponentData(XElement e) : base(e) {
            radius = e.GetFloat("radius");
            battleMovingType = (AttackMovingType)Enum.Parse(typeof(AttackMovingType), e.GetString("attack_moving_type"));
        }

        public FreeFlyNearPointComponentData(bool inAlignWithForwardDirection, float inRotationSpeed, float radius, AttackMovingType battleMovingType, bool useHitProbForAgro = false)
            : base(inAlignWithForwardDirection, inRotationSpeed, useHitProbForAgro) {
            this.radius = radius;
            this.battleMovingType = battleMovingType;
        }

        public FreeFlyNearPointComponentData(Hashtable hash) : base(hash) {
            radius = hash.GetValue<float>((int)SPC.Radius, 0f);
            battleMovingType = (AttackMovingType)hash.GetValue<int>((int)SPC.AttackMovingType, (int)AttackMovingType.AttackStay);
        }


        public override ComponentSubType subType {
            get {
                return ComponentSubType.ai_wander_point;
            }
        }

        public Hashtable AsHash() {
            return new Hashtable {
                {(int)SPC.AlignWithForwardDirection, alignWithForwardDirection },
                {(int)SPC.RotationSpeed, rotationSpeed  },
                {(int)SPC.Radius, radius },
                {(int)SPC.AttackMovingType, (int)battleMovingType },
                {(int)SPC.UseHitProbForAgro, useHitProbForAgro },
                {(int)SPC.SubType, (int)subType }
            };
        }
    }
}
