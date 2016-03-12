using Nebula.Engine;
using Nebula.Game.Components;
using Space.Game;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_00000452 : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            if(ShotToEnemyRestricted(source, skill)) {
                return false;
            }

            float dmgMult = skill.GetFloatInput("dmg_mult");
            float hpMult = skill.GetFloatInput("hp_mult");

            bool mastery = RollMastery(source);
            if(mastery) {
                dmgMult *= 2;
                hpMult *= 2;
            }
            WeaponHitInfo hit;
            var shot = source.Weapon().Fire(out hit, skill.data.Id, dmgMult);
            if(hit.hitAllowed) {
                float hp = hit.actualDamage.totalDamage * hpMult;
                source.Damagable().RestoreHealth(source, hp);
                source.MmoMessage().SendShot(Common.EventReceiver.OwnerAndSubscriber, shot);
                return true;
            } else {
                source.MmoMessage().SendShot(Common.EventReceiver.ItemOwner, shot);
                return false;
            }
        }
    }
}
