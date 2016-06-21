using Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Nebula.Server.Components;

namespace Nebula.Server.Nebula.Server.Components.PlanetObjects {
    public class TurretPlanetObjectComponentData : PlanetObjectBaseComponentData {
        public TurretPlanetObjectComponentData(XElement element) 
            : base(element) { }

        public TurretPlanetObjectComponentData(Hashtable hash)
            : base(hash) { }

        public TurretPlanetObjectComponentData(int row, int column, PlanetBasedObjectType objectType, string ownerId, float life, float lifeTimer)
            : base(row, column, objectType, ownerId, life, lifeTimer) { }

        public override ComponentSubType subType {
            get {
                return ComponentSubType.planet_object_turret;
            }
        }
    }
}
