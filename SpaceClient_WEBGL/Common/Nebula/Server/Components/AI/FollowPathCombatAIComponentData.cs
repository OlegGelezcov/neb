using Common;
using GameMath;
using System;
#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif

namespace Nebula.Server.Components {
    public class FollowPathCombatAIComponentData  : CombatBaseAIComponentData {
        public Vector3[] path { get; private set; }
        public AttackMovingType battleMovingType { get; private set; }

#if UP
        public FollowPathCombatAIComponentData(UPXElement e) : base(e) {
            path = e.ToVector3List("path").ToArray();
            battleMovingType = (AttackMovingType)Enum.Parse(typeof(AttackMovingType), e.GetString("attack_moving_type"));
        }
#else
        public FollowPathCombatAIComponentData(XElement e) : base(e) {
            path = e.ToVector3List("path").ToArray();
            battleMovingType = (AttackMovingType)Enum.Parse(typeof(AttackMovingType), e.GetString("attack_moving_type"));
        }
#endif

        public FollowPathCombatAIComponentData(bool inAlignWithForwardDirection, float inRotationSpeed, Vector3[] path, AttackMovingType battleMovingType, bool useHitProbForAgro = false)
            : base(inAlignWithForwardDirection, inRotationSpeed, useHitProbForAgro) {
            this.path = path;
            this.battleMovingType = battleMovingType;
        }

        public override ComponentSubType subType {
            get {
                return ComponentSubType.ai_follow_path_combat;
            }
        }
    }
}
