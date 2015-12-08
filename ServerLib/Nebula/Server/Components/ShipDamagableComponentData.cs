using Common;
using System.Xml.Linq;

namespace Nebula.Server.Components {
    public class ShipDamagableComponentData : MultiComponentData {

        public bool createChestOnKilling { get; private set; }

        public ShipDamagableComponentData(XElement e) {
            if (e.HasAttribute("create_chest_when_killed"))
                createChestOnKilling = e.GetBool("create_chest_when_killed");
            else
                createChestOnKilling = true;
        }

        public ShipDamagableComponentData(bool createChestOnKilling) {
            this.createChestOnKilling = createChestOnKilling;
        }

        public override ComponentID componentID {
            get {
                return ComponentID.Damagable;
            }
        }

        public override ComponentSubType subType {
            get {
                return ComponentSubType.damagable_ship;
            }
        }
    }
}
