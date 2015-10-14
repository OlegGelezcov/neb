using Common;
using Nebula.Engine;
using Nebula.Server.Components;
using Space.Game;
using System;
using System.Collections;

namespace Nebula.Game.Components {
    public class Turret : NebulaBehaviour, IDatabaseObject {

        private TurretComponentData mInitData;
        public void Init(TurretComponentData data) {
            mInitData = data;
        }

        public override void Start() {
            base.Start();
        }

        public override void Update(float deltaTime) {
            base.Update(deltaTime);
        }

        public override int behaviourId {
            get {
                return (int)ComponentID.Turret;
            }
        }



        public static FractionType SelectFraction(Race race) {
            switch(race) {
                case Race.Humans:
                    return FractionType.BotHumans;
                case Race.Criptizoids:
                    return FractionType.BotCriptizids;
                case Race.Borguzands:
                    return FractionType.BotBorguzands;
                case Race.None:
                    return FractionType.Friend;
            }
            throw new Exception("unknown race");
        }

        public Hashtable GetDatabaseSave() {
            if(mInitData != null ) {
                return mInitData.AsHash();
            }
            return new Hashtable();
        }
    }
}
