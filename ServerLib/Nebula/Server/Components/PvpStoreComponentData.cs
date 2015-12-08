using Common;
using System.Xml.Linq;

namespace Nebula.Server.Components {
    public class PvpStoreComponentData : ComponentData {

        public PvpStoreComponentData(XElement e) { }

        public PvpStoreComponentData() { }

        public override ComponentID componentID {
            get {
                return ComponentID.PvpStore;
            }
        }
    }
}
