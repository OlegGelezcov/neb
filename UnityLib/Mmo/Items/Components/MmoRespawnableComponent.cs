using System;
using Common;

namespace Nebula.Mmo.Items.Components {
    public class MmoRespawnableComponent : MmoBaseComponent {
        public override ComponentID componentID {
            get {
                return ComponentID.Respawnable;
            }
        }
    }
}
