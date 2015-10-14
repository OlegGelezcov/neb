namespace Nebula.Mmo.Items.Components {
    using UnityEngine;
    using System.Collections;
    using Common;
    using System;

    public class MmoSubZoneComponent : MmoBaseComponent {

        public const int SUB_ZONE_TO_CHILD_ZONE_START_ID = 1000;

        public int subZoneID {
            get {
                int subZone = 0;
                if (item != null) {
                    if (item.TryGetProperty<int>((byte)PS.SubZoneID, out subZone)) {
                        return subZone;
                    }
                }
                return 0;
            }
        }

        public float innerRadius {
            get {
                float innerRadius = 0;
                if (item != null) {
                    if (item.TryGetProperty<float>((byte)PS.InnerRadius, out innerRadius)) {
                        return innerRadius;
                    }
                }
                return 0f;
            }
        }

        public float outerRadius {
            get {
                float outerRadius = 0f;
                if (item != null) {
                    if (item.TryGetProperty<float>((byte)PS.OuterRadius, out outerRadius)) {
                        return outerRadius;
                    }
                }
                return 0f;
            }
        }

        public override ComponentID componentID {
            get {
                return ComponentID.SubZone;
            }
        }
    }

}