using Common;
#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif

namespace Nebula.Server.Components {
    public class BotShipMovableComponentData : MultiComponentData {

#if UP
        public BotShipMovableComponentData(UPXElement e) {

        }
#else
        public BotShipMovableComponentData(XElement e) {

        }
#endif
        public BotShipMovableComponentData() {

        }

        public override ComponentID componentID {
            get {
                return ComponentID.Movable;
            }
        }

        public override ComponentSubType subType {
            get {
                return ComponentSubType.bot_ship_movable;
            }
        }
    }
}
