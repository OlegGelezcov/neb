using Common;
using ExitGames.Client.Photon;
#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif

namespace Nebula.Server.Components {
    public class TurretComponentData : ComponentData, IDatabaseComponentData {
#if UP
        public TurretComponentData(UPXElement e) { }
#else
        public TurretComponentData(XElement e) { }
#endif
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
