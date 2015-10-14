namespace Nebula.Mmo.Items.Components {
    using UnityEngine;
    using System.Collections;
    using Common;
    using System;

    public class MmoMovableComponent : MmoBaseComponent {
        public float speed {
            get {
                if (item != null) {
                    float sp = 0f;
                    if (item.TryGetProperty<float>((byte)PS.CurrentLinearSpeed, out sp)) {
                        return sp;
                    }
                }
                return 0f;
            }
        }

        public float maxSpeed {
            get {
                if (item != null) {
                    float sp = 0f;
                    if (item.TryGetProperty<float>((byte)PS.MaxLinearSpeed, out sp)) {
                        return sp;
                    }
                }
                return 0f;
            }
        }

        public override ComponentID componentID {
            get {
                return ComponentID.Movable;
            }
        }
    }
}
