using Common;
using System.Xml.Linq;

namespace Nebula.Server.Components {
    public class StationComponentData : ComponentData {
        public StationComponentData(XElement element) {

        }

        public override ComponentID componentID {
            get {
                return ComponentID.Station;
            }
        }
    }
}
