using Common;
#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif

namespace Nebula.Server.Components {
    public class SpawnPiratesActivatorComponentData : ActivatorComponentData {

        public int pirateCount { get; private set; }

#if UP
        public SpawnPiratesActivatorComponentData(UPXElement e) : base(e) {
            pirateCount = e.GetInt("pirate_count");
        }
#else
        public SpawnPiratesActivatorComponentData(XElement e) : base(e) {
            pirateCount = e.GetInt("pirate_count");
        }
#endif
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
