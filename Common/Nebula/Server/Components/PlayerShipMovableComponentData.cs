using Common;

namespace Nebula.Server.Components {
    public class PlayerShipMovableComponentData : MultiComponentData {

        public override ComponentID componentID {
            get {
                return ComponentID.Movable;
            }
        }

        public override ComponentSubType subType {
            get {
                return ComponentSubType.player_ship_movable;
            }
        }
    }
}
