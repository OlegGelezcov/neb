
namespace Nebula.Server.Components {
    using Common;
    using System.Xml.Linq;
    using System;
    using System.Collections;

    public class TargetComponentData : ComponentData, IDatabaseComponentData {
        public TargetComponentData(XElement e) {

        }

        public TargetComponentData(Hashtable hash) {

        }

        public TargetComponentData() { }
        public override ComponentID componentID {
            get {
                return ComponentID.Target;
            }
        }

        public Hashtable AsHash() {
            return new Hashtable();
        }
    }
}
