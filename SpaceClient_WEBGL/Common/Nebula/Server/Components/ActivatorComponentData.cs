using Common;
#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif

namespace Nebula.Server.Components {
    public abstract class ActivatorComponentData : MultiComponentData {

        public float cooldown { get; private set; }
        public float radius { get; private set; }
#if UP
        public ActivatorComponentData(UPXElement e) {
            cooldown = e.GetFloat("cooldown");
            radius = e.GetFloat("radius");
        }
#else
        public ActivatorComponentData(XElement e) {
            cooldown = e.GetFloat("cooldown");
            radius = e.GetFloat("radius");
        }
#endif

        public ActivatorComponentData(float inCooldown, float inRadius) {
            cooldown = inCooldown;
            radius = inRadius;
        }

        public override ComponentID componentID {
            get {
                return ComponentID.Activator;
            }
        }
    }
}
