﻿using Common;
using ExitGames.Logging;
using GameMath;
using Nebula.Drop;
using Nebula.Engine;
using ServerClientCommon;
using Space.Game;
using System.Collections;

namespace Nebula.Game.Components {

    [REQUIRE_COMPONENT(typeof(PlayerTarget))]
    [REQUIRE_COMPONENT(typeof(CharacterObject))]
    [REQUIRE_COMPONENT(typeof(PlayerBonuses))]
    //[REQUIRE_COMPONENT(typeof(PlayerSkills))]
    [REQUIRE_COMPONENT(typeof(DamagableObject))]
    public abstract class BaseWeapon  : NebulaBehaviour {

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        protected PassiveBonusesComponent mPassiveBonuses;
        private MmoMessageComponent mMessage;
        protected PlayerTarget cachedTarget { get; private set; }
        protected CharacterObject cachedCharacter { get; private set; }
        protected PlayerBonuses cachedBonuses { get; private set; }
        protected PlayerSkills cachedSkills { get; private set; }
        protected DamagableObject cachedDamagable { get; private set; }
        private AchievmentComponent m_Achievments;
        private BotObject m_CachedBot;

        public abstract WeaponBaseType myWeaponBaseType { get; }

        //0 - overheateed or not
        //1 - blocked or not
        private byte[] m_State = new byte[2] { 0, 0 };

        protected void EnableWeaponState(WeaponState state) {
            switch(state) {
                case WeaponState.overheated:
                    m_State[0] = 1;
                    break;
                case WeaponState.blocked:
                    m_State[1] = 1;
                    break;
            }
            UpdateStateProperty();
        }

        protected void DisableWeaponState(WeaponState state) {
            switch(state) {
                case WeaponState.overheated:
                    m_State[0] = 0;
                    break;
                case WeaponState.blocked:
                    m_State[1] = 0;
                    break;
            }
            UpdateStateProperty();
        }

        private void UpdateStateProperty() {
            props.SetProperty(PS.WeaponState, m_State);
        }

        private void UpdateStates() {
            if((!Mathf.Approximately(0f, cachedBonuses.Value(BonusType.block_weapon))) || singleShotBlocked) {
                EnableWeaponState(WeaponState.blocked);
            } else {
                DisableWeaponState(WeaponState.blocked);
            }
        }


        public override Hashtable DumpHash() {
            var hash = base.DumpHash();
            hash["shot_counter"] = shotCounter.ToString();
            hash["overheated?"] = overheated.ToString();
            hash["single_shot_blocked?"] = singleShotBlocked.ToString();
            hash["not_resettable_shot_counter"] = m_NotResettableShotCounter.ToString();
            hash["blocked?"] = blocked.ToString();
            hash["hit %"] = hitProb.ToString();
            return hash;
        }

        protected int shotCounter { get; private set; }

        protected bool overheated {
            get {
                return m_State[0] != 0;
            }
            set {
                if(value) {
                    EnableWeaponState(WeaponState.overheated);
                } else {
                    DisableWeaponState(WeaponState.overheated);
                }
            }
        }
        protected bool singleShotBlocked { get; private set; }
        private int m_NotResettableShotCounter = 0;

        public abstract float optimalDistance { get; }
        public abstract float criticalChance { get; }       
        public abstract bool ready { get; }


        protected bool blocked {
            get {
                return m_State[1] != 0;
            }
        }
        protected float hitProb {
            get {
                if (!(cachedTarget.hasTarget && cachedTarget.targetObject)) {
                    return 0.0f;
                }
                return HitProbTo(cachedTarget.targetObject);
            }
        }

        public int notResettableShotCounter {
            get {
                return m_NotResettableShotCounter;
            }
        }

        public abstract WeaponDamage GetDamage(bool isCrit);

        public override int behaviourId {
            get {
                return (int)ComponentID.Weapon;
            }
        }

        public void BlockSingleShot() {
            singleShotBlocked = true;
        }

        public void UnblockSingleShot() {
            singleShotBlocked = false;
        }

