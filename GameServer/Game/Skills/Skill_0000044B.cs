using Nebula.Engine;
using Nebula.Game.Components;
using Space.Game;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_0000044B : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            if (NotEnemyCheck(source, skill, info)) {
                return false;
            }

            var sourceWeapon = source.Weapon();
            var targetObject = source.Target().targetObject;
            var targetDamagable = targetObject.Damagable();
            var sourceMessage = source.MmoMessage();

            float dmgMult = skill.GetFloatInput("dmg_mult");
            float timedDmgMult = skill.GetFloatInput("timed_dmg_mult");
            float timedDmgTime = skill.GetFloatInput("dmg_time");

            bool mastery = RollMastery(source);
            if(mastery) {
                dmgMult *= 2;
            }

            WeaponHitInfo hit;
            var shot = sourceWeapon.Fire(out hit, skill.data.Id, dmgMult);
            if(hit.normalOrMissed) {
                var timedDamage = sourceWeapon.GenerateDamage().totalDamage * timedDmgMult / timedDmgTime;
                targetDamagable.SetTimedDamage(timedDmgTime, timedDamage, sourceWeapon.myWeaponBaseType, skill.idInt);
                sourceMessage.SendShot(Common.EventReceiver.OwnerAndSubscriber, shot);
                return true;
            } else {
                sourceMessage.SendShot(Common.EventReceiver.ItemOwner, shot);
                return false;
            }
        }
    }
}
