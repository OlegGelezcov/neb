using Common;
using ExitGames.Logging;
using GameMath;
using Nebula.Engine;
using Nebula.Game.Components;
using ServerClientCommon;
using Space.Database;
using Space.Game;
using Space.Game.Drop;
using Space.Game.Inventory.Objects;
using System.Collections;
using System;
using Nebula.Server.Components;
using System.Collections.Concurrent;
using Nebula.Database;
using Nebula.Drop;

namespace Nebula.Game.Components {

    [REQUIRE_COMPONENT(typeof(ShipEnergyBlock))]    
    [REQUIRE_COMPONENT(typeof(RaceableObject))]
    public class ShipWeapon : BaseWeapon, IInfoSource
    {
        public class AdditionalDamage {
            public float damageMult { get; set; }
            public float expireTime { get; set; }
        }

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        #region Linked components
        private RaceableObject mRace;
        private MmoActor mPlayer;
        private BaseShip mShip;
        private ShipEnergyBlock mEnergy;
        #endregion

        public override WeaponBaseType myWeaponBaseType {
            get {
                if(weaponObject != null ) {
                    return weaponObject.baseType;
                }
                if(mRace != null ) {
                    return CommonUtils.Race2WeaponBaseType((Race)mRace.race);
                }
                return WeaponBaseType.Rocket;
            }
        }

        public override Hashtable DumpHash() {
            var hash = base.DumpHash();
            hash["optimal_distance"] = optimalDistance.ToString();
            hash["critical_chance"] = criticalChance.ToString();
            hash["crit_damage"] = critDamage.ToString();
            hash["damage"] = damage.ToString();
            hash["ready"] = ready.ToString();
            hash["weapon"] = (Weapon != null) ? Weapon.GetInfo() : new Hashtable();
            hash["has_weapon"] = hasWeapon.ToString();
            hash["light_ready"] = lightReady.ToString();
            hash["heavy_ready"] = heavyReady.ToString();
            hash["light_timer"] = lightTimer.ToString();
            hash["heavy_timer"] = heavyTimer.ToString();
            hash["exists_weapon"] = mExistWeapon.ToString();
            hash["is_initialized"] = mInitialized.ToString();
            hash["weapon_difficulty"] = weaponDifficulty.ToString();
            hash["additional_damage_targets_count"] = mAdditionalDamageReceivers.Count.ToString();
            hash["Get_Damage(true)"] = GetDamage(true).totalDamage.ToString();
            hash["Get_Damage(false)"] = GetDamage(false).totalDamage.ToString();
            return hash;
        }
        private float lightTimer = 0f;
        private float heavyTimer = 0f;
        private bool mExistWeapon = false;
        private bool mInitialized = false;

        private ConcurrentDictionary<string, AdditionalDamage> mAdditionalDamageReceivers = new ConcurrentDictionary<string, AdditionalDamage>();

        #region Properties
        public override float optimalDistance {
            get {
                if (weaponObject != null) {
                    float result = weaponObject.OptimalDistance;
                    result = Mathf.ClampLess(result * (1f + cachedBonuses.optimalDistancePcBonus) + cachedBonuses.optimalDistanceCntBonus, 0f);
                    result = ApplyOptimalDistancePassiveBonus(result);
                    return result;
                }
                return 0f;
            }
        }

        public override float criticalChance {
            get {
                float result = 0;
                if (weaponObject != null) {
                    result = weaponObject.baseCritChance;
                }

                var ship = GetComponent<BaseShip>();
                if (ship) {
                    result += ship.shipModel.critChance;
                    result = result * (1.0f + cachedBonuses.critChancePcBonus) + cachedBonuses.critChanceCntBonus;
                    result = Mathf.ClampLess(result, 0.0f);
                }
                result = ApplyCriticalChancePassiveBonus(result);
                return result;
            }
        }

        public WeaponDamage critDamage {
            get {
                WeaponDamage result = damage;
                if (mShip) {
                    float  cr = (1 + mShip.shipModel.critDamageBonus) * (1.0f + cachedBonuses.critDamagePcBonus) + cachedBonuses.critDamageCntBonus;
                    result.Mult(cr);
                }
                result = ApplyCriticalDamagePassiveBonus(result);
                return result;
            }
        }

