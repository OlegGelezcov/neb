﻿using Common;
using System.Xml.Linq;
using System;
using System.Collections;

namespace Nebula.Server.Components {
    public class TurretComponentData : ComponentData, IDatabaseComponentData {

        public TurretComponentData(XElement e) { }

        public TurretComponentData() { }

        public TurretComponentData(Hashtable hash) { }

        public override ComponentID componentID {
            get {
                return ComponentID.Turret;
            }
        }

        public Hashtable AsHash() {
            return new Hashtable();
        }
    }
}
