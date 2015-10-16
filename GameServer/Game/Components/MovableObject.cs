﻿using Common;
using Nebula.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Game.Components {

    public abstract class MovableObject : NebulaBehaviour {


        public abstract float speed { get; }

        private float mStopTimer = -1;

        /// <summary>
        /// Normal speed of ship based on ship model and sets, don't include additional modificators from controls or bonuses
        /// </summary>
        public abstract float normalSpeed { get; }

        public abstract float maximumSpeed { get;  }

        public override int behaviourId {
            get {
                return (int)ComponentID.Movable;
            }
        }

        public override void Update(float deltaTime) {

            if(nebulaObject.IAmBotAndNoPlayers()) {
                return;
            }


            nebulaObject.properties.SetProperty((byte)PS.CurrentLinearSpeed, speed);
            nebulaObject.properties.SetProperty((byte)PS.MaxLinearSpeed, speed);

            if(mStopTimer >= 0f ) {
                mStopTimer -= deltaTime;
            }
        }

        public void SetStopTimer(float t) {
            mStopTimer = t;
        }

        public bool stopped {
            get {
                return mStopTimer >= 0f;
            }
        }

    }

}