        private WeaponDamage damage {
            get {
                WeaponDamage dmg = new WeaponDamage();
                if (weaponObject != null) {
                    dmg.SetFromDamage(weaponObject.damage);
                }
                if (mShip) {
                    dmg.Mult(1.0f + mShip.shipModel.damageBonus);
                }
                dmg = ApplyDamagePassiveBonus(dmg);
                return dmg;
            }
        }

        public override bool ready {
            get {
                return true;
            }
        }

        public WeaponObject Weapon {
            get {
                return this.weaponObject;
            }
        }

        public bool hasWeapon {
            get {
                return (weaponObject != null);
            }
        }

        public bool lightReady {
            get {
                return lightTimer <= 0f;
            }
        }

        public bool heavyReady {
            get {
                return heavyTimer <= 0f;
            }
        }

        public Difficulty weaponDifficulty { get; private set; }
        public WeaponObject weaponObject { get; private set; } 
        #endregion

        private float ApplyOptimalDistancePassiveBonus(float inputOptimalDistance) {
            if (nebulaObject.IsPlayer()) {
                if (mPassiveBonuses != null && mPassiveBonuses.optimalDistanceTier > 0) {
                    return inputOptimalDistance * (1.0f + mPassiveBonuses.optimalDistanceBonus);
                }
            }
            return inputOptimalDistance;
        }

        /// <summary>
        /// Critical chance modified by passive bonuses
        /// </summary>
        /// <param name="criticalInput">Input critical chance from previous stages</param>
        /// <returns>Modified critical chance</returns>
        private float ApplyCriticalChancePassiveBonus(float criticalInput) {
            if(nebulaObject.IsPlayer()) {
                if(mPassiveBonuses != null && mPassiveBonuses.criticalChanceTier > 0 ) {
                    return Mathf.Clamp01(criticalInput + mPassiveBonuses.critChanceBonus);
                }
            }
            return criticalInput;
        }

        private WeaponDamage ApplyCriticalDamagePassiveBonus(WeaponDamage inputCriticalDamage) {
            if(nebulaObject.IsPlayer()) {
                if(mPassiveBonuses != null && mPassiveBonuses.criticalDamageTier > 0 ) {
                    inputCriticalDamage.Mult(1.0f + mPassiveBonuses.critDamageBonus);
                    return inputCriticalDamage;
                }
            }
            return inputCriticalDamage;
        }

        public override WeaponDamage GetDamage(bool isCritical) {
            WeaponDamage result = (isCritical) ? critDamage : damage;
            result.Mult(1.0f + cachedBonuses.damagePcBonus);
            result.AddToBase(cachedBonuses.damageCntBonus);
            result.ClampLess(0f);
            return result;
        }

        public void Init(ShipWeaponComponentData data) {
            nebulaObject.SetTag((byte)PS.Difficulty, (byte)data.difficulty);
            nebulaObject.SetTag((byte)PS.LightCooldown, data.cooldown);


            weaponDifficulty = data.difficulty;
            InitializeAsBot();
            CacheComponents();
            Startup();
        }

        private bool m_StartCalled = false;

        public override void Start() {
            if (!m_StartCalled) {
                m_StartCalled = true;
                base.Start();
                Startup();
            }
        }

        private void Startup() {
            if(!mInitialized) {
                mInitialized = true;
                mEnergy = RequireComponent<ShipEnergyBlock>();

                mRace = RequireComponent<RaceableObject>();
                mShip = GetComponent<BaseShip>();

                var character = GetComponent<CharacterObject>();
                var dropManager = DropManager.Get( nebulaObject.world.Resource());

                var player = GetComponent<MmoActor>();
                if (player != null) {

                } else {
                    InitializeAsBot();
                }

                mPlayer = GetComponent<MmoActor>();

                //log.Info("ShipWeapon.Start() completed");
            }
        }

