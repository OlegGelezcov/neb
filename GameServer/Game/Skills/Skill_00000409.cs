using Nebula.Engine;
using Nebula.Game.Components;
using Space.Game;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_00000409 : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();

            float dmgMult = skill.GetFloatInput("dmg_mult");
            float dmgAreaMult = skill.GetFloatInput("area_dmg_mult");
            float radius = skill.GetFloatInput("radius");
            var sourceCharacter = source.Character();
            var sourceRaceable = source.Raceable();

            if(!CheckForShotEnemy(source, skill)) {
                return false;
            }

            WeaponHitInfo hit;

            var sourceWeapon = source.Weapon();
            float baseDamage = sourceWeapon.GetDamage(false);
            float secondDamage = baseDamage * dmgAreaMult;

            bool mastery = RollMastery(source);
            if(mastery) {
                dmgMult *= 2;

            }
            var shot = source.Weapon().Fire(out hit, skill.data.Id, dmgMult);
            if(hit.hitAllowed) {
                source.MmoMessage().SendShot(Common.EventReceiver.OwnerAndSubscriber, shot);

                InputDamage inpDamage = new InputDamage(source, secondDamage);
                if(mastery) {
                    inpDamage.SetDamage(inpDamage.damage * 2);
                }
                foreach(var pitem in GetTargets(source, source.Target().targetObject, radius)) {
                    pitem.Value.Damagable().ReceiveDamage(inpDamage);
                }
                return true;
            } else {
                source.MmoMessage().SendShot(Common.EventReceiver.ItemOwner, shot);
                return false;
            }
        }
    }
}
