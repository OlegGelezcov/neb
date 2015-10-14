using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Space.Game
{
    public class PlayerShipEnergyBlock
    {
        private MmoActor actor;
        private float energy;
        private float maxEnergy;
        private float energyTouchTime;
        private float energyRestorationSpeed;
        private Dictionary<string, float> energyBuffs;

        public PlayerShipEnergyBlock(MmoActor actor) {
            this.actor = actor;
            this.energyBuffs = new Dictionary<string,float>();
        }

        public void Initialize(float maxEnergy, float energy, float energyRestorationSpeed) {
            SetBaseMaxEnergy(maxEnergy);
            SetEnergy(energy);
            this.energyRestorationSpeed = energyRestorationSpeed;
            this.energyTouchTime = Time.time;
        }

        public void Touch() {
            float delta = Time.time - energyTouchTime;
            float de = delta * energyRestorationSpeed;
            energyTouchTime = Time.time;
            SetEnergy(energy + de);
        }

        private void SetActorProperty(string group, string prop, object val) {
            if (actor != null)
                actor.SetProperty(group, prop, val);
        }

        private float TotalBuf {
            get {
                float total = 0.0f;
                foreach (var pair in energyBuffs) {
                    total += pair.Value;
                }
                return total;
            }
        }

        public float Energy {
            get {
                Touch();
                return energy;
            }
        }

        public float MaxEnergy {
            get {
                float total = TotalBuf;
                return Mathf.Clamp(maxEnergy + total, 0.0f, Math.Max(0.0f, maxEnergy + total));
            }
        }

        public void SetEnergyBuf(string id, float buf) {
            if (energyBuffs.ContainsKey(id))
            {
                energyBuffs[id] = buf;
            }
            else 
            {
                energyBuffs.Add(id, buf);
            }
        }

        public void RemoveEnergyBuf(string id) {
            energyBuffs.Remove(id);
        }

        public void SetEnergy(float e) {
            float me = MaxEnergy;
            energy = Mathf.Clamp(e, 0.0f, me);
            SetActorProperty(GroupProps.SHIP_BASE_STATE, Props.SHIP_BASE_STATE_ENERGY, energy);
            SetActorProperty(GroupProps.SHIP_BASE_STATE, Props.SHIP_BASE_STATE_MAX_ENERGY, me);
        }

        public void SetBaseMaxEnergy(float me) {
            maxEnergy = me;
        }

        public bool EnoughAdd(float e) {
            float meEnergy = Energy;
            if (e <= meEnergy) {
                SetEnergy(meEnergy - e);
                return true;
            }
            return false;
        }
    }
}