        public void Load() {
            Start();
            log.InfoFormat("ShipWeapon Load() [dy]");

            var character = GetComponent<CharacterObject>();
            var dropManager = DropManager.Get(nebulaObject.world.Resource());
            var player = GetComponent<MmoActor>();

            var app = nebulaObject.mmoWorld().application;
            bool isNew = false;
            var dbWeapon = WeaponDatabase.instance(app).LoadWeapon(player.GetComponent<PlayerCharacterObject>().characterId, resource as Res, out isNew);


            if (isNew) {
                GenerateNewWeapon(dropManager);
                string characterID = GetComponent<PlayerCharacterObject>().characterId;
                WeaponDatabase.instance(app).SaveWeapon(characterID, new ShipWeaponSave { characterID = characterID, weaponObject = weaponObject.GetInfo() });
            } else {
                weaponObject = new WeaponObject(dbWeapon.weaponObject);
            }
        }

        //private float mPropTimer = 3;

        public override void Update(float deltaTime) {
            base.Update(deltaTime);

            lightTimer -= deltaTime;
            heavyTimer -= deltaTime;
            lightTimer = Mathf.ClampLess(lightTimer, -1);
            heavyTimer = Mathf.ClampLess(heavyTimer, -1);

            if (!hasWeapon) {
                return;
            }

            nebulaObject.properties.SetProperty((byte)PS.LightShotReady, lightReady);
            nebulaObject.properties.SetProperty((byte)PS.HeavyShotReady, heavyReady);
            nebulaObject.properties.SetProperty((byte)PS.LightShotReloadTimer, lightTimer);
            nebulaObject.properties.SetProperty((byte)PS.HeavyShotReloadTimer, heavyTimer);

            //if (mPlayer) {
            //    mPropTimer -= deltaTime;
            //    if(mPropTimer < 0) {
            //        mPropTimer = 3;
            //        nebulaObject.properties.SetProperty((byte)PS.Damage, GetDamage(false));
            //        nebulaObject.properties.SetProperty((byte)PS.CritDamage, GetDamage(true));
            //        nebulaObject.properties.SetProperty((byte)PS.CritChance, criticalChance);
            //        nebulaObject.properties.SetProperty((byte)PS.OptimalDistance, optimalDistance);
            //    }
            //}

        }

        public WeaponObject SetWeapon(WeaponObject weaponObject)
        {
            WeaponObject oldWeapon = this.weaponObject;
            this.weaponObject = weaponObject;
            if(this.weaponObject != null) {
                this.weaponObject.Bind();
            }
            if(oldWeapon != null) {
                oldWeapon.Bind();
            }

            var player = GetComponent<MmoActor>();
            if (player != null)
            {
                player.EventOnWeaponUpdated();
                player.EventOnShipModelUpdated();
                player.GetComponent<MmoMessageComponent>().ReceiveUpdateCombatStats();
            }
            return oldWeapon;
        }



        public override Hashtable Fire(NebulaObject targetObject, out WeaponHitInfo hit, int skillID = -1, float damageMult = 1, bool forceShot = false, bool useDamageMultSelfAsDamage = false) {

            RemoveExpiredAdditionalDamagers();

            if(mAdditionalDamageReceivers.ContainsKey(targetObject.Id)) {
                AdditionalDamage receiver;
                if(mAdditionalDamageReceivers.TryRemove(targetObject.Id, out receiver)) {
                    damageMult *= receiver.damageMult;
                }
            }

            var result =  base.Fire(targetObject, out hit, skillID, damageMult, forceShot, useDamageMultSelfAsDamage);
            if (cachedSkills) {
                cachedSkills.OnSourceFire();
            }

            //StartDamageDron(targetObject, hit);

            return result;
        }

        public override Hashtable Heal(NebulaObject targetObject, float healValue, int skillID = -1) {
            var result =  base.Heal(targetObject, healValue, skillID);
            return result;
        }

