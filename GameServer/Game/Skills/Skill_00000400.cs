using Common;
using Nebula.Engine;
using Nebula.Game.Bonuses;
using Nebula.Game.Components;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_00000400 : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();

            if(!source) {
                return false;
            }
            var sourceBonuses = source.GetComponent<PlayerBonuses>();
            if(!sourceBonuses) {
                return false;
            }

            string buffID = source.Id + skill.data.Id;
            float buffSpeedPercent = skill.data.Inputs.Value<float>("b_speed_pc");
            float buffSpeedInterval = skill.data.Inputs.Value<float>("b_speed_interval");
            bool mastery = RollMastery(source);
            if(mastery) {
                buffSpeedInterval *= 2;
            }

            Buff speedBuff = new Buff(buffID, null, BonusType.increase_speed_on_pc, buffSpeedInterval, buffSpeedPercent);
            sourceBonuses.SetBuff(speedBuff, source);
            return true;
        }
    }
}
