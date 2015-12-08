using Common;
using System.Xml.Linq;

namespace Nebula.Server.Components {
    public class PlanetObjectComponentData : ComponentData {

        public string nebulaElement { get; private set; }
        public int maxSlots { get; private set; }

        public PlanetType planetType { get; private set; }

        public PlanetObjectComponentData(XElement element) {
            nebulaElement = element.GetString("nebula_element");
            maxSlots = element.GetInt("max_slots");
            if(element.HasAttribute("planet_type")) {
                planetType = (PlanetType)System.Enum.Parse(typeof(PlanetType), element.GetString("planet_type"));
            } else {
                planetType = PlanetType.Planet;
            }
        }

        public override ComponentID componentID {
            get {
                return ComponentID.Planet;
            }
        }
    }
}
