using Common;
#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif

namespace Nebula.Server.Components {
    public class ShipDamagableComponentData : MultiComponentData {

        public bool createChestOnKilling { get; private set; }

#if UP
        public ShipDamagableComponentData(UPXElement e) {
            if (e.HasAttribute("create_chest_when_killed"))
                createChestOnKilling = e.GetBool("create_chest_when_killed");
            else
                createChestOnKilling = true;
        }
#else
        public ShipDamagableComponentData(XElement e) {
            if (e.HasAttribute("create_chest_when_killed"))
                createChestOnKilling = e.GetBool("create_chest_when_killed");
            else
                createChestOnKilling = true;
        }
#endif
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
