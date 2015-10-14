namespace Nebula.Mmo.Items.Components {
    using System;
    using Common;

    public class MmoDamagableComponent : MmoBaseComponent {

        public float maxHealth {
            get {
                if (item != null) {
                    float h;
                    if (item.TryGetProperty<float>((byte)PS.MaxHealth, out h)) {
                        return h;
                    }
                }
                return 0;
            }
        }

        public float health {
            get {
                if (item != null) {
                    float h;
                    if (item.TryGetProperty<float>((byte)PS.CurrentHealth, out h)) {
                        return h;
                    }
                }
                return 0f;
            }
        }

        public bool destroyed {
            get {
                if (item != null) {
                    bool dest;
                    if (item.TryGetProperty<bool>((byte)PS.Destroyed, out dest)) {
                        return dest;
                    }
                }
                return false;
            }
        }

        public bool ignoreDamage {
            get {
                if (item != null) {
                    bool ignore = false;
                    if (item.TryGetProperty<bool>((byte)PS.IgnoreDamage, out ignore)) {
                        return ignore;
                    }
                }
                return false;
            }
        }

        public float ignoreDamageTimer {
            get {
                if (item != null) {
                    float timer = 0f;
                    if (item.TryGetProperty<float>((byte)PS.IgnoreDamageTimer, out timer)) {
                        return timer;
                    }
                }
                return 0f;
            }
        }

        public override ComponentID componentID {
            get {
                return ComponentID.Damagable;
            }
        }
    }
}
