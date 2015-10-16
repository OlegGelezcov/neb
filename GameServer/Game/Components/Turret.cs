using Common;
using ExitGames.Logging;
using Nebula.Engine;
using Nebula.Server.Components;
using Space.Game;
using System;
using System.Collections;

namespace Nebula.Game.Components {
    public class Turret : NebulaBehaviour, IDatabaseObject {

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private TurretComponentData mInitData;

        private DamagableObject mDamagable;

        public void Init(TurretComponentData data) {
            mInitData = data;
        }

        public override void Start() {
            base.Start();
            mDamagable = GetComponent<DamagableObject>();
        }

        public override void Update(float deltaTime) {
            if(mDamagable.god) {
                log.InfoFormat("turret at world = {0} was GOD, we change it to NOT GOD [red]", nebulaObject.mmoWorld().Zone.Id);
                mDamagable.SetGod(false);
            }
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
