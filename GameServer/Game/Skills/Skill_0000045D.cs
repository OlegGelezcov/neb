using Nebula.Engine;
using Nebula.Game.Bonuses;
using Nebula.Game.Components;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_0000045D : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();

            float time = skill.GetFloatInput("time");

            bool mastery = RollMastery(source);
            if(mastery) {
                time *= 2;
            }

            source.Damagable().SetIgnoreDamage(time);
            Buff blockWeaponDebuff = new Buff(skill.id, null, Common.BonusType.block_weapon, time);
            source.Bonuses().SetBuff(blockWeaponDebuff);
            return true;
        }
    }
}
