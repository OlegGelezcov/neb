using System;
using Common;
using Nebula.Engine;
using Space.Game;
using Nebula.Server.Components;

namespace Nebula.Game.Components {
    public class AIState : NebulaBehaviour
    {
        private PlayerState prevControlState { get; set; }
        public PlayerState controlState { get; private set; }
        public ShiftKeyState shiftState { get; private set; }

        public void Init(PlayerAIStateComponentData data) { }

        public override void Start() {
            controlState = PlayerState.Idle;
            shiftState = new ShiftKeyState();
        }

        public void SetControlState(PlayerState state) {
            prevControlState = controlState;
            controlState = state;
        }

        public override void Update(float deltaTime) {
            if(shiftState.keyPressed) {
                if(controlState == PlayerState.Idle) {
                    controlState = PlayerState.MoveDirection;
                }
            }

            nebulaObject.properties.SetProperty((byte)PS.ControlState, (byte)controlState);
            nebulaObject.properties.SetProperty((byte)PS.ShiftPressed, shiftState.keyPressed);
        }

        public void SetShift(bool value) {
            if(value) {

                if (controlState != PlayerState.MoveDirection) {
                    controlState = PlayerState.MoveDirection;
                }

                shiftState.OnKeyDown(Time.curtime());

            } else {
                shiftState.OnKeyUp();
            }
        }

        public override int behaviourId {
            get {
                return (int)ComponentID.PlayerAI;
            }
        }

        
    }

}
