namespace Nebula.Mmo.Items.Components {
    using UnityEngine;
    using System.Collections;
    using Common;
    using System;

    public class MmoRaceableComponent : MmoBaseComponent {
        public override ComponentID componentID {
            get {
                return ComponentID.Raceable;
            }
        }

        public Race race {
            get {
                byte bRace;
                if (item != null) {
                    if (item.TryGetProperty<byte>((byte)PS.Race, out bRace)) {
                        return (Race)bRace;
                    }
                } else {
                    Debug.LogError("RACEABLE ITEM NULL");
                }

                return Race.None;
            }
        }
    }
}
