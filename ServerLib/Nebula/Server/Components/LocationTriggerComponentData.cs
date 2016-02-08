using Common;
using System.Xml.Linq;

namespace Nebula.Server.Components {
    /// <summary>
    /// Trigger object init data (radius and trigger name)
    /// </summary>
    public class LocationTriggerComponentData : ComponentData {

        public float radius { get; private set; }
        public string name { get; private set; }

        public LocationTriggerComponentData(XElement element) {
            radius = element.GetFloat("radius");
            name = element.GetString("name");
        }

        public override ComponentID componentID {
            get {
                return ComponentID.Trigger;
            }
        }
    }
}