        public void ResetShotCounter() {
            shotCounter = 0;
        }


        protected virtual bool CheckWeaponTarget(NebulaObject target) {
            if(ready && nebulaObject && target.Damagable()) {
                return true;
            }
            return false;
        }


        public override void Start() {
            cachedTarget = RequireComponent<PlayerTarget>();
            cachedCharacter = RequireComponent<CharacterObject>();
            cachedBonuses = GetComponent<PlayerBonuses>();
            cachedSkills = GetComponent<PlayerSkills>();
            cachedDamagable = GetComponent<DamagableObject>();
            mPassiveBonuses = GetComponent<PassiveBonusesComponent>();
            mMessage = GetComponent<MmoMessageComponent>();
            m_Achievments = GetComponent<AchievmentComponent>();
            m_CachedBot = GetComponent<BotObject>();
        }

        public override void Update(float deltaTime) {
            nebulaObject.properties.SetProperty((byte)PS.OptimalDistance, optimalDistance);
            nebulaObject.properties.SetProperty((byte)PS.HitProb, hitProb);
            UpdateStates();
        }

        public Hashtable Fire(out WeaponHitInfo hit, int skillID = -1, float damageMult = 1.0f, bool forceShot = false, bool useDamageMultSelfAsDamage = false) {
            return Fire(cachedTarget.targetObject, out hit, skillID, damageMult, forceShot, useDamageMultSelfAsDamage);
        }



        public virtual Hashtable Fire(NebulaObject targetObject, 
            out WeaponHitInfo hit, 
            int skillID = -1, 
            float damageMult = 1.0f, 
            bool forceShot = false, 
            bool useDamageMultSelfAsDamage = false) {

            m_NotResettableShotCounter++;

            MakeMeVisible();

            hit = new WeaponHitInfo();
            
            if (CheckWeaponTarget(targetObject)) {

                Hashtable result = new Hashtable();
                result.Add((int)SPC.Source, nebulaObject.Id);
                result.Add((int)SPC.SourceType, nebulaObject.Type);
                result.Add((int)SPC.Target, targetObject.Id);
                result.Add((int)SPC.TargetType, targetObject.Type);
                result.Add((int)SPC.Workshop, cachedCharacter.workshop);
                result.Add((int)SPC.Skill, skillID);

                //InputDamage inputDamage = new InputDamage(nebulaObject, GetDamage())
                if (forceShot) {
                    InputDamage inputDamage = new InputDamage(nebulaObject, GetDamage(false));
                    CompleteForceShot(targetObject, ref hit, damageMult, useDamageMultSelfAsDamage, inputDamage);
                    inputDamage.SetHitInfo(hit);

                } else {

                    CheckWeaponBlocked(hit);

                    if(hit.interrupted) {
                        UnblockSingleShot();
                        WeaponDamage dmg = GetDamage(false);
                        dmg.ClearAllDamages();
                        InputDamage inputDamage = new InputDamage(nebulaObject, dmg);
                        inputDamage.SetHitInfo(hit);
                    } else {

                        WeaponDamage damage = null;
                        if (IsCritical(ref hit)) {
                            damage = GetDamage(true);
                        } else {
                            damage = GetDamage(false);
                        }
                        InputDamage inputDamage = new InputDamage(nebulaObject, damage);

                        CheckWeaponDistance(targetObject, hit);

                        if(hit.interrupted ) {
                            damage.ClearAllDamages();
                        } else {


                            CheckWeaponHit(targetObject, hit);

                            if(hit.interrupted ) {
                                damage.ClearAllDamages();
                            } else {
                                if (nebulaObject.IsPlayer()) {
                                    nebulaObject.SendMessage(ComponentMessages.OnCriticalHit);
                                }

                                shotCounter++;

                                ApplyDamage(ref hit, targetObject.Damagable(), damageMult, useDamageMultSelfAsDamage, inputDamage);
                            }
                        }

                        inputDamage.SetHitInfo(hit);
                    }
                }

                foreach (DictionaryEntry entry in hit.GetInfo()) {
                    result[(int)entry.Key] = entry.Value;
                }

                CheckPlayerAgro(targetObject);
                SendFireMessage(hit);
                //log.InfoFormat("final hit state: {0}", hit.state);

                return result;
            }
            return new Hashtable();
        }

