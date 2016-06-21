using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using Nebula.Server.Components;
using System.Xml.Linq;
using System.Collections;

namespace Nebula.Server.Nebula.Server.Components.PlanetObjects {
    public class CommandCenterPlanetObjectComponentData : PlanetObjectBaseComponentData {

        public CommandCenterPlanetObjectComponentData(XElement element)
            : base(element) { }

        public CommandCenterPlanetObjectComponentData(Hashtable hash)
            : base(hash) { }

        public CommandCenterPlanetObjectComponentData(int row, int column, PlanetBasedObjectType objectType, string ownerId, float life, float lifeTimer)
            : base(row, column, objectType, ownerId, life, lifeTimer) { }

        public override ComponentSubType subType {
            get {
                return ComponentSubType.planet_object_command_center;
            }
        }
    }
}
