namespace Nebula.Mmo.Items.Components {
    using UnityEngine;
    using System.Collections;
    using Common;
    using System;

    public class MmoActivatorComponent : MmoBaseComponent {

        public float cooldown {
            get {
                if (item == null) { return 0f; }
                float c;
                if (item.TryGetProperty<float>((byte)PS.LightCooldown, out c)) {
                    return c;
                }
                return 0f;
            }
        }

        public float radius {
            get {
                if (item == null) { return 0f; }
                float r;
                if (item.TryGetProperty<float>((byte)PS.Radius, out r)) {
                    return r;
                }
                return 0f;
            }
        }

        public bool active {
            get {
                if (item == null) { return false; }
                bool a;
                if (item.TryGetProperty<bool>((byte)PS.Active, out a)) {
                    return a;
                }
                return false;
            }
        }

        public override ComponentID componentID {
            get {
                return ComponentID.Activator;
            }
        }
    }
}
