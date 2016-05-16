using System;
using Common;
using Nebula.Engine;
using Nebula.Server.Components;
using Space.Game;
using ExitGames.Logging;
using System.Collections;

namespace Nebula.Game.Components {
    public class RaceableObject : NebulaBehaviour, IDatabaseObject {

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        private RaceableComponentData mInitData;

        public byte race { get; private set; }


        public override void Start() {
            UpdateProperty();
        }

        public void Init(RaceableComponentData data) {
            mInitData = data;
            SetRace(data.race);
        }

        public Race getRace() {
            return (Race)race;
        }

        public void SetRace(byte r) {
            race = r;
            UpdateProperty();
            if (GetComponent<MmoActor>()) {
                //log.InfoFormat("Setted Actor Race = {0}||| Call Stack = {1}",
                //    (Race)race, System.Environment.StackTrace);
            }
            
        }

        public void SetRace(int r) {
            race = (byte)r;
            UpdateProperty();
            if (GetComponent<MmoActor>()) {
                //log.InfoFormat("Setted Actor Race = {0}||| Call Stack = {1}",
                //    (Race)race, System.Environment.StackTrace);
            }
            
        }

        public void SetRace(Race r) {
            race = (byte)r;
            UpdateProperty();
            if (GetComponent<MmoActor>()) {
                //log.InfoFormat("Setted Actor Race = {0}||| Call Stack = {1}",
                //    (Race)race, System.Environment.StackTrace);
            }
        }

        private void UpdateProperty() {
            if(nebulaObject.properties)
                nebulaObject.properties.SetProperty((byte)PS.Race, race);
        }

        public Hashtable GetDatabaseSave() {
            if(mInitData != null) {
                return mInitData.AsHash();
            }
            return new Hashtable();
        }

        public override int behaviourId {
            get {
                return (int)ComponentID.Raceable;
            }
        }

    }
}
