namespace Nebula.Mmo.Items.Components {
    using UnityEngine;
    using System.Collections;
    using Common;
    using System;

    public class MmoWeaponComponent : MmoBaseComponent {
        public float optimalDistance {
            get {
                if (item != null) {
                    float dist = 0f;
                    if (item.TryGetProperty<float>((byte)PS.OptimalDistance, out dist)) {
                        return dist;
                    }

                }
                return 0f;
            }
        }

        public Difficulty difficulty {
            get {

                if (item != null) {
                    byte bDifficulty;
                    if (item.TryGetProperty<byte>((byte)PS.Difficulty, out bDifficulty)) {
                        return (Difficulty)bDifficulty;
                    }
                }
                return Difficulty.medium;
            }
        }

        public override ComponentID componentID {
            get {
                return ComponentID.Weapon;
            }
        }
    }
}
