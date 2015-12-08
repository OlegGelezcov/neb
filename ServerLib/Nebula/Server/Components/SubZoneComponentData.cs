using Common;
using System.Xml.Linq;

namespace Nebula.Server.Components {
    public class SubZoneComponentData : ComponentData {
        public float innerRadius { get; private set; }
        public float outerRadius { get; private set; }
        public int subZoneID { get; private set; }

        public SubZoneComponentData(XElement e) {
            innerRadius = e.GetFloat("inner_radius");
            outerRadius = e.GetFloat("outer_radius");
            subZoneID = e.GetInt("subzone_id");
        }

        public override ComponentID componentID {
            get {
                return ComponentID.SubZone;
            }
        }
    }
}
