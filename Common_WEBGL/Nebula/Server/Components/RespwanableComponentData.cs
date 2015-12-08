using Common;
using System.Xml.Linq;

namespace Nebula.Server.Components {
    public class RespwanableComponentData : ComponentData  {
        public float interval { get; private set; }

        public RespwanableComponentData(XElement e) {
            interval = e.GetFloat("interval");
        }

        public RespwanableComponentData(float interval) {
            this.interval = interval;
        }

        public override ComponentID componentID {
            get {
                return ComponentID.Respawnable;
            }
        }
    }
}
