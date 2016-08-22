using Common;
using Nebula.Engine;
using Nebula.Game.Bonuses;
using Nebula.Game.Components;
using Space.Game;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_0000040D : SkillExecutor  {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();

            info.SetSkillUseState(SkillUseState.normal);
            if (ShotToEnemyRestricted(source, skill)) {
                info.SetSkillUseState(SkillUseState.invalidTarget);
                return false;
            }
            if (NotCheckDistance(source)) {
                info.SetSkillUseState(SkillUseState.tooFar);
                return false;
            }
            var sourceWeapon = source.GetComponent<BaseWeapon>();
            float dmgMult = skill.GetFloatInput("dmg_mult");
            float damagePc = skill.GetFloatInput("damage_pc");
            float damageTime = skill.GetFloatInput("damage_time");

            bool mastery = RollMastery(source);
            if(mastery) {
                dmgMult *= 2;
                damageTime *= 2;
                info.SetMastery(true);
            } else {
                info.SetMastery(false);
            }

            WeaponHitInfo hit;
            var shotInfo = sourceWeapon.Fire(out hit, skill.data.Id, dmgMult);
            if (hit.normalOrMissed) {
                Buff damageDebuff = new Buff(skill.data.Id.ToString(), null, BonusType.decrease_damage_on_pc, damageTime, damagePc);
                source.Target().targetObject.Bonuses().SetBuff(damageDebuff, source);
                source.MmoMessage().SendShot(EventReceiver.OwnerAndSubscriber, shotInfo);
                return true;
            }
            return false;
        }
    }
}
