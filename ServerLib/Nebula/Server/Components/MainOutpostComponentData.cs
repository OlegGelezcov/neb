using Common;
using System.Xml.Linq;
using System;
using System.Collections;

namespace Nebula.Server.Components {
    public class MainOutpostComponentData : ComponentData, IDatabaseComponentData{

        public MainOutpostComponentData(XElement e) { }

        public MainOutpostComponentData() { }

        public MainOutpostComponentData(Hashtable hash) { }

        public override ComponentID componentID {
            get {
                return ComponentID.MainOutpost;
            }
        }

        public Hashtable AsHash() {
            return new Hashtable();
        }
    }
}
