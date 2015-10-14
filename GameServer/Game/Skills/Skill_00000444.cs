using Nebula.Engine;
using Nebula.Game.Components;
using Space.Game;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_00000444 : SkillExecutor {
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
            float hpPc = skill.GetFloatInput("hp_pc");

            var shot = sourceWeapon.Fire(out hit, skill.data.Id, dmgMult);
            if(hit.hitAllowed) {
                sourceWeapon.Heal(source, source.Damagable().maximumHealth * hpPc);
                source.MmoMessage().SendShot(Common.EventReceiver.OwnerAndSubscriber, shot);
                return true;
            } else {
                source.MmoMessage().SendShot(Common.EventReceiver.ItemOwner, shot);
                return false;
            }
        }
    }
}
