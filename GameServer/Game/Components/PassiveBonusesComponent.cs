using Common;
using ExitGames.Logging;
using Nebula.Database;
using Nebula.Engine;
using Nebula.Resources;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System;
using System.Collections;
using ServerClientCommon;
using Space.Game;

namespace Nebula.Game.Components {
    public class PassiveBonusesComponent : NebulaBehaviour, IInfoSource{

        private const float BONUSES_UPDATE_INTERVAL = 30;


        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private Data mSpeed;
        private Data mDamageDron;
        private Data mHealDron;
        private Data mCritChance;
        private Data mCritDamage;
        private Data mDamage;
        private Data mResist;
        private Data mMaxHP;
        private Data mMaxEnergy;
        private Data mColoredLoot;
        private Data mRestoreHP;
        private Data mRestoreEnergy;
        private Data mOptimalDistance;
        private Data mChanceDontDropLoot;
        private Data mChanceCraftColoredModule;

        private MmoMessageComponent mMessage;

        private bool mBonusesChanged = false;
        private float mUpdateTimer = BONUSES_UPDATE_INTERVAL;
        private bool mLoaded = false;

        private bool m_StartCalled = false;

        public override void Start() {
            if (!m_StartCalled) {
                m_StartCalled = true;
                mMessage = GetComponent<MmoMessageComponent>();
            }
        }

        public override void Update(float deltaTime) {
            if (mLoaded) {
                mUpdateTimer -= deltaTime;
                if (mUpdateTimer <= 0f) {
                    mUpdateTimer = BONUSES_UPDATE_INTERVAL;

                    UpdateBonus(mSpeed);
                    UpdateBonus(mDamageDron);
                    UpdateBonus(mHealDron);
                    UpdateBonus(mCritChance);
                    UpdateBonus(mCritDamage);
                    UpdateBonus(mDamage);
                    UpdateBonus(mResist);
                    UpdateBonus(mMaxHP);
                    UpdateBonus(mMaxEnergy);
                    UpdateBonus(mColoredLoot);
                    UpdateBonus(mRestoreHP);
                    UpdateBonus(mRestoreEnergy);
                    UpdateBonus(mOptimalDistance);
                    UpdateBonus(mChanceDontDropLoot);
                    UpdateBonus(mChanceCraftColoredModule);
                }
            }
        }

        private void UpdateBonus(Data bonus) {
            if(bonus != null ) {
                if(bonus.learningStarted ) {
                    if(bonus.TryCompleteBonus()) {
                        mMessage.ReceivePassiveBonusComplete(bonus.GetInfo());
                    }
                }
            }
        }
        public void SendUpdate() {
            if (mMessage) {
                mMessage.ReceivePassiveBonuses(GetInfo());
            }
        }

        public void StartLearning(PassiveBonusType type) {

            bool started = false;
            foreach(var pair in allData ) {
                if(pair.Key == type ) {
                    if(pair.Value != null ) {
                        if(pair.Value.TryStartLearning(resource)) {
                            started = true;
                            break;
                        }
                    }
                }
            }
            if(started) {
                SendUpdate();
            }
        }

        public Data GetData(PassiveBonusType type) {
            foreach(var data in allData ) {
                if(data.Value.type == type ) {
                    return data.Value;
                }
            }
            return null;
        }


         public ConcurrentDictionary<PassiveBonusType, Data> allData {
            get {
                ConcurrentDictionary<PassiveBonusType, Data> result = new ConcurrentDictionary<PassiveBonusType, Data>();
                result.TryAdd(PassiveBonusType.Speed, mSpeed);
                result.TryAdd(PassiveBonusType.DamageDron, mDamageDron);
                result.TryAdd(PassiveBonusType.HealDron, mHealDron);
                result.TryAdd(PassiveBonusType.CritChance, mCritChance);
                result.TryAdd(PassiveBonusType.CritDamage, mCritDamage);
                result.TryAdd(PassiveBonusType.Damage, mDamage);
                result.TryAdd(PassiveBonusType.Resist, mResist);
                result.TryAdd(PassiveBonusType.MaxHP, mMaxHP);
                result.TryAdd(PassiveBonusType.MaxEnergy, mMaxEnergy);
                result.TryAdd(PassiveBonusType.ColoredLoot, mColoredLoot);
                result.TryAdd(PassiveBonusType.RestoreHPSpeed, mRestoreHP);
                result.TryAdd(PassiveBonusType.RestoreEnergy, mRestoreEnergy);
                result.TryAdd(PassiveBonusType.OptimalDistance, mOptimalDistance);
                result.TryAdd(PassiveBonusType.ChanceNotDropLootAtDeath, mChanceDontDropLoot);
                result.TryAdd(PassiveBonusType.ChanceCraftColoredModule, mChanceCraftColoredModule);
                return result;
            }
        }

