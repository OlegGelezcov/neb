namespace Nebula.Mmo.Items.Components {
    using UnityEngine;
    using System.Collections;
    using Common;
    using System;

    public class MmoPlayerAIComponent : MmoBaseComponent {
        public override ComponentID componentID {
            get {
                return ComponentID.PlayerAI;
            }
        }

        public PlayerState controlState {
            get {
                if (item != null) {
                    byte controlState;
                    if (item.TryGetProperty<byte>((byte)PS.ControlState, out controlState)) {
                        return (PlayerState)controlState;
                    }
                }
                return PlayerState.Idle;
            }
        }

        public bool shiftPressed {
            get {
                if (item != null) {
                    bool pressed = false;
                    if (item.TryGetProperty<bool>((byte)PS.ShiftPressed, out pressed)) {
                        return pressed;
                    }
                }
                return false;
            }
        }
    }
}
