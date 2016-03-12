using Nebula.Engine;
using Nebula.Game.Bonuses;
using Nebula.Game.Components;
using ServerClientCommon;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_000007E0 : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            if(ShotToEnemyRestricted(source, skill)) {
                return false;
            }

            float speedPc = skill.GetFloatInput("speed_pc");
            float speedTime = skill.GetFloatInput("speed_time");
            float dmgPc = skill.GetFloatInput("dmg_pc");

            bool mastery = RollMastery(source);
            if(mastery) {
                dmgPc *= 2;
                speedPc *= 2;
            }


            var sourceWeapon = source.Weapon();
            var targetObject = source.Target().targetObject;
            var targetBonuses = targetObject.Bonuses();

            Buff speedDebuff = new Buff(skill.id, null, Common.BonusType.decrease_speed_on_pc, speedTime, speedPc);
            targetBonuses.SetBuff(speedDebuff);

            var targetDamagable = targetObject.Damagable();

            float damage = sourceWeapon.GenerateDamage().totalDamage * dmgPc;
            targetDamagable.SetTimedDamage(speedTime, damage, sourceWeapon.myWeaponBaseType);
            info.Add((int)SPC.Damage, damage);

            source.SetInvisibility(false);

            return true;
        }
    }
}
