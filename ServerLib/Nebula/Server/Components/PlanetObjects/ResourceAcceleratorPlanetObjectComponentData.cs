using Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Nebula.Server.Components;

namespace Nebula.Server.Nebula.Server.Components.PlanetObjects {
    public class ResourceAcceleratorPlanetObjectComponentData : PlanetObjectBaseComponentData {
        public ResourceAcceleratorPlanetObjectComponentData(XElement element)
            : base(element) { }

        public ResourceAcceleratorPlanetObjectComponentData(Hashtable hash)
            : base(hash) { }

        public ResourceAcceleratorPlanetObjectComponentData(int row, int column, PlanetBasedObjectType objectType, string ownerId, float life, float lifeTimer,
            string characterId, string characterName, string coalitionName)
            : base(row, column, objectType, ownerId, life, lifeTimer, characterId, characterName, coalitionName) { }

        public override ComponentSubType subType {
            get {
                return ComponentSubType.planet_object_resource_accelerator;
            }
        }
    }
}
