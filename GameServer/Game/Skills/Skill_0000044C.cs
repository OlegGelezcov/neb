using Nebula.Engine;
using Nebula.Game.Bonuses;
using Nebula.Game.Components;
using Space.Game;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_0000044C : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            if (NotEnemyCheck(source, skill, info)) {
                return false;
            }

            var sourceWeapon = source.Weapon();
            var targetObject = source.Target().targetObject;
            var targetBonuses = targetObject.Bonuses();

            float dmgMult = skill.GetFloatInput("dmg_mult");
            float hpPc = skill.GetFloatInput("hp_pc");
            float hpTime = skill.GetFloatInput("hp_time");

            bool mastery = RollMastery(source);
            if(mastery) {
                dmgMult *= 2;
                hpTime *= 2;
            }

            WeaponHitInfo hit;
            var shot = sourceWeapon.Fire(out hit, skill.data.Id, dmgMult);
            if(hit.normalOrMissed) {
                Buff buff = new Buff(skill.id, null, Common.BonusType.decrease_max_hp_on_pc, hpTime, hpPc);
                targetBonuses.SetBuff(buff, source);
                source.MmoMessage().SendShot(Common.EventReceiver.OwnerAndSubscriber, shot);
                return true;
            } else {
                source.MmoMessage().SendShot(Common.EventReceiver.ItemOwner, shot);
                return false;
            }
        }
    }
}
