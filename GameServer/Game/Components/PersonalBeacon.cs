using System;
using System.Collections;
using Common;
using Nebula.Server.Components;
using Space.Server;

namespace Nebula.Game.Components {

    /// <summary>
    /// Presonal beacon make only created from consumable item
    /// </summary>
    public class PersonalBeacon : Teleport, IDatabaseObject  {

        private float mTimer;
        private IDatabaseComponentData mInitData;

        public void Init(PersonalBeaconComponentData data) {
            mInitData = data;
            base.Init(data);
            mTimer = data.time;
        }
        public override void Start() {
            myRaceable = GetComponent<RaceableObject>();
            nebulaObject.properties.SetProperty((byte)PS.ConstructionTimer, mTimer);
        }

        public override void Update(float deltaTime) {

            //destriy beacon when expire time
            if(mTimer > 0) {
                mTimer -= deltaTime;
                nebulaObject.properties.SetProperty((byte)PS.ConstructionTimer, mTimer);
                if(mTimer <= 0f ) {
                    (nebulaObject as Item).Destroy();
                }
            }

        }

        public Hashtable GetDatabaseSave() {
            if(mInitData != null ) {
                return mInitData.AsHash();
            }
            return new Hashtable();
        }
    }
}
