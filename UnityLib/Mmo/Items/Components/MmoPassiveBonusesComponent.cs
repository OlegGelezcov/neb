using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;

namespace Nebula.Mmo.Items.Components {
    public class MmoPassiveBonusesComponent : MmoBaseComponent {
        public override ComponentID componentID {
            get {
                return ComponentID.PassiveBonuses;
            }
        }
    }
}
