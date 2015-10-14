using Common;
using Nebula.Engine;
using Space.Game.Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Game.Components {
    public abstract class BaseShip : NebulaBehaviour {
        public ShipModel shipModel { get; private set; }

        protected readonly BlockResistState blockResist = new BlockResistState();

        public virtual void SetModule(ShipModule module, out ShipModule prevModule) {
            this.shipModel.SetModule(module, out prevModule);
            this.shipModel.Update();
        }

        public void SetModel(ShipModel inShipModel) {
            shipModel = inShipModel;
        } 


        public abstract float damageResistance { get; }
        public abstract int holdCapacity { get; }

        public override int behaviourId {
            get {
                return (int)ComponentID.Ship;
            }
        }

        public override void Update(float deltaTime) {
            base.Update(deltaTime);
            blockResist.Update(deltaTime);
        }

        public void BlockResist(float interval) {
            blockResist.Block(interval);
        }

        public class BlockResistState {
            public bool blocked { get; private set; }

            private float mTimer;

            public void Update(float deltaTime) {
                if(blocked) {
                    if(mTimer > 0f) {
                        mTimer -= deltaTime;
                        if(mTimer <= 0f ) {
                            blocked = false;
                        }
                    } else {
                        blocked = false;
                    }
                }
            }

            public void Block(float interval) {
                mTimer = interval;
                blocked = true;
            }
        }
    }


}
