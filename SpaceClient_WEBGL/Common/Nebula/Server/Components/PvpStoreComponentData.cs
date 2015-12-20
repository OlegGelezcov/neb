using Common;
#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif

namespace Nebula.Server.Components {
    public class PvpStoreComponentData : ComponentData {

#if UP
        public PvpStoreComponentData(UPXElement e) { }
#else
        public PvpStoreComponentData(XElement e) { }
#endif

        public PvpStoreComponentData() { }

        public override ComponentID componentID {
            get {
                return ComponentID.PvpStore;
            }
        }
    }
}
