using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Game.Components {

    /// <summary>
    /// Describe bot state in god mode
    /// </summary>
    public class GodState {
        private const float MAX_TIME_INTERVAL_IN_GOD_MODE = 60.0f;
        private float mGodTimeoutTimer = 0f;

        private bool mGod = false;

        public void SetGod(bool value) {
            mGod = value;
            if(mGod ) {
                mGodTimeoutTimer = MAX_TIME_INTERVAL_IN_GOD_MODE;
            }
        }

        public void Update(float deltaTime) {
            if(mGod) {
                mGodTimeoutTimer -= deltaTime;
                if(mGodTimeoutTimer <= 0f ) {
                    SetGod(false);
                }
            }
        }

        public bool god {
            get {
                return mGod;
            }
        }
    }
}
