namespace Nebula.Game.Components {
    using System;
    using System.Collections;
    using Common;
    using ExitGames.Logging;
    using GameMath;
    using Nebula.Engine;
    using Nebula.Server.Components;
    using Space.Game;

    /// <summary>
    /// System outpost, have race which have current world
    /// </summary>
    [REQUIRE_COMPONENT(typeof(RaceableObject))]
    public class Outpost : NebulaBehaviour, IDatabaseObject {
        private float CONSTRUCTION_INTERVAL = 5 * 60;

        private static ILogger log = LogManager.GetCurrentClassLogger();
        private RaceableObject mRaceable;
        private DamageInfo mLastDamage;
        private CharacterObject mCharacter;
        private DamagableObject mDamagable;
        private bool mUnderConstruction;
        private float mConstructionTimer;
        private OutpostComponentData mInitData;

        public void Init(OutpostComponentData data) {
            mInitData = data;
        }

        public override void Start() {
            base.Start();
            //cache raceable component
            mRaceable = GetComponent<RaceableObject>();
            mCharacter = GetComponent<CharacterObject>();
            mDamagable = GetComponent<DamagableObject>();

            mConstructionTimer = CONSTRUCTION_INTERVAL;
            SetUnderConstruction(true);
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

            if (mConstructionTimer > 0) {
                mConstructionTimer -= deltaTime;
                if (mConstructionTimer <= 0f) {
                    mDamagable.ForceSetHealth(mDamagable.maximumHealth);
                    SetUnderConstruction(false);
                }
                nebulaObject.properties.SetProperty((byte)PS.ConstructionTimer, constructProgress);
            }

            if (mDamagable.god) {
                log.InfoFormat("outpost at world = {0} was GOD, we change it to NOT GOD [red]", nebulaObject.mmoWorld().Zone.Id);
                mDamagable.SetGod(false);
            }
        }

        public float constructProgress {
            get {
                float tm = Mathf.Clamp(mConstructionTimer, 0f, CONSTRUCTION_INTERVAL);
                return Mathf.Clamp01((CONSTRUCTION_INTERVAL - tm) / CONSTRUCTION_INTERVAL);
            }
        } 

           

        public override int behaviourId {
            get {
                return (int)ComponentID.Outpost;
            }
        }

        /// <summary>
        /// Message receiver from DamagableObject component
        /// </summary>
        public void OnNewDamage(object damageInfo) {
            mLastDamage = damageInfo as DamageInfo;
            if (mLastDamage != null) {

                var world = nebulaObject.world as MmoWorld;
                if (!world.underAttack) {
                    world.SetUnderAttack(true);
                    world.SetAttackRace((Race)mLastDamage.race);
                }

                log.InfoFormat("OnNewDamage(): receive damage from race = {0}", (Race)mLastDamage.race);
            } else {
                log.Info("OnNewDamage() error: invalid damage parameter");
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
