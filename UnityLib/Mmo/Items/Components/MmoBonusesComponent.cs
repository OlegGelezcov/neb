namespace Nebula.Mmo.Items.Components {
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using Common;
    using System;

    public class MmoBonusesComponent : MmoBaseComponent {

        private const float UPDATE_BONUS_LIST_INTERVAL = 2.0f;

        private float timer = UPDATE_BONUS_LIST_INTERVAL;
        private float lastUpdate = 0;

        private readonly Dictionary<BonusType, float> mBonuses = new Dictionary<BonusType, float>();

        public Dictionary<BonusType, float> bonuses {
            get {
                return mBonuses;
            }
        }

        public override ComponentID componentID {
            get {
                return ComponentID.Bonuses;
            }
        }




        public void ClearBonuses() {
            mBonuses.Clear();
        }

        public void AddBonus(BonusType bonusType, float bonusValue) {
            mBonuses.Add(bonusType, bonusValue);
        }

        public void UpdateBonusList() {

            float t = Time.time;
            float dt = t - lastUpdate;
            lastUpdate = t;
            timer -= dt;

            if (timer <= 0f) {
                timer = 2f;

                Hashtable bonusHash = null;
                item.TryGetProperty<Hashtable>((byte)PS.Bonuses, out bonusHash);
                if (bonusHash == null) {
                    return;
                }

                mBonuses.Clear();

                foreach (DictionaryEntry entry in bonusHash) {
                    int bonusType = (int)entry.Key;
                    float bonusValue = (float)entry.Value;
                    mBonuses.Add((BonusType)(byte)bonusType, bonusValue);
                }
            }
        }
    }
}
