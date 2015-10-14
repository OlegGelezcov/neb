﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nebula.Engine;
using Nebula.Game.Components;
using Nebula.Game.Bonuses;

namespace Nebula.Game.Skills {
    public class Skill_00000439 : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            return true;
        }

        public void OnEnemyDeath(PlayerSkills mySkills) {

            int skillID = SkillExecutor.SkilIDFromHexString("00000439");
            var skill = mySkills.GetSkillById(skillID);
            if(skill == null || skill.IsEmpty) {
                return;
            }

            float critDmgPc = skill.GetFloatInput("crit_dmg_pc");
            float critDmgTime = skill.GetFloatInput("crit_dmg_time");
            int stackCount = skill.GetIntInput("stack_count");

            int buffsCount = mySkills.bonuses.GetBuffCountWithTag(Common.BonusType.increase_crit_damage_on_pc, skillID);
            if(buffsCount < stackCount) {
                Buff newBuff = new Buff(Guid.NewGuid().ToString(), null, Common.BonusType.increase_crit_damage_on_pc, critDmgTime, critDmgPc);
                newBuff.SetTag(skillID);
                mySkills.bonuses.SetBuff(newBuff);
            }
        }
    }
}
