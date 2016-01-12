using Nebula.Engine;
using Nebula.Game.Bonuses;
using Nebula.Game.Components;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_0000045F : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();

            float radius = skill.GetFloatInput("radius");
            float dmgPc = skill.GetFloatInput("dmg_pc");
            float dmgTime = skill.GetFloatInput("dmg_time");
            float dronPc = skill.GetFloatInput("dron_pc");
            float dronTime = skill.GetFloatInput("dron_time");

            bool mastery = RollMastery(source);
            if(mastery) {
                dmgPc *= 2;
                dronTime *= 2;
            }

            ActionOnEnemyTargets((item) => {
                Buff buff = new Buff(skill.id, null, Common.BonusType.decrease_damage_on_pc, dmgTime, dmgPc);
                item.Bonuses().SetBuff(buff);
            }, source, source, radius);

            Buff dronBuff = new Buff(skill.id, null, Common.BonusType.increase_dron_strength_on_pc, dronTime, dronPc);
            source.Bonuses().SetBuff(dronBuff);
            return true;
        }
    }
}
