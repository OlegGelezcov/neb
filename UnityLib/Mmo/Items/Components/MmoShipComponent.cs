namespace Nebula.Mmo.Items.Components {
    using UnityEngine;
    using System.Collections;
    using Common;
    using System;

    public class MmoShipComponent : MmoBaseComponent {

        public Hashtable modelInfo {
            get {
                if (item != null) {
                    Hashtable modelHash;
                    if (item.TryGetProperty<Hashtable>((byte)PS.ModelInfo, out modelHash)) {
                        return modelHash;
                    }
                }
                return new Hashtable();
            }
        }


        /// <summary>
        /// Don't use this will be removed later
        /// </summary>
        public Hashtable modulePrefabs {
            get {
                if (item != null) {
                    Hashtable mPrefabs;
                    if (item.TryGetProperty<Hashtable>((byte)PS.ModulePrefabs, out mPrefabs)) {
                        return mPrefabs;
                    }
                }
                return new Hashtable();
            }
        }

        public float angleSpeed {
            get {
                if (item != null) {
                    float aSpeed = 0f;
                    if (item.TryGetProperty<float>((byte)PS.AngleSpeed, out aSpeed)) {
                        return aSpeed;
                    }
                }
                return 0f;
            }
        }

        public float acceleration {
            get {
                if (item != null) {
                    float acc = 0f;
                    if (item.TryGetProperty<float>((byte)PS.Acceleration, out acc)) {
                        return acc;
                    }
                }
                return 0f;
            }
        }

        public override ComponentID componentID {
            get {
                return ComponentID.Ship;
            }
        }
    }
}
