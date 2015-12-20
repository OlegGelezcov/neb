using Common;
using GameMath;
#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif

namespace Nebula.Server.Components {
    public class FollowPathNonCombatAIComponentData  : BaseAIComponentData {
        public Vector3[] path { get; private set; }

#if UP
        public FollowPathNonCombatAIComponentData(UPXElement e) : base(e) {
            path = e.ToVector3List("path").ToArray();
        }
#else
        public FollowPathNonCombatAIComponentData(XElement e) : base(e) {
            path = e.ToVector3List("path").ToArray();
        }
#endif
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