        private WeaponDamage ApplyDamagePassiveBonus(WeaponDamage inputDamage) {
            if(nebulaObject.IsPlayer()) {
                if(mPassiveBonuses != null && mPassiveBonuses.damageBonusTier > 0 ) {
                    inputDamage.Mult(1.0f + mPassiveBonuses.damageBonus);
                    return inputDamage;
                }
            }
            return inputDamage;
        }

        private void GenerateNewWeapon(DropManager dropManager) {

            var character = GetComponent<CharacterObject>();

            Workshop workshop = (Workshop)character.workshop;
            if(nebulaObject.tags.ContainsKey((byte)PlayerTags.Workshop)) {
                workshop = (Workshop)(byte)(int)nebulaObject.Tag((byte)PlayerTags.Workshop);
                log.InfoFormat("Found workshop tag = {0} yellow", workshop);
            }
            WeaponDropper.WeaponDropParams dropParams = new WeaponDropper.WeaponDropParams(
                nebulaObject.world.Resource(),
                character.level,
                workshop,
                WeaponDamageType.damage,
                Difficulty.none);
            var dropper = dropManager.GetWeaponDropper(dropParams);
            weaponObject = dropper.Drop() as WeaponObject;
        }

        public Hashtable GetInfo()
        {
            Hashtable result = new Hashtable();
            result.Add((int)SPC.HasWeapon, hasWeapon);
            result.Add((int)SPC.Damage, GetDamage(false).totalDamage);
            result.Add((int)SPC.CritDamage, GetDamage(true).totalDamage);
            result.Add((int)SPC.Source, hasWeapon ? this.weaponObject.GetInfo() : new Hashtable());
            result.Add((int)SPC.OptimalDistance, optimalDistance);
            return result;
        }

        public void InitializeAsBot() {

            if (!mExistWeapon) {
                mExistWeapon = true;
                CacheComponents();

                Difficulty difficulty = Difficulty.none;
                if (nebulaObject.HasTag((byte)PS.Difficulty)) {
                    difficulty = (Difficulty)(byte)nebulaObject.Tag((byte)PS.Difficulty);
                }
                nebulaObject.properties.SetProperty((byte)PS.Difficulty, (byte)weaponDifficulty);

                float lightCooldown = 2;
                float heavyCooldown = 10;

                if (nebulaObject.HasTag((byte)PS.LightCooldown)) {
                    lightCooldown = (float)nebulaObject.Tag((byte)PS.LightCooldown);
                }
                if (nebulaObject.HasTag((byte)PS.HeavyCooldown)) {
                    heavyCooldown = (float)nebulaObject.Tag((byte)PS.HeavyCooldown);
                }

                var dropParameters = new WeaponDropper.WeaponDropParams(
                    resource,
                    cachedCharacter.level,
                    (Workshop)cachedCharacter.workshop,
                    WeaponDamageType.damage,
                    difficulty
                    );
                var dropper = DropManager.Get(resource).GetWeaponDropper(dropParameters);
                SetWeapon(dropper.Drop() as WeaponObject);
            }
        }

        public ShipWeaponSave GetSave() {
            return new ShipWeaponSave {
                characterID = GetComponent<PlayerCharacterObject>().characterId,
                weaponObject = weaponObject.GetInfo()
            };
        }

        private void RemoveExpiredAdditionalDamagers() {
            if(mAdditionalDamageReceivers.Count == 0) {
                return;
            }

            ConcurrentBag<string> expiredIds = new ConcurrentBag<string>();
            float time = Time.curtime();
            foreach(var pair in mAdditionalDamageReceivers) {
                if(pair.Value.expireTime < time ) {
                    expiredIds.Add(pair.Key);
                }
            }

            foreach(string id in expiredIds ) {
                AdditionalDamage old;
                mAdditionalDamageReceivers.TryRemove(id, out old);
            }
        }

        public void AddAdditionalDamager(string id, AdditionalDamage dmg) {
            if(mAdditionalDamageReceivers.ContainsKey(id)) {
                AdditionalDamage old;
                if(mAdditionalDamageReceivers.TryRemove(id, out old)) {
                    mAdditionalDamageReceivers.TryAdd(id, dmg);
                }
            }
        }
    }

}
