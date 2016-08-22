using Common;
using Nebula.Engine;
using Nebula.Game.Bonuses;
using Nebula.Game.Components;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_0000041E : SkillExecutor  {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            info.SetSkillUseState(SkillUseState.normal);

            if(!source) {
                return false;
            }

            float speedPc = skill.data.Inputs.Value<float>("speed_pc");
            float speedTime = skill.data.Inputs.Value<float>("speed_time");

            bool mastery = RollMastery(source);
            if(mastery) {
                speedTime *= 2;
                speedPc *= 2.0f;
                info.SetMastery(true);
            } else {
                info.SetMastery(false);
            }

            Buff buff = new Buff(skill.data.Id.ToString(), null, BonusType.increase_speed_on_pc, speedTime, speedPc);
            source.GetComponent<PlayerBonuses>().SetBuff(buff, source);
            return true;
        }
    }
}