        public void Load() {
            Start();
            var characterID = GetComponent<PlayerCharacterObject>().characterId;
            log.InfoFormat("load passive bonuses = {0} [red]", characterID);
            var app = nebulaObject.mmoWorld().application;
            var savedBonuses = PassiveBonusesDatabase.instance(app).LoadPassiveBonuses(characterID);

            foreach(var savedBonusPair in savedBonuses ) {

                PassiveBonusType type = (PassiveBonusType)savedBonusPair.Key;
                switch(type) {
                    case PassiveBonusType.Speed:
                        mSpeed = new Data(type, savedBonusPair.Value.tier, savedBonusPair.Value.learningStarted, savedBonusPair.Value.learnStartTime, savedBonusPair.Value.learnEndTime);
                        break;
                    case PassiveBonusType.DamageDron:
                        mDamageDron = new Data(type, savedBonusPair.Value.tier, savedBonusPair.Value.learningStarted, savedBonusPair.Value.learnStartTime, savedBonusPair.Value.learnEndTime);
                        break;
                    case PassiveBonusType.HealDron:
                        mHealDron = new Data(type, savedBonusPair.Value.tier, savedBonusPair.Value.learningStarted, savedBonusPair.Value.learnStartTime, savedBonusPair.Value.learnEndTime);
                        break;
                    case PassiveBonusType.CritChance:
                        mCritChance = new Data(type, savedBonusPair.Value.tier, savedBonusPair.Value.learningStarted, savedBonusPair.Value.learnStartTime, savedBonusPair.Value.learnEndTime);
                        break;
                    case PassiveBonusType.CritDamage:
                        mCritDamage = new Data(type, savedBonusPair.Value.tier, savedBonusPair.Value.learningStarted, savedBonusPair.Value.learnStartTime, savedBonusPair.Value.learnEndTime);
                        break;
                    case PassiveBonusType.Damage:
                        mDamage = new Data(type, savedBonusPair.Value.tier, savedBonusPair.Value.learningStarted, savedBonusPair.Value.learnStartTime, savedBonusPair.Value.learnEndTime);
                        break;
                    case PassiveBonusType.Resist:
                        mResist = new Data(type, savedBonusPair.Value.tier, savedBonusPair.Value.learningStarted, savedBonusPair.Value.learnStartTime, savedBonusPair.Value.learnEndTime);
                        break;
                    case PassiveBonusType.MaxHP:
                        mMaxHP = new Data(type, savedBonusPair.Value.tier, savedBonusPair.Value.learningStarted, savedBonusPair.Value.learnStartTime, savedBonusPair.Value.learnEndTime);
                        break;
                    case PassiveBonusType.MaxEnergy:
                        mMaxEnergy = new Data(type, savedBonusPair.Value.tier, savedBonusPair.Value.learningStarted, savedBonusPair.Value.learnStartTime, savedBonusPair.Value.learnEndTime);
                        break;
                    case PassiveBonusType.ColoredLoot:
                        mColoredLoot = new Data(type, savedBonusPair.Value.tier, savedBonusPair.Value.learningStarted, savedBonusPair.Value.learnStartTime, savedBonusPair.Value.learnEndTime);
                        break;
                    case PassiveBonusType.RestoreHPSpeed:
                        mRestoreHP = new Data(type, savedBonusPair.Value.tier, savedBonusPair.Value.learningStarted, savedBonusPair.Value.learnStartTime, savedBonusPair.Value.learnEndTime);
                        break;
                    case PassiveBonusType.RestoreEnergy:
                        mRestoreEnergy = new Data(type, savedBonusPair.Value.tier, savedBonusPair.Value.learningStarted, savedBonusPair.Value.learnStartTime, savedBonusPair.Value.learnEndTime);
                        break;
                    case PassiveBonusType.OptimalDistance:
                        mOptimalDistance = new Data(type, savedBonusPair.Value.tier, savedBonusPair.Value.learningStarted, savedBonusPair.Value.learnStartTime, savedBonusPair.Value.learnEndTime);
                        break;
                    case PassiveBonusType.ChanceNotDropLootAtDeath:
                        mChanceDontDropLoot = new Data(type, savedBonusPair.Value.tier, savedBonusPair.Value.learningStarted, savedBonusPair.Value.learnStartTime, savedBonusPair.Value.learnEndTime);
                        break;
                    case PassiveBonusType.ChanceCraftColoredModule:
                        mChanceCraftColoredModule = new Data(type, savedBonusPair.Value.tier, savedBonusPair.Value.learningStarted, savedBonusPair.Value.learnStartTime, savedBonusPair.Value.learnEndTime);
                        break;
                }
            }

           
                

            var allDataDict = allData;
            foreach(PassiveBonusType type in System.Enum.GetValues(typeof(PassiveBonusType))) {
                Data data;
                if(allDataDict.TryGetValue(type, out data)) {
                    if(data == null ) {
                        data = new Data(type, 0, false, 0, 0);
                    }
                }
            }

            mBonusesChanged = true;
            mLoaded = true;
        }



