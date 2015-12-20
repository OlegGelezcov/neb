using Common;
#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif


namespace Nebula.Server.Components {
    public class SubZoneComponentData : ComponentData {
        public float innerRadius { get; private set; }
        public float outerRadius { get; private set; }
        public int subZoneID { get; private set; }
#if UP
        public SubZoneComponentData(UPXElement e) {
            innerRadius = e.GetFloat("inner_radius");
            outerRadius = e.GetFloat("outer_radius");
            subZoneID = e.GetInt("subzone_id");
        }
#else
        public SubZoneComponentData(XElement e) {
            innerRadius = e.GetFloat("inner_radius");
            outerRadius = e.GetFloat("outer_radius");
            subZoneID = e.GetInt("subzone_id");
        }
#endif
        public override ComponentID componentID {
            get {
                return ComponentID.SubZone;
            }
        }
    }
}
