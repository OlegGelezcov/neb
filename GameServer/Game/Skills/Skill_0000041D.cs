﻿using Nebula.Engine;
using Nebula.Game.Bonuses;
using Nebula.Game.Components;
using System.Collections;

namespace Nebula.Game.Skills {

    //restored targe
    public class Skill_0000041D : SkillExecutor {

        /// <summary>
        /// Now this stun skill
        /// </summary>
        /// <param name="source">Source who cast</param>
        /// <param name="skill">Skill was used</param>
        /// <param name="info">Returned cast info</param>
        /// <returns>Status of cast using</returns>
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            info.SetSkillUseState(Common.SkillUseState.normal);

            if(ShotToEnemyRestricted(source, skill)) {
                info.SetSkillUseState(Common.SkillUseState.invalidTarget);
                return false;
            } else {
                if(NotCheckDistance(source)) {
                    info.SetSkillUseState(Common.SkillUseState.tooFar);
                    return false;
                }
            }

            float time = skill.GetFloatInput("time");

            bool mastery = RollMastery(source);
            if(mastery) {
                time *= 2.0f;
                info.SetMastery(true);
            } else {
                info.SetMastery(false);
            }

            source.Target().targetObject.Bonuses().SetBuff(new Buff(skill.id, null, Common.BonusType.stun, time, 1.0f), source);
            return true;

            /*
            info.SetSkillUseState(Common.SkillUseState.normal);
            if(!CheckForHealAlly(source)) {
                info.SetSkillUseState(Common.SkillUseState.invalidTarget);
                return false;
            }
            if(NotCheckDistance(source)) {
                info.SetSkillUseState(Common.SkillUseState.tooFar);
                return false;
            }

            float hpPc = skill.GetFloatInput("hp_pc");

            var targetObject = source.Target().targetObject;
            var targetDamagable = targetObject.Damagable();
            float restoreHP = targetDamagable.maximumHealth * hpPc;

            var weapon = source.Weapon();

            bool mastery = RollMastery(source);
            if(mastery) {
                restoreHP *= 2;
            }

            var heal = weapon.Heal(targetObject, restoreHP, skill.data.Id);
            source.MmoMessage().SendHeal(Common.EventReceiver.OwnerAndSubscriber, heal);

            var targetEnergy = targetObject.GetComponent<ShipEnergyBlock>();
            if(targetEnergy ) {
                float restoredEnergy = hpPc * targetEnergy.maximumEnergy;
                if(mastery) {
                    restoredEnergy *= 2;
                }
                targetEnergy.RestoreEnergy(restoredEnergy);
            }*/


        }
    }
}
