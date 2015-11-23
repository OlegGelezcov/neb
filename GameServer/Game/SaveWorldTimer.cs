using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Game {
    public class SaveWorldTimer {
        private const float SAVE_WORLD_INTERVAL = 120;
        private float mTimer = 0f;

        public bool Update(float deltaTime) {
            mTimer += deltaTime;
            if(mTimer >= SAVE_WORLD_INTERVAL) {
                mTimer = 0f;
                return true;
            }
            return false;
        }
    }
}
