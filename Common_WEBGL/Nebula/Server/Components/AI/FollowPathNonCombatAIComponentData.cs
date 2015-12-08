using Common;
using GameMath;
using System.Xml.Linq;

namespace Nebula.Server.Components {
    public class FollowPathNonCombatAIComponentData  : BaseAIComponentData {
        public Vector3[] path { get; private set; }
        

        public FollowPathNonCombatAIComponentData(XElement e) : base(e) {
            path = e.ToVector3List("path").ToArray();
        }

        public FollowPathNonCombatAIComponentData(bool inAlignWithForwardDirection, float inRotationSpeed, Vector3[] path) 
            : base(inAlignWithForwardDirection, inRotationSpeed) {
            this.path = path;
        }

        public override ComponentSubType subType {
            get {
                return ComponentSubType.ai_follow_path_non_combat;
            }
        }
    }
}
