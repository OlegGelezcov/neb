using Common;
using Nebula.Engine;
using Space.Game.Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Nebula.Game.Components {
    public abstract class BaseShip : NebulaBehaviour {

        public ShipModel shipModel { get; private set; }

        protected readonly BlockResistState blockResist = new BlockResistState();

        public abstract float damageResistance { get; }
        public abstract int holdCapacity { get; }

        public override Hashtable DumpHash() {
            var hash = base.DumpHash();
            hash["model"] = (shipModel != null) ? shipModel.GetInfo() : new Hashtable();
            hash["resist_blocked?"] = blockResist.blocked.ToString();
            if(shipModel != null ) {
                hash["model_hp"] = shipModel.hp.ToString();
                hash["model_capacity"] = shipModel.cargo.ToString();
                hash["model_speed"] = shipModel.speed.ToString();
                hash["model_resist"] = shipModel.resistance.ToString();
                hash["model_damage_bonus"] = shipModel.damageBonus.ToString();
                hash["model_energy_bonus"] = shipModel.energyBonus.ToString();
                hash["model_crit_chance"] = shipModel.critChance.ToString();
                hash["model_crit_damage_bonus"] = shipModel.critDamageBonus.ToString();
            }
            return hash;
        }

        public override int behaviourId {
            get {
                return (int)ComponentID.Ship;
            }
        }


        public virtual void SetModule(ShipModule module, out ShipModule prevModule) {
            this.shipModel.SetModule(module, out prevModule);
            this.shipModel.Update();
        }

        public void SetModel(ShipModel inShipModel) {
            shipModel = inShipModel;
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
