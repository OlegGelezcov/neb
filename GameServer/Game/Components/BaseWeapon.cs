using Common;
using ExitGames.Logging;
using GameMath;
using Nebula.Engine;
using ServerClientCommon;
using Space.Game;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        protected int shotCounter { get; private set; }

        protected bool overheated { get; private set; }

        protected bool singleShotBlocked { get; private set; }

        public abstract float optimalDistance { get; }
        public abstract float criticalChance { get; }       
        public abstract bool ready { get; }

        public abstract float GetDamage(bool isCrit);

        public override int behaviourId {
            get {
                return (int)ComponentID.Weapon;
            }
        }

        protected bool blocked {
            get {
                return (!Mathf.Approximately(0f, cachedBonuses.Value(BonusType.block_weapon))) || singleShotBlocked;
            }
        }
        

        protected float hitProb {
            get {
                if(!(cachedTarget.hasTarget && cachedTarget.targetObject)) {
                    return 0.0f;
                }
                return HitProbTo(cachedTarget.targetObject);
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
        }

        public override void Update(float deltaTime) {
            nebulaObject.properties.SetProperty((byte)PS.OptimalDistance, optimalDistance);
            nebulaObject.properties.SetProperty((byte)PS.HitProb, hitProb);
        }

        public Hashtable Fire(out WeaponHitInfo hit, int skillID = -1, float damageMult = 1.0f, bool forceShot = false, bool useDamageMultSelfAsDamage = false) {
            return Fire(cachedTarget.targetObject, out hit, skillID, damageMult, forceShot, useDamageMultSelfAsDamage);
        }


        public virtual Hashtable Fire(NebulaObject targetObject, out WeaponHitInfo hit, int skillID = -1, float damageMult = 1.0f, bool forceShot = false, bool useDamageMultSelfAsDamage = false) {
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

                if (forceShot) {
                    CompleteForceShot(targetObject, ref hit, damageMult, useDamageMultSelfAsDamage);
                } else {
                    CompleteShot(targetObject, ref hit, targetObject.Damagable(), damageMult, useDamageMultSelfAsDamage);
                }

                foreach (DictionaryEntry entry in hit.GetInfo()) {
                    result[(int)entry.Key] = entry.Value;
                }

                CheckPlayerAgro(targetObject);
                SendFireMessage(hit);
                return result;
            }
            return new Hashtable();
        }

        public virtual Hashtable Heal(NebulaObject targetObject, float healValue, int skillID = -1) {
            MakeMeVisible();

            float notCritDmg = GetDamage(false);
            float critDmg = GetDamage(true);
            float ratio = critDmg / notCritDmg;

            healValue = Mathf.ClampLess( healValue * (1.0f + cachedBonuses.healingPcBonus) + cachedBonuses.healingCntBonus, 0f);

            bool isCritHeal = false;
            if (Rand.Float01() < criticalChance) {
                isCritHeal = true;
                healValue *= ratio;
                nebulaObject.SendMessage(ComponentMessages.OnCriticalHeal, healValue);
            }

            var targetDamaable = targetObject.Damagable();
            targetDamaable.RestoreHealth(nebulaObject, healValue);

            nebulaObject.SendMessage(ComponentMessages.OnMakeHeal, healValue);

            StartHealDron(targetDamaable, healValue);

            Hashtable result = new Hashtable();
            result.Add((int)SPC.Source, nebulaObject.Id);
            result.Add((int)SPC.SourceType, nebulaObject.Type);
            result.Add((int)SPC.Target, targetObject.Id);
            result.Add((int)SPC.TargetType, targetObject.Type);
            result.Add((int)SPC.Workshop, cachedCharacter.workshop);
            result.Add((int)SPC.Skill, skillID);
            result.Add((int)SPC.HealValue, healValue);
            result.Add((int)SPC.IsCritical, isCritHeal);
                   
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
            if (hit.hitAllowed) {
                SendMessage(ComponentMessages.OnMakeFire, hit);
            }
        }

        private void CheckPlayerAgro(NebulaObject targetObject) {
            if(targetObject.IsPlayer()) {
                targetObject.Target().OnHitMe(nebulaObject);
            }
        }

        private float ComputeMissProb(NebulaObject targetObject) {

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

        protected void CompleteForceShot(NebulaObject obj, ref WeaponHitInfo hit, float damageMult, bool useDamageMultSelfAsDamage) {
            hit.hitAllowed = true;
            hit.hitProb = 1;
            hit.isHitted = true;
            hit.isCritical = false;

            bool hitBlocked = blocked;

            hit.SetWeaponBlocked(hitBlocked);
            if(hitBlocked) {
                hit.hitAllowed = false;
            }
            UnblockSingleShot();

            hit.SetGunsOverheatted(false);
            ApplyDamage(ref hit, obj.Damagable(), damageMult, useDamageMultSelfAsDamage);
        }


        protected void CompleteShot(NebulaObject targetObject, ref WeaponHitInfo hit, DamagableObject target, float damageMult, bool useDamageMultSelfAsDamage) {
            float hp = HitProbTo(targetObject);
            if(hp <= 0.01f) {
                hit.SetErrorMessageId("EM0002");
                return;
            }

            //If hp 100% exists small chance to hit miss!
            if(Mathf.Approximately(hp, 1f)) {
                hp -= 0.05f;
            }

            hp -= ComputeMissProb(targetObject);
            hp = Mathf.ClampLess(hp, 0f);

            if(Rand.Float01() < hp) {
                hit.isHitted = true;
                float myCritChance = criticalChance;
                if(cachedSkills) {
                    myCritChance = cachedSkills.ModifyCritChance(targetObject.Damagable(), myCritChance);
                }

                if(Rand.Float01() < criticalChance) {
                    hit.isCritical = true;
                    if (nebulaObject.IsPlayer()) {
                        nebulaObject.SendMessage(ComponentMessages.OnCriticalHit);
                    }
                }
            } else {
                hit.isHitted = false;
            }
            hit.hitAllowed = true;
            hit.hitProb = hp;
            hit.SetWeaponBlocked(blocked);
            UnblockSingleShot();

            if(hit.IsWeaponBlocked) {
                hit.hitAllowed = false;
                hit.SetErrorMessageId("EM0004");
                return;
            }

            if(!hit.isHitted) {
                hit.SetActualDamage(0);
                return;
            }
            hit.SetGunsOverheatted(overheated);
            shotCounter++;
            ApplyDamage(ref hit, target, damageMult, useDamageMultSelfAsDamage);
        }



        private void ApplyDamage(ref WeaponHitInfo hit, DamagableObject target, float damageMult, bool useDamageMultSelfAsDamage) {
            float inputDamage = GetDamage(hit.isCritical) * damageMult * Rand.NormalNumber(0.8f, 1.2f);
            if(useDamageMultSelfAsDamage) {
                inputDamage = damageMult;
            }

            if (cachedSkills) {
                inputDamage = cachedSkills.ModifyDamage(target, inputDamage);
            }

            int level = 1;
            byte workshop = (byte)Workshop.Arlen;
            byte race = (byte)Race.None;
            level = cachedCharacter.level;
            workshop = cachedCharacter.workshop;

            RaceableObject raceable= nebulaObject.Raceable();
            if (raceable) {
                race = raceable.race;
            }
            float actualDamage = 0f;

            if (false == ReflectDamage(target, ref hit, inputDamage)) {
                actualDamage = target.ReceiveDamage(nebulaObject.Type, nebulaObject.Id, inputDamage, workshop, level, race);
                StartDamageDron(target, inputDamage, workshop, level, race);
            }
            hit.SetActualDamage(actualDamage);
            hit.SetRemainTargetHp(target.health);
        }

        private void StartDamageDron(DamagableObject targetObject, float inputDamage, byte workshop, int level, byte race) {
            if (nebulaObject.IsPlayer()) {
                if (mPassiveBonuses != null && mPassiveBonuses.damageDronTier > 0) {
                    float dronDamage = inputDamage * mPassiveBonuses.damageDronBonus;
                    targetObject.ReceiveDamage(nebulaObject.Type, nebulaObject.Id, dronDamage, workshop, level, race);

                    Hashtable dronInfo = new Hashtable {
                        { (int)SPC.Target, targetObject.nebulaObject.Id },
                        { (int)SPC.TargetType, targetObject.nebulaObject.Type },
                        { (int)SPC.Damage, dronDamage }
                    };
                    mMessage.SendDamageDron(dronInfo);
                }
            }
        }

        private bool ReflectDamage(DamagableObject targetDamagable, ref WeaponHitInfo hit, float damage) {
            bool reflected = targetDamagable.TryReflectDamage();
            if(reflected) {
                var targetCharacter = targetDamagable.nebulaObject.Character();
                var targetRaceable = targetDamagable.nebulaObject.Raceable();
                byte targetWorkshop = (byte)Workshop.Arlen;
                int targetLevel = 1;
                if(targetCharacter) {
                    targetWorkshop = targetCharacter.workshop;
                    targetLevel = targetCharacter.level;
                }
                byte targetRace = (byte)Race.None;
                if(targetRaceable) {
                    targetRace = targetRaceable.race;
                }

                if(hit.notBlocked && hit.hitAllowed) {
                    cachedDamagable.ReceiveDamage(targetDamagable.nebulaObject.Type, targetDamagable.nebulaObject.Id, damage, targetWorkshop, targetLevel, targetRace);
                }
            }
            return reflected;
        }


        public float HitProbTo(NebulaObject nebObject) {
            return GameBalance.ComputeHitProb(optimalDistance, transform.DistanceTo(nebObject.transform), nebObject.size);
        }

        public float GenerateDamage() {
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
