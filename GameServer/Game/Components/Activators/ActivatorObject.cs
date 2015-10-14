using System;
using Common;
using ExitGames.Logging;
using Nebula.Engine;
using Space.Game;
using Nebula.Server.Components;

namespace Nebula.Game.Components.Activators {
    public abstract class ActivatorObject : NebulaBehaviour {

        private float mCooldown;
        private float mRadius;
        private float mCooldownTimer;
        private bool mActive;

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public void Init(ActivatorComponentData data) {
            mCooldown = data.cooldown;
            mRadius = data.radius;
            mCooldownTimer = 10;
            mActive = false;
        }

        public override int behaviourId {
            get {
                return (int)ComponentID.Activator;
            }
        }

        public override void Update(float deltaTime) {
            nebulaObject.properties.SetProperty((byte)PS.LightCooldown, cooldown);
            nebulaObject.properties.SetProperty((byte)PS.Radius, radius);
            nebulaObject.properties.SetProperty((byte)PS.Active, active);

            if(!active) {
                if (mCooldownTimer > 0f) {
                    mCooldownTimer -= deltaTime;
                }

                if(mCooldownTimer <= 0f ) {
                    if(CheckActivate()) {
                        Activate();
                    }
                }
            }

            if(active ) {
                if(CheckDeactivate()) {
                    Deactivate();
                }
            }
        }

        protected virtual bool active {
            get {
                return mActive;
            }
        }

        protected virtual bool CheckActivate() {

            var items = (nebulaObject.world as MmoWorld).GetItems(it => it.Type == (byte)ItemType.Avatar);

            foreach(var pair in items) {
                if(nebulaObject.transform.DistanceTo(pair.Value.transform) < mRadius ) {
                    return true;
                }
            }

            return false;
        }

        protected virtual void Activate() {
            SetActive(true);
            GetComponent<MmoMessageComponent>().SendActivatorEvent(active);
        }

        protected abstract bool CheckDeactivate();

        protected virtual void Deactivate() {
            SetCooldownTimer(cooldown);
            SetActive(false);
            GetComponent<MmoMessageComponent>().SendActivatorEvent(active);
        }


        protected void SetCooldownTimer(float interval ) {
            mCooldownTimer = interval;
        }

        protected float cooldown {
            get {
                return mCooldown;
            }
        }

        protected float radius {
            get {
                return mRadius;
            }
        }

        protected void SetActive(bool val) {
            mActive = val;
        }
    }
}
