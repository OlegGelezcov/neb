using Common;
using System.Xml.Linq;

namespace Nebula.Server.Components {
    public class AsteroidComponentData : ComponentData {

        public string dataID { get; private set; }

        public AsteroidComponentData(XElement e) {
            dataID = e.GetString("data_id");
        }

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
