namespace Nebula.Mmo.Items.Components {
    using UnityEngine;
    using System.Collections;
    using Common;
    using System;

    public class MmoEnergyComponent : MmoBaseComponent {

        private const float UPDATE_ENERGY_INTERVAL = 2.0f;
        private float mUpdateTimer = UPDATE_ENERGY_INTERVAL;
        private float mLastUpdateTime = 0;

        private float mCurrentEnergy = 0f;
        private float mMaxEnergy = 0f;

        public override ComponentID componentID {
            get {
                return ComponentID.Energy;
            }
        }

        public float currentEnergy {
            get {
                return mCurrentEnergy;
            }
        }

        public float maxEnergy {
            get {
                return mMaxEnergy;
            }
        }

        public void UpdateEnergyValues() {
            float deltaTime = Time.time - mLastUpdateTime;
            mUpdateTimer -= deltaTime;

            if (mUpdateTimer <= 0f) {
                mLastUpdateTime = Time.time;
                mUpdateTimer = UPDATE_ENERGY_INTERVAL;

                item.TryGetProperty<float>((byte)PS.CurrentEnergy, out mCurrentEnergy);
                item.TryGetProperty<float>((byte)PS.MaxEnergy, out mMaxEnergy);
            }
        }
    }
}