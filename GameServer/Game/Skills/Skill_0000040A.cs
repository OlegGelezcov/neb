﻿using Common;
using Nebula.Engine;
using Nebula.Game.Bonuses;
using Nebula.Game.Components;
using Space.Game;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_0000040A : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            info.SetSkillUseState(SkillUseState.normal);
            if (ShotToEnemyRestricted(source, skill)) {
                info.SetSkillUseState(SkillUseState.invalidTarget);
                return false;
            }
            if (NotCheckDistance(source)) {
                info.SetSkillUseState(SkillUseState.tooFar);
                return false;
            }

            var sourceWeapon = source.Weapon();
            var sourceTarget = source.Target();

            var targetBonuses = sourceTarget.targetObject.Bonuses();
            if(!sourceTarget) {
                return false;
            }
            if(!sourceTarget.targetObject) {
                return false;
            }
            var targetShip = sourceTarget.targetObject.GetComponent<BaseShip>();
            if(!targetShip) {
                return false;
            }

            float targetResistance = sourceTarget.targetObject.GetComponent<BaseShip>().commonResist;
            float dmgMult = skill.GetFloatInput("dmg_mult");
            float resistTime = skill.GetFloatInput("resist_time");

            bool mastery = RollMastery(source);
            if(mastery) {
                dmgMult *= 2;
                resistTime *= 2;
                info.SetMastery(true);
            } else {
                info.SetMastery(false);
            }

            WeaponHitInfo hit;
            var shot = sourceWeapon.Fire(out hit, skill.data.Id, dmgMult);
            if(hit.normalOrMissed) {
                Buff resistDebuff = new Buff(skill.data.Id.ToString(), null, Common.BonusType.decrease_resist_on_cnt, resistTime, targetResistance);
                targetBonuses.SetBuff(resistDebuff, source);
                source.MmoMessage().SendShot(Common.EventReceiver.OwnerAndSubscriber, shot);
                return true;
            }
            return false;
        }
    }
}
