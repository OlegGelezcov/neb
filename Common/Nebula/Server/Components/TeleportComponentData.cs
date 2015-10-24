﻿using Common;
using System.Xml.Linq;
using System;
using System.Collections;

namespace Nebula.Server.Components {
    public class TeleportComponentData : MultiComponentData {
        public TeleportComponentData(XElement e) {

        }
        public TeleportComponentData() { }

        public TeleportComponentData(Hashtable hash) {

        }

        public override ComponentID componentID {
            get {
                return ComponentID.Teleport;
            }
        }

        public override ComponentSubType subType {
            get {
                return ComponentSubType.SimpleTeleport;
            }
        }
    }
}
