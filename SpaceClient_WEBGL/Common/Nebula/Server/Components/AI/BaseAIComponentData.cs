using Common;
using ServerClientCommon;
using ExitGames.Client.Photon;
#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif

namespace Nebula.Server.Components {
    public abstract class BaseAIComponentData  : MultiComponentData {

        public bool alignWithForwardDirection { get; private set; } = true;
        public float rotationSpeed { get; private set; } = 0.5f;

#if UP
        public BaseAIComponentData(UPXElement e) {
            if (e.HasAttribute("align_with_forward_direction"))
                alignWithForwardDirection = e.GetBool("align_with_forward_direction");
            if (e.HasAttribute("rotation_speed"))
                rotationSpeed = e.GetFloat("rotation_speed");
        }
#else
        public BaseAIComponentData(XElement e) {
            if(e.HasAttribute("align_with_forward_direction"))
                alignWithForwardDirection = e.GetBool("align_with_forward_direction");
            if (e.HasAttribute("rotation_speed"))
                rotationSpeed = e.GetFloat("rotation_speed");
        }
#endif

        public BaseAIComponentData(bool inAlignWithForwardDirection, float inRotationSpeed ) {
            alignWithForwardDirection = inAlignWithForwardDirection;
            rotationSpeed = inRotationSpeed;
        }

        public BaseAIComponentData(Hashtable hash) {
            alignWithForwardDirection = hash.GetValue<bool>((int)SPC.AlignWithForwardDirection, false);
            rotationSpeed = hash.GetValue<float>((int)SPC.RotationSpeed, 0f);
        }

        public override ComponentID componentID {
            get {
                return ComponentID.CombatAI;
            }
        }
    }
}
