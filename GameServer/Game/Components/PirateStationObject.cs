using Common;
using GameMath;
using Nebula.Engine;
using ServerClientCommon;
using Space.Game;
using Space.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Game.Components {

    [REQUIRE_COMPONENT(typeof(DamagableObject))]
    [REQUIRE_COMPONENT(typeof(MmoMessageComponent))]
    public class PirateStationObject : NebulaBehaviour{
        public enum PirateStationState { Idle, Dead }
        private BaseFSM<PirateStationState> fsm;

        private DamagableObject mDamagable;
        private MmoMessageComponent mMessage;

        private bool destroyFlag = false;
        private float destroyTimer = 5f;



        public override void Start() {

            this.fsm = new BaseFSM<PirateStationState>();
            this.fsm.AddState(new FSMState<PirateStationState>(PirateStationState.Idle, ()=> { }, ()=> { }, ()=> { }));
            this.fsm.AddState(new FSMState<PirateStationState>(PirateStationState.Dead, ()=> { }, ()=> { }, ()=> { }));
            this.fsm.ForceState(PirateStationState.Idle, true);

            mDamagable = RequireComponent<DamagableObject>();
            mMessage = RequireComponent<MmoMessageComponent>();
            Move(nebulaObject.transform.position, nebulaObject.transform.rotation);
            (nebulaObject as Item).UpdateInterestManagement();
        }

        public override void Update(float deltaTime) {
            bool destroyed = nebulaObject;

            nebulaObject.properties.SetProperty((byte)PS.Ship, new Hashtable {
                {(int)SPC.MaxHealth, mDamagable.maximumHealth },
                {(int)SPC.Health, mDamagable.health },
                {(int)SPC.Destroyed, destroyed }
            });
            nebulaObject.properties.SetProperty((byte)PS.CurrentHealth, mDamagable.health);

            if (fsm.IsState(PirateStationState.Dead)) {
                if(!destroyFlag) {
                    destroyTimer -= deltaTime;
                    if(destroyTimer <= 0f ) {
                        destroyFlag = true;
                        (nebulaObject as Item).Destroy();
                    }
                }
            }
        }

        private void Move(Vector3 pos, Vector3 rot) {
            var oldPos = nebulaObject.transform.position;
            var oldRot = nebulaObject.transform.rotation;
            (nebulaObject as Item).Move(pos.ToVector(), rot.ToVector());
            mMessage.PublishMove(oldPos.ToArray(), oldRot.ToArray(), pos.ToArray(), rot.ToArray(), 0);
        }

        public void Death() {
            if(!fsm.IsState(PirateStationState.Dead)) {
                fsm.GotoState(PirateStationState.Dead);
            }
        }

        public override int behaviourId {
            get {
                return (int)ComponentID.PirateStation;
            }
        }

    }
}