        public Hashtable HealSelf(float val, int skillId = -1) {
            return Heal(nebulaObject, val, skillId);
        }

        public virtual Hashtable Heal(NebulaObject targetObject, float healValue, int skillID = -1, bool generateCrit = true) {
            MakeMeVisible();

            WeaponDamage notCritDmg = GetDamage(false);
            WeaponDamage critDmg = GetDamage(true);
            float ratio = critDmg.totalDamage / notCritDmg.totalDamage;

            healValue = Mathf.ClampLess( healValue * (1.0f + cachedBonuses.healingPcBonus) + cachedBonuses.healingCntBonus, 0f);

            bool isCritHeal = false;

            if (generateCrit) {
                if (Rand.Float01() < criticalChance) {
                    isCritHeal = true;
                    healValue *= ratio;

                }
            }

            var targetDamaable = targetObject.Damagable();
            targetDamaable.RestoreHealth(nebulaObject, healValue);

            nebulaObject.SendMessage(ComponentMessages.OnMakeHeal, healValue);

            StartHealDron(targetDamaable, healValue);

            if(isCritHeal ) {
                nebulaObject.SendMessage(ComponentMessages.OnCriticalHeal, healValue);
            }

            return ConstructHealMessage(nebulaObject, targetObject, cachedCharacter.workshop, skillID, healValue, isCritHeal);
        }

        public static Hashtable ConstructHealMessage(NebulaObject source, NebulaObject target, byte sourceObjectWorkshop, int skillId, float val, bool isCritical) {
            return new Hashtable {
                { (int)SPC.Source, source.Id },
                { (int)SPC.SourceType, source.Type },
                { (int)SPC.Target, target.Id },
                { (int)SPC.TargetType, target.Type },
                { (int)SPC.Workshop, sourceObjectWorkshop },
                { (int)SPC.Skill, skillId },
                { (int)SPC.HealValue, val },
                { (int)SPC.Critical, isCritical }
            };
        }

        protected Hashtable FailHeal(NebulaObject targetObject) {
            Hashtable result = new Hashtable();
            result.Add((int)SPC.Source, nebulaObject.Id);
            result.Add((int)SPC.SourceType, nebulaObject.Type);
            result.Add((int)SPC.Target, targetObject.Id);
            result.Add((int)SPC.TargetType, targetObject.Type);
            result.Add((int)SPC.Workshop, cachedCharacter.workshop);
            result.Add((int)SPC.Skill, -1);
            result.Add((int)SPC.HealValue, 0);
            //result.Add((int)SPC.IsCritical, false);
            return result;
        }

        private void StartHealDron(DamagableObject targetObject, float healValue) {
            if (nebulaObject.IsPlayer()) {

                if (mPassiveBonuses != null && mPassiveBonuses.healDronTier > 0) {

                    float dronHealValue = healValue * mPassiveBonuses.healDronBonus;

                    targetObject.RestoreHealth(nebulaObject, dronHealValue);

                    Hashtable dronInfo = new Hashtable {
                        { (int)SPC.Target, targetObject.nebulaObject.Id },
                        { (int)SPC.TargetType, targetObject.nebulaObject.Type },
                        { (int)SPC.HealValue, dronHealValue }
                    };
                    mMessage.SendHealDron(dronInfo);
                }
            }
        }

        private void MakeMeVisible() {
            nebulaObject.SetInvisibility(false);
        }

        private void SendFireMessage(WeaponHitInfo hit) {
            SendMessage(ComponentMessages.OnMakeFire, hit);
        }

        private void CheckPlayerAgro(NebulaObject targetObject) {
            if(targetObject.IsPlayer()) {
                targetObject.Target().OnHitMe(nebulaObject);
            }
        }

