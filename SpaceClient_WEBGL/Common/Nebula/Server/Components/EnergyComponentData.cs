using Common;
#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif

namespace Nebula.Server.Components {
    public class EnergyComponentData : ComponentData {
#if UP
        public EnergyComponentData(UPXElement data) {

        }
#else
        public EnergyComponentData(XElement data) {

        }
#endif
        public EnergyComponentData() { }

        public override ComponentID componentID {
            get {
                return ComponentID.Energy;
            }
        }
    }
}
