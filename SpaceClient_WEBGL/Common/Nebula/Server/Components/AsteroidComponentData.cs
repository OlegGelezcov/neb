using Common;
#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif

namespace Nebula.Server.Components {
    public class AsteroidComponentData : ComponentData {

        public string dataID { get; private set; }

#if UP
        public AsteroidComponentData(UPXElement e) {
            dataID = e.GetString("data_id");
        }
#else
        public AsteroidComponentData(XElement e) {
            dataID = e.GetString("data_id");
        }
#endif
        public AsteroidComponentData(string inDataID) {
            dataID = inDataID;
        }

        public override ComponentID componentID {
            get {
                return ComponentID.Asteroid;
            }
        }
    }
}
