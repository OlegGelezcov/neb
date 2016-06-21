using Nebula.Server.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using System.Xml.Linq;

namespace Nebula.Server.Nebula.Server.Components {
    public class PlanetLandingTeleportData : ComponentData {

        public string planetId { get; private set; } = string.Empty;

        public PlanetLandingTeleportData(XElement element) {
            planetId = element.GetString("planet_id");
        }

        public override ComponentID componentID {
            get {
                return ComponentID.PlanetLandingTeleport;
            }
        }
    }
}
