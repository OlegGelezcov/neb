using Common;
#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif

namespace Nebula.Server.Components {
    public class PlanetObjectComponentData : ComponentData {

        public string nebulaElement { get; private set; }
        public int maxSlots { get; private set; }

        public PlanetType planetType { get; private set; }

#if UP
        public PlanetObjectComponentData(UPXElement element) {
            nebulaElement = element.GetString("nebula_element");
            maxSlots = element.GetInt("max_slots");
            if (element.HasAttribute("planet_type")) {
                planetType = (PlanetType)System.Enum.Parse(typeof(PlanetType), element.GetString("planet_type"));
            } else {
                planetType = PlanetType.Planet;
            }
        }
#else
        public PlanetObjectComponentData(XElement element) {
            nebulaElement = element.GetString("nebula_element");
            maxSlots = element.GetInt("max_slots");
            if(element.HasAttribute("planet_type")) {
                planetType = (PlanetType)System.Enum.Parse(typeof(PlanetType), element.GetString("planet_type"));
            } else {
                planetType = PlanetType.Planet;
            }
        }
#endif
        public override ComponentID componentID {
            get {
                return ComponentID.Planet;
            }
        }
    }
}
