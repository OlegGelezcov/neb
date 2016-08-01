using Nebula.Engine;
using Nebula.Game.Bonuses;
using Nebula.Game.Components;
using System.Collections;

namespace Nebula.Game.Skills {
    /// <summary>
    /// Cast speed debuff on target
    /// </summary>
    public class Skill_00000441 : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            info.SetSkillUseState(Common.SkillUseState.normal);
            if(ShotToEnemyRestricted(source, skill)) {
                info.SetSkillUseState(Common.SkillUseState.invalidTarget);
                return false;
            }
            if(NotCheckDistance(source)) {
                info.SetSkillUseState(Common.SkillUseState.tooFar);
                return false;
            }

            var targetObject = source.Target().targetObject;
            var targetBonuses = targetObject.Bonuses();
            var sourceMovable = source.Movable();

            float speedPc = skill.GetFloatInput("speed_pc");
            float speedTime = skill.GetFloatInput("time");

            float speedValue = sourceMovable.maximumSpeed * speedPc;

            bool mastery = RollMastery(source);
            if(mastery) {
                speedTime *= 2;
            }

            Buff targetBuff = new Buff(skill.id, null, Common.BonusType.decrease_speed_on_cnt, speedTime, speedValue);
            targetBonuses.SetBuff(targetBuff);
            return true;
        }
    }
}
