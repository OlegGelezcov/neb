using Common;
using Nebula.Engine;
using Nebula.Server.Components;
using Space.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Nebula.Game.Components {

    public class Teleport  : NebulaBehaviour{

        protected RaceableObject myRaceable { get; set; }

        private float mRaceTimer = 60;

        private TeleportComponentData mCachedData;

        public override int behaviourId {
            get {
                return (int)ComponentID.Teleport;
            }
        }

        public void Init(TeleportComponentData data) {
            mCachedData = data;
        }

        public override void Start() {
            myRaceable = GetComponent<RaceableObject>();
            myRaceable.SetRace((nebulaObject.world as MmoWorld).ownedRace);
        }

        public override void Update(float deltaTime) {
            mRaceTimer -= deltaTime;
            if(mRaceTimer < 0f ) {
                mRaceTimer = 60;
                var world = nebulaObject.world as MmoWorld;
                if(world.ownedRace != (Race)myRaceable.race ) {
                    myRaceable.SetRace(world.ownedRace);
                }
            }
        }

        public bool CheckObject(NebulaObject obj) {
            var raceable = obj.GetComponent<RaceableObject>();
            if(!raceable) {
                return false;
            }
            if(raceable.race == myRaceable.race) {
                return true;
            }
            return false;
        }
    }
}
