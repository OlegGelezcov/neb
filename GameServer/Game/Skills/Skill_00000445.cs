using Nebula.Engine;
using Nebula.Game.Bonuses;
using Nebula.Game.Components;
using Space.Game;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_00000445 : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            if(ShotToEnemyRestricted(source, skill)) {
                return false;
            }
            var targetObject = source.Target().targetObject;
            var sourceWeapon = source.Weapon();
            var sourceBonuses = source.Bonuses();
            WeaponHitInfo hit;

            float dmgMult = skill.GetFloatInput("dmg_mult");
            float speedPc = skill.GetFloatInput("speed_pc");
            float speedTime = skill.GetFloatInput("speed_time");

            bool mastery = RollMastery(source);
            if(mastery) {
                dmgMult *= 2;
                speedTime *= 2;
            }

            var shot = sourceWeapon.Fire(out hit, skill.data.Id, dmgMult);
            if(hit.hitAllowed) {
                Buff buff = new Buff(skill.id, null, Common.BonusType.increase_speed_on_pc, speedTime, speedPc);
                sourceBonuses.SetBuff(buff);
                source.MmoMessage().SendShot(Common.EventReceiver.OwnerAndSubscriber, shot);
                return true;
            } else {
                source.MmoMessage().SendShot(Common.EventReceiver.ItemOwner, shot);
                return false;
            }
        }
    }
}
