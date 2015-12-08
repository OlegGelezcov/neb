using Common;
using System.Xml.Linq;

namespace Nebula.Server.Components {
    public class BotShipMovableComponentData : MultiComponentData {

        public BotShipMovableComponentData(XElement e) {

        }

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
