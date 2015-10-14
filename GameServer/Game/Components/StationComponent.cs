using Common;
using Nebula.Engine;
using Nebula.Server.Components;
using Space.Game;

namespace Nebula.Game.Components {
    [REQUIRE_COMPONENT(typeof(RaceableObject))]
    [REQUIRE_COMPONENT(typeof(ModelComponent))]
    public class StationComponent : NebulaBehaviour {

        private const float UPDATE_RACE_INTERVAL = 30f;
         
        private RaceableObject mRaceable;
        private float mUpdateRaceTimer = UPDATE_RACE_INTERVAL;

        public void Init(StationComponentData data) {

        }

        public override void Start() {
            mRaceable = GetComponent<RaceableObject>();
        }
        public override void Update(float deltaTime) {
            mUpdateRaceTimer -= deltaTime;
            if(mUpdateRaceTimer <= 0f ) {
                mUpdateRaceTimer = UPDATE_RACE_INTERVAL;

                var worldRace = (nebulaObject.world as MmoWorld).ownedRace;
                if(worldRace != (Race)mRaceable.race) {
                    mRaceable.SetRace(worldRace);
                }
            }
        }

        public override int behaviourId {
            get {
                return (int)ComponentID.Station;
            }
        }
    }
}
