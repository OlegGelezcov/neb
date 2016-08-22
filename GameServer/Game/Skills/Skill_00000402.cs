using Nebula.Engine;
using Nebula.Game.Bonuses;
using Nebula.Game.Components;
using System.Collections;

namespace Nebula.Game.Skills {

    public class Skill_00000402 : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            float resistPc = skill.GetFloatInput("resist_pc");
            float resistTime = skill.GetFloatInput("resist_time");
            bool mastery = RollMastery(source);
            if(mastery) {
                resistTime *= 2;
            }

            Buff buff = new Buff(skill.data.Id.ToString(), null, Common.BonusType.increase_resist_on_pc, resistTime, resistPc);
            source.Bonuses().SetBuff(buff, source);
            return true;
        }
    }
}
