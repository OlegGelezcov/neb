using Common;
using ExitGames.Client.Photon;
#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif

namespace Nebula.Server.Components {
    public class TeleportComponentData : MultiComponentData {
#if UP
        public TeleportComponentData(UPXElement e) {

        }
#else
        public TeleportComponentData(XElement e) {

        }
#endif
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