        private float ComputeMissProb(NebulaObject targetObject) {

            //if target either planet obj or turret or drill we don't use level difference miss
            if(targetObject.Type == (byte)ItemType.Bot ) {
                var botComp = targetObject.GetComponent<BotObject>();
                if(botComp != null ) {
                    BotItemSubType subType = (BotItemSubType)botComp.botSubType;
                    switch(subType) {
                        case BotItemSubType.PlanetBuilding:
                        case BotItemSubType.Turret:
                        case BotItemSubType.Drill:
                            if(nebulaObject.Type == (byte)ItemType.Avatar) {
                                log.InfoFormat("your attacked {0}, miss prob %", subType);
                            }
                            return 0;
                    }
                }
            }

            //if source planet obj or drill or turret we don't use level difference miss
            if(nebulaObject.Type == (byte)ItemType.Bot) {
                if(m_CachedBot != null ) {
                    BotItemSubType subType = (BotItemSubType)m_CachedBot.botSubType;
                    switch(subType) {
                        case BotItemSubType.Drill:
                        case BotItemSubType.PlanetBuilding:
                        case BotItemSubType.Turret:
                            return 0;
                    }
                }
            }

            var targetCharacter = targetObject.Character();
            if(!targetCharacter) {
                return 0f;
            }

            int myLevel = cachedCharacter.level;
            int targetLevel = targetCharacter.level;
            int diff = myLevel - targetLevel;
            if (diff >= 0) {
                return 0f;
            }
            switch(Mathf.Abs(diff)) {
                case 1:
                    return 0.01f;
                case 2:
                    return 0.05f;
                case 3:
                    return 0.15f;
                case 4:
                    return 0.25f;
                case 5:
                    return 0.4f;
            }

            return 0.6f;
        }

        public bool BlockedByDistance(NebulaObject obj) {
            return transform.DistanceTo(obj.transform) > optimalDistance;
        }



             
        protected void CompleteForceShot(NebulaObject obj, ref WeaponHitInfo hit, float damageMult, bool useDamageMultSelfAsDamage, InputDamage inputDamage) {

            CheckWeaponBlocked(hit);
            if(hit.normal) {
                CheckWeaponDistance(obj, hit);
                if(hit.normal) {
                    UnblockSingleShot();
                    ApplyDamage(ref hit, obj.Damagable(), damageMult, useDamageMultSelfAsDamage, inputDamage);
                }
            }

        }




        //private bool IsHitted(NebulaObject target, ref WeaponHitInfo hit) {
        //    if(hit.normal) {
        //        if(BlockedByDistance(target)) {
        //            hit.Interrupt(ShotState.blockedByDistance);
        //            return false;
        //        } else {
        //            float hitProb = 1.0f - ComputeMissProb(target);
        //            hitProb = Mathf.ClampLess(hitProb, 0.0f);
        //            if (Rand.Float01() <= hitProb) {
        //                hit.Interrupt(ShotState.missed);
        //                return false;
        //            } else {
        //                return true;
        //            }
        //        }

        //    }
        //    return false;
        //}

        private void CheckWeaponHit(NebulaObject target, WeaponHitInfo hit ) {
            float hitProb = 1.0f - ComputeMissProb(target);
            hitProb = Mathf.ClampLess(hitProb, 0.0f);
            if (Rand.Float01() > hitProb) {
                hit.Interrupt(ShotState.missed);
            }
        }

        private void CheckWeaponDistance( NebulaObject target, WeaponHitInfo hit) {
            if(hit.normal && BlockedByDistance(target)) {
                hit.Interrupt(ShotState.blockedByDistance);
            } 
        }

        private void CheckWeaponBlocked(WeaponHitInfo hit ) {
            if(hit.normal && blocked ) {
                hit.Interrupt(ShotState.blockedBySkill);
            }
        }
        //private bool IsWeaponNotBlocked(ref WeaponHitInfo hit) {
        //    if(hit.normal) {
        //        if(blocked) {
        //            hit.Interrupt(ShotState.blockedBySkill);
        //            return false;
        //        }
        //    }
        //    return true;
        //}

