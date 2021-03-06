﻿using Common;
using ExitGames.Logging;
using Nebula.Drop;
using Nebula.Engine;
using Nebula.Game.Components;
using Space.Game;
using System.Collections;
using System;

namespace Nebula.Game.Pets {
    public class PetWeapon : BaseWeapon {
        private static readonly ILogger s_Log = LogManager.GetCurrentClassLogger();

        private PetObject m_Pet;
        private float m_Timer;
        private PlayerTarget m_Target;
        private MmoMessageComponent m_Message;
        private RaceableObject m_Race;

        public override void Awake() {
            base.Awake();
        }

        public override void Start() {
            base.Start();
            m_Pet = GetComponent<PetObject>();
            m_Target = GetComponent<PlayerTarget>();
            m_Message = GetComponent<MmoMessageComponent>();
            m_Race = GetComponent<RaceableObject>();

            if(m_Pet) {
                if(m_Pet.info != null ) {
                    m_Timer = m_Pet.info.Cooldown(nebulaObject.resource.petParameters.cooldown);
                }
            }
        }

        public override WeaponBaseType myWeaponBaseType {
            get {
                if(m_Race != null ) {
                    return CommonUtils.Race2WeaponBaseType((Race)m_Race.race);
                }
                return WeaponBaseType.Rocket;
            }
        }

        public void MakeShot() {

            if(!ready || !m_Target || !m_Pet || (m_Pet.info == null)) {
                return;
            }
            if(!m_Target.targetObject) {
                return;
            }

            if(m_Pet.info.damageType == Common.WeaponDamageType.damage) {
                WeaponHitInfo hit;
                Hashtable shot = Fire(m_Target.targetObject, out hit);
                if(hit.normalOrMissed) {
                    m_Message.SendShot(Common.EventReceiver.ItemSubscriber, shot);
                    ResetTimer();
                }
            } else if(m_Pet.info.damageType == Common.WeaponDamageType.heal) {
                Hashtable heal = Heal(m_Target.targetObject, GetDamage(false).totalDamage);
                m_Message.SendHeal(Common.EventReceiver.ItemSubscriber, heal);
                ResetTimer();
            }
        }

        private void ResetTimer() {
            m_Timer = m_Pet.info.Cooldown(nebulaObject.resource.petParameters.cooldown);
        }

        protected override bool CheckWeaponTarget(NebulaObject target) {
            return base.CheckWeaponTarget(target);
        }

        public override Hashtable Fire(NebulaObject targetObject, out WeaponHitInfo hit, int skillID = -1, float damageMult = 1, bool forceShot = false, bool useDamageMultSelfAsDamage = false) {
            return base.Fire(targetObject, out hit, skillID, damageMult, forceShot, useDamageMultSelfAsDamage);
        }

        //private WeaponBaseType myWeaponBaseType {
        //    get {
        //        if(m_Race != null ) {
        //            return CommonUtils.Race2WeaponBaseType((Race)m_Race.race);
        //        }
        //        return WeaponBaseType.Rocket;
        //    }
        //}

        public override WeaponDamage GetDamage(bool isCrit) {
            if(m_Pet) {
                if(m_Pet.info != null ) {
                    WeaponDamage dmg = new WeaponDamage(myWeaponBaseType);
                    dmg.SetBaseTypeDamage(m_Pet.info.Damage(nebulaObject.resource.petParameters.damage, nebulaObject.resource.Leveling));
                    return dmg;
                }
            }
            return new WeaponDamage(myWeaponBaseType);
        }

        public override Hashtable Heal(NebulaObject targetObject, float healValue, int skillID = -1, bool generateCrit = true) {
            if (ready) {
                return base.Heal(targetObject, healValue, skillID, generateCrit);
            }
            return FailHeal(targetObject);
        }

        public override float optimalDistance {
            get {
                if(m_Pet) {
                    if(m_Pet.info != null ) {
                        return m_Pet.info.OptimalDistance(nebulaObject.resource.petParameters.od, nebulaObject.resource.Leveling);
                    }
                }
                return 0f;
            }
        }

        public override bool ready {
            get {
                if(!m_Pet) {
                    return false;
                }
                if(m_Pet.info == null ) {
                    return false;
                }

                return (m_Timer <= 0f);
            }
        }
        public override float criticalChance {
            get {
                return 0f;
            }
        }
        public override void Update(float deltaTime) {
            base.Update(deltaTime);

            if(m_Timer > 0f ) {
                m_Timer -= deltaTime;
            }
            nebulaObject.properties.SetProperty((byte)PS.Damage, GetDamage(false).totalDamage);
            if (m_Pet) {
                nebulaObject.properties.SetProperty((byte)PS.LightCooldown, m_Pet.info.Cooldown(nebulaObject.resource.petParameters.cooldown));
            }
        }
    }
}
