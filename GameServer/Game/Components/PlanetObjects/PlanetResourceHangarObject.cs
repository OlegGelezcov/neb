using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;

namespace Nebula.Game.Components.PlanetObjects {
    public class PlanetResourceHangarObject : PlanetObjectBase {

        public override PlanetBasedObjectType objectType {
            get {
                return PlanetBasedObjectType.ResourceHangar;
            }
        }
    }
}
