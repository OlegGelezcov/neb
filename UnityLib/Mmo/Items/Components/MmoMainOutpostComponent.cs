namespace Nebula.Mmo.Items.Components {
    using UnityEngine;
    using System.Collections;
    using Common;
    using System;

    public class MmoMainOutpostComponent : MmoBaseComponent {
        public override ComponentID componentID {
            get {
                return ComponentID.MainOutpost;
            }
        }
    }
}