        public Dictionary<int, PassiveBonusDbData> GetSave() {
            Dictionary<int, PassiveBonusDbData> result = new Dictionary<int, PassiveBonusDbData>();
            foreach(var pair in allData ) {
                if(pair.Value != null ) {
                    result.Add((int)pair.Key, new PassiveBonusDbData {
                        tier = pair.Value.tier,
                        learningStarted = pair.Value.learningStarted,
                        learnStartTime = pair.Value.learningStartTime,
                        learnEndTime = pair.Value.learningEndTime
                    });
                } else {
                    result.Add((int)pair.Key, new PassiveBonusDbData {
                        tier = 0,
                        learnEndTime = 0,
                        learningStarted = false,
                        learnStartTime = 0
                    });
                }
            }
            return result;
        }

        public Hashtable GetInfo() {
            Hashtable result = new Hashtable();
            foreach(var pair in allData ) {
                if(pair.Value != null ) {
                    result.Add((int)pair.Key, pair.Value.GetInfo());
                }
            }
            return result;
        }



        public override int behaviourId {
            get {
                return (int)ComponentID.PassiveBonuses;
            }
        }

        public class Data : IInfoSource {
            public PassiveBonusType type { get; private set; }
            public int tier { get; private set; }
            public bool learningStarted { get; private set; }
            public int learningStartTime { get; private set; }
            public int learningEndTime { get; private set; }

            public Data(PassiveBonusType inType, int inTier, bool inLearningStarted, int inLearningStartTime, int inLearningEndTime ) {
                type = inType;
                tier = inTier;
                learningStarted = inLearningStarted;
                learningStartTime = inLearningStartTime;
                learningEndTime = inLearningEndTime;
            }

            public bool TryCompleteBonus() {
                if(learningStarted) {
                    if(CommonUtils.SecondsFrom1970() >= learningEndTime ) {
                        learningStarted = false;
                        tier++;
                        return true;
                    }
                }
                return false;
            }

            private int nextTier {
                get {
                    return tier + 1;
                }
            }

            public bool TryStartLearning(IRes resource) {
                if(!learningStarted) {
                    int curTime = CommonUtils.SecondsFrom1970();
                    learningStartTime = curTime;
                    learningEndTime = curTime + (int)(nextTier * resource.PassiveBonuses.GetBaseTime(type));
                    learningStarted = true;
                    return true;
                }
                return false;
            }

            public Hashtable GetInfo() {
                var hash = new Hashtable {
                    { (int)SPC.Type, (int)type },
                    { (int)SPC.Tier, tier },
                    { (int)SPC.LearningStarted, learningStarted },
                    { (int)SPC.LearnStartTime, learningStartTime },
                    { (int)SPC.LearnEndTime, learningEndTime }
                };
                if(learningStarted) {
                    hash.Add((int)SPC.LearnProgress, (float)(CommonUtils.SecondsFrom1970() - learningStartTime) / (float)(learningEndTime - learningStartTime));
                }
                return hash;
            }
        }

        //realized in ActionExecutor.TransformObjectAndMoveToHold()
        public float craftColorredModuleBonus {
            get {
                if (mChanceCraftColoredModule != null) {
                    return mChanceCraftColoredModule.tier * resource.PassiveBonuses.chanceCraftColoredModule.valueForTier;
                }
                return 0f;
            }
        }

        public int craftColoredModuleTier {
            get {
                if (mChanceCraftColoredModule != null) {
                    return mChanceCraftColoredModule.tier;
                }
                return 0;
            }
        }


        //realized in MmoActor.Death()
        public float chanceDontDropLootBonus {
            get {
                if (mChanceDontDropLoot != null) {
                    return mChanceDontDropLoot.tier * resource.PassiveBonuses.chanceDontDropLoot.valueForTier;
                }
                return 0f;
            }
        }

        //realized in ShipWeapon
        public float optimalDistanceBonus {
            get {
                if (mOptimalDistance != null) {
                    return mOptimalDistance.tier * resource.PassiveBonuses.optimalDistance.valueForTier;
                }
                return 0f;
            }
        }

