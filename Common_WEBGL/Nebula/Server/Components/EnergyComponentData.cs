using Common;
using System.Xml.Linq;

namespace Nebula.Server.Components {
    public class EnergyComponentData : ComponentData {

        public EnergyComponentData(XElement data) {

        }

        public EnergyComponentData() { }

        public override ComponentID componentID {
            get {
                return ComponentID.Energy;
            }
        }
    }
}
