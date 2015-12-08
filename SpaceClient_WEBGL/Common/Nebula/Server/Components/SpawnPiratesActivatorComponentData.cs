using Common;
using System.Xml.Linq;

namespace Nebula.Server.Components {
    public class SpawnPiratesActivatorComponentData : ActivatorComponentData {

        public int pirateCount { get; private set; }

        public SpawnPiratesActivatorComponentData(XElement e) : base(e) {
            pirateCount = e.GetInt("pirate_count");
        }

        public SpawnPiratesActivatorComponentData(float inCooldown, float inRadius, int pirateCount)
            : base(inCooldown, inRadius) {
            this.pirateCount = pirateCount;
        }

        public override ComponentSubType subType {
            get {
                return ComponentSubType.spawn_pirate_activator;
            }
        }
    }
}
