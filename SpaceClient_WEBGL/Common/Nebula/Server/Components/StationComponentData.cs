using Common;
#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif

namespace Nebula.Server.Components {
    public class StationComponentData : ComponentData {
#if UP
        public StationComponentData(UPXElement element) {

        }
#else
        public StationComponentData(XElement element) {

        }
#endif
        public override ComponentID componentID {
            get {
                return ComponentID.Station;
            }
        }
    }
}
