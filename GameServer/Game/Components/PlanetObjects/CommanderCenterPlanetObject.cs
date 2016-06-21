using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using ServerClientCommon;

namespace Nebula.Game.Components.PlanetObjects {
    public class CommanderCenterPlanetObject : PlanetObjectBase {

        public override PlanetBasedObjectType objectType {
            get {
                return PlanetBasedObjectType.CommanderCenter;
            }
        }
    }
}