        public WeaponDamage GetDamage() {
            if(RollCritical()) {
                return GetDamage(true);
            }
            return GetDamage(false);
        }

        private bool RollCritical() {
            return Rand.Float01() <= criticalChance;
        }

        private bool IsCritical(ref WeaponHitInfo hit) {
            if(RollCritical()) {
                hit.MakeCritical();
            }
            return (hit.state == ShotState.normalCritical);
        }


        private void ApplyDamage(ref WeaponHitInfo hit, DamagableObject target, float damageMult, bool useDamageMultSelfAsDamage, InputDamage inputDamage) {
            inputDamage.Mult(damageMult * Rand.NormalNumber(0.8f, 1.2f));

            if(useDamageMultSelfAsDamage) {
                WeaponDamage dmg = new WeaponDamage(inputDamage.weaponBaseType, 0, 0, 0);
                dmg.SetBaseTypeDamage(damageMult);
                inputDamage.CopyValues(dmg);
            }

            if (cachedSkills) {
                cachedSkills.ModifyDamage(target, inputDamage);
            }

            int level = 1;
            byte workshop = (byte)Workshop.Arlen;
            byte race = (byte)Race.None;
            level = cachedCharacter.level;
            workshop = cachedCharacter.workshop;

            RaceableObject raceable = nebulaObject.Raceable();
            if (raceable) {
                race = raceable.race;
            }

            var copy = inputDamage.CreateCopy();
            if (false == ReflectDamage(target, ref hit, inputDamage)) {
                target.ReceiveDamage(inputDamage);
                StartDamageDron(target, inputDamage.CreateCopy(), workshop, level, race);
            }
            hit.SetRemainTargetHp(target.health);

            if(m_Achievments != null ) {
                m_Achievments.OnMakeDamage(copy);
            }
        }


        private void StartDamageDron(DamagableObject targetObject, WeaponDamage inputDamage, byte workshop, int level, byte race) {

            if (nebulaObject.IsPlayer()) {

                if (mPassiveBonuses != null && mPassiveBonuses.damageDronTier > 0) {

                    WeaponDamage dronDamage = new WeaponDamage();
                    dronDamage.SetFromDamage(inputDamage);
                    dronDamage.Mult(mPassiveBonuses.damageDronBonus);

                    InputDamage inpDamage = new InputDamage(nebulaObject, dronDamage);
                    targetObject.ReceiveDamage(inpDamage);

                    Hashtable dronInfo = new Hashtable {
                        { (int)SPC.Target, targetObject.nebulaObject.Id },
                        { (int)SPC.TargetType, targetObject.nebulaObject.Type },
                        { (int)SPC.Damage, dronDamage.totalDamage  }
                    };
                    mMessage.SendDamageDron(dronInfo);
                }
            }
        }

        private bool ReflectDamage(DamagableObject targetDamagable, ref WeaponHitInfo hit, InputDamage inputDamage) {
            bool reflected = targetDamagable.TryReflectDamage();
            if(reflected) {
                if(hit.normal) {
                    DamageParams damageParams = new DamageParams();
                    damageParams.SetReflrected(true);
                    InputDamage inpDamage = new InputDamage(targetDamagable.nebulaObject, inputDamage.CreateCopy(), damageParams);
                    cachedDamagable.ReceiveDamage(inpDamage);
                }
            }
            return reflected;
        }


        public float HitProbTo(NebulaObject nebObject) {
            return GameBalance.ComputeHitProb(optimalDistance, transform.DistanceTo(nebObject.transform), nebObject.size);
        }

        public WeaponDamage GenerateDamage() {
            if (Rand.Float01() < criticalChance) {
                return GetDamage(true);
            } else {
                return GetDamage(false);
            }
        }

        protected void CacheComponents() {
            cachedBonuses = GetComponent<PlayerBonuses>();
            cachedCharacter = GetComponent<CharacterObject>();
            cachedDamagable = GetComponent<DamagableObject>();
            cachedSkills = GetComponent<PlayerSkills>();
            cachedTarget = GetComponent<PlayerTarget>();
        }
    }
}
