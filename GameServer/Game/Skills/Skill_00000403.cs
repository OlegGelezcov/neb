using Nebula.Engine;
using Nebula.Game.Bonuses;
using Nebula.Game.Components;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_00000403 : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {

            info = new Hashtable();

            float hpRestorePc = skill.GetFloatInput("hp_restore_pc");
            float maxHpPc = skill.GetFloatInput("max_hp_pc");

            bool mastery = RollMastery(source);
            if(mastery) {
                maxHpPc *= 2;
                hpRestorePc *= 2;
            }

            var damagable = source.Damagable();
            float restoredHp = hpRestorePc * damagable.baseMaximumHealth;
            damagable.RestoreHealth(source, restoredHp);

            var skills = source.Skills();

            Buff buff = new Buff(skill.data.Id.ToString(), source, Common.BonusType.increase_max_hp_on_pc, -1, maxHpPc, () => {
                return true;
            }, skill.data.Id);
            source.Bonuses().SetBuff(buff, source);
            return true;
        }
    }
}
