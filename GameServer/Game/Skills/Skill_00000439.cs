using ExitGames.Logging;
using Nebula.Engine;
using Nebula.Game.Bonuses;
using Nebula.Game.Components;
using Nebula.Game.Utils;
using System;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_00000439 : SkillExecutor {

        private static readonly ILogger s_Log = LogManager.GetCurrentClassLogger();

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

            bool mastery = RollMastery(mySkills.nebulaObject);
            if(mastery) {
                critDmgTime *= 2;
            }


            int buffsCount = mySkills.bonuses.GetBuffCountWithTag(Common.BonusType.increase_crit_damage_on_pc, skillID);

            
            if(buffsCount < stackCount) {
                Buff newBuff = new Buff(Guid.NewGuid().ToString(), null, Common.BonusType.increase_crit_damage_on_pc, critDmgTime, critDmgPc);
                newBuff.SetTag(skillID);
                mySkills.bonuses.SetBuff(newBuff, mySkills.nebulaObject);
            }
            s_Log.InfoFormat("439.OnEnemyDeath()->critdmg% = {0}, critdmgtime={1}, stackcnt={2}, buffscnt={3}, bonustotal={4}".Color(LogColor.orange),
                critDmgPc, critDmgTime, stackCount, buffsCount, mySkills.bonuses.critDamagePcBonus);
        }
    }
}