        public int optimalDistanceTier {
            get {
                if (mOptimalDistance != null) {
                    return mOptimalDistance.tier;
                }
                return 0;
            }
        }

        //realized in ShipEnergyBlock
        public float restoreEnergyBonus {
            get {
                if (mRestoreEnergy != null) {
                    return mRestoreEnergy.tier * resource.PassiveBonuses.restoreEnergy.valueForTier;
                }
                return 0f;
            }
        }

        public int restoreEnergyTier {
            get {
                if (mRestoreEnergy != null) {
                    return mRestoreEnergy.tier;
                }
                return 0;
            }
        }

        //realized in ShipBasedDamagableObject
        public float restoreHPBonus {
            get {
                if (mRestoreHP != null) {
                    return mRestoreHP.tier * resource.PassiveBonuses.restoreHP.valueForTier;
                }
                return 0f;
            }
        }

        public int restoreHPTier {
            get {
                if (mRestoreHP != null) {
                    return mRestoreHP.tier;
                }
                return 0;
            }
        }

        //realized in chest component
        public float coloredLootBonus {
            get {
                if (mColoredLoot != null) {
                    return mColoredLoot.tier * resource.PassiveBonuses.coloredLoot.valueForTier;
                }
                return 0f;
            }
        }

        //realized in ShipEnergyBlock
        public float maxEnergyBonus {
            get {
                if (mMaxEnergy != null) {
                    return mMaxEnergy.tier * resource.PassiveBonuses.maxEnergy.valueForTier;
                }
                return 0f;
            }
        }

        public int maxEnergyTier {
            get {
                if (mMaxEnergy != null) {
                    return mMaxEnergy.tier;
                }
                return 0;
            }
        }

        //realized in ShipBasedDamagableObject
        public float maxHPBonus {
            get {
                if (mMaxHP != null) {
                    return mMaxHP.tier * resource.PassiveBonuses.maxHP.valueForTier;
                }
                return 0f;


            }
        }

        public int maxHPTier {
            get {
                if (mMaxHP != null) {
                    return mMaxHP.tier;
                }
                return 0;
            }
        }

        //realized in ShipBasedDamagable object
        public float resistBonus {
            get {
                if (mResist != null) {
                    return mResist.tier * resource.PassiveBonuses.resist.valueForTier;
                }
                return 0f;
            }
        }

        public int resistTier {
            get {
                if (mResist != null) {
                    return mResist.tier;
                }
                return 0;
            }
        }

        //realized in ShipWeapon
        public float damageBonus {
            get {
                if (mDamage != null) {
                    return mDamage.tier * resource.PassiveBonuses.damage.valueForTier;
                }
                return 0f;
            }
        }

        public int damageBonusTier {
            get {
                if (mDamage != null) {
                    return mDamage.tier;
                }
                return 0;
            }
        }

        //realized in ShipWeapon
        public float critDamageBonus {
            get {
                if (mCritDamage != null) {
                    return mCritDamage.tier * resource.PassiveBonuses.critDamage.valueForTier;
                }
                return 0f;
            }
        }

        public int criticalDamageTier {
            get {
                if (mCritDamage != null) {
                    return mCritDamage.tier;
                }
                return 0;
            }
        }

        //Realized in ship weapon
        public float critChanceBonus {
            get {
                if (mCritChance != null) {
                    return mCritChance.tier * resource.PassiveBonuses.critChance.valueForTier;
                }
                return 0f;
            }
        }

        public int criticalChanceTier {
            get {
                if (mCritChance != null) {
                    return mCritChance.tier;
                }
                return 0;
            }
        }

        //realized in BaseWeapon
        public float healDronBonus {
            get {
                if (mHealDron != null) {
                    return mHealDron.tier * resource.PassiveBonuses.healDron.valueForTier;
                }
                return 0f;
            }
        }

        //realized in ShipMovable
        public float speedBonus {
            get {
                if (mSpeed != null) {
                    return mSpeed.tier * resource.PassiveBonuses.speed.valueForTier;
                }
                return 0f;
            }
        }

        public int speedTier {
            get {
                if (mSpeed != null) {
                    return mSpeed.tier;
                }
                return 0;
            }
        }

        //realized in BaseWeapon
        public float damageDronBonus {
            get {
                if (mDamageDron != null) {
                    return mDamageDron.tier * resource.PassiveBonuses.damageDron.valueForTier;
                }
                return 0f;
            }
        }

        public int damageDronTier {
            get {
                if (mDamageDron != null) {
                    return mDamageDron.tier;
                }
                return 0;
            }
        }

        public int healDronTier {
            get {
                if (mHealDron != null) {
                    return mHealDron.tier;
                }
                return 0;
            }
        }
    }
}
