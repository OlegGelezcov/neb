namespace Nebula.Server.Components {
    using Common;
    using System.Xml.Linq;
    using System;
    using System.Collections;

    public class OutpostComponentData : ComponentData, IDatabaseComponentData  {
        public OutpostComponentData(XElement e) {

        }

        public OutpostComponentData() { }
        public OutpostComponentData(Hashtable hash) { }
        public override ComponentID componentID {
            get {
                return ComponentID.Outpost;
            }
        }

        public Hashtable AsHash() {
            return new Hashtable();
        }
    }
}
