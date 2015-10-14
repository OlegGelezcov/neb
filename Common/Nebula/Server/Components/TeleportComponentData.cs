using Common;
using System.Xml.Linq;

namespace Nebula.Server.Components {
    public class TeleportComponentData : ComponentData {
        public TeleportComponentData(XElement e) {

        }
        public TeleportComponentData() { }

        public override ComponentID componentID {
            get {
                return ComponentID.Teleport;
            }
        }
    }
}
