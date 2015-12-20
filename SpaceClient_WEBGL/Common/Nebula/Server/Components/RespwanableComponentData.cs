using Common;
#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif

namespace Nebula.Server.Components {
    public class RespwanableComponentData : ComponentData  {
        public float interval { get; private set; }

#if UP
        public RespwanableComponentData(UPXElement e) {
            interval = e.GetFloat("interval");
        }
#else
        public RespwanableComponentData(XElement e) {
            interval = e.GetFloat("interval");
        }
#endif

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
