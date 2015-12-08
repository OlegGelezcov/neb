using Common;
using System.Xml.Linq;

namespace Nebula.Server.Components {
    public abstract class ActivatorComponentData : MultiComponentData {

        public float cooldown { get; private set; }
        public float radius { get; private set; }

        public ActivatorComponentData(XElement e) {
            cooldown = e.GetFloat("cooldown");
            radius = e.GetFloat("radius");
        }

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
