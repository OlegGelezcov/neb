using System;
using System.Collections;
using Common;
using ExitGames.Logging;
using GameMath;
using Nebula.Engine;
using Nebula.Server.Components;
using Space.Game;

namespace Nebula.Game.Components {
    public class MainOutpost : NebulaBehaviour, IDatabaseObject {

        private float CONSTRUCTION_INTERVAL = 5 * 60;

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private float mUpdateImmunityInterval = 5;
        private float mUpdateImmunityTimer;

        private bool mUnderConstruction;
        private float mConstructionTimer;

        private DamagableObject mDamagable;
        private DamageInfo mLastDamage;
        private RaceableObject mRaceable;


        public void Init(MainOutpostComponentData data) { }

        public override void Start() {
            base.Start();
            mUpdateImmunityTimer = mUpdateImmunityInterval;
            mDamagable = GetComponent<DamagableObject>();
            mRaceable = GetComponent<RaceableObject>();

            mConstructionTimer = CONSTRUCTION_INTERVAL;
            SetUnderConstruction(true);

            var world = nebulaObject.world as MmoWorld;


            if ((byte)world.ownedRace != mRaceable.race) {
                log.InfoFormat("change world race on outpost start [red]");
                world.SetCurrentRace((Race)mRaceable.race);
            }
        }

        public void SetConstruct(float time) {
            CONSTRUCTION_INTERVAL = time;
            mConstructionTimer = time;
            SetUnderConstruction(true);
            nebulaObject.mmoWorld().SaveWorldState();
        }

        private void SetUnderConstruction(bool value) {
            mUnderConstruction = value;
            nebulaObject.properties.SetProperty((byte)PS.UnderConstruction, mUnderConstruction);
            nebulaObject.properties.SetProperty((byte)PS.ConstructionTimer, constructProgress);
            nebulaObject.MmoMessage().SendUnderConstructChanged(value);
        }

        public override void Update(float deltaTime) {
            base.Update(deltaTime);

            mUpdateImmunityTimer -= deltaTime;
            if(mUpdateImmunityTimer <= 0f) {
                mUpdateImmunityTimer = mUpdateImmunityInterval;
                CheckGodState();
            }



            if(mConstructionTimer > 0 ) {
                mConstructionTimer -= deltaTime;
                if(mConstructionTimer <= 0f) {
                    mDamagable.ForceSetHealth(mDamagable.maximumHealth);
                    SetUnderConstruction(false);
                }
                nebulaObject.properties.SetProperty((byte)PS.ConstructionTimer, constructProgress);
            }
        }

        public float constructProgress {
            get {
                float tm = Mathf.Clamp(mConstructionTimer, 0f, CONSTRUCTION_INTERVAL);
                return Mathf.Clamp01((CONSTRUCTION_INTERVAL - tm) / CONSTRUCTION_INTERVAL);
            }
        }

        private void CheckGodState() {
            var world = nebulaObject.world as MmoWorld;
            if(world.FindObjectsOfType<Outpost>().Count > 0 ) {

                if(!mDamagable.god) {
                    //log.Info("Set MainOutpost GOD to true");
                    mDamagable.SetGod(true);
                }
            } else {
                if(mDamagable.god) {
                    //log.Info("Set MainOutpost GOD to false");
                    mDamagable.SetGod(false);
                }
            }
        }

        public void OnWasKilled() {
            log.InfoFormat("change race ow world at outpost when is killed [{0}:{1}] [red]", 
                nebulaObject.mmoWorld().Zone.Id, (Race)mRaceable.race);
            nebulaObject.mmoWorld().SetCurrentRace(Race.None, true);

        }

        public void Death() {
            
            
        }

        public void OnNewDamage(DamageInfo damageInfo) {
            mLastDamage = damageInfo as DamageInfo;
            if(mLastDamage != null ) {

                var world = nebulaObject.world as MmoWorld;
                if(!world.underAttack) {
                    world.SetUnderAttack(true);
                    world.SetAttackRace((Race)mLastDamage.race);
                }

                //log.InfoFormat("OnNewDamage(): receive damage from race = {0}", (Race)mLastDamage.race);
            } else {
                log.Info("OnNewDamage() error: invalid damage parameter");
            }
        }

        public Hashtable GetDatabaseSave() {
            return new Hashtable();
        }

        public override int behaviourId {
            get {
                return (int)ComponentID.MainOutpost;
            }
        }
    }
}
