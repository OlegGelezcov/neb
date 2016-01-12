using Nebula.Engine;
using Nebula.Game.Components;
using Space.Game;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_00000447 : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            if(ShotToEnemyRestricted(source, skill)) {
                return false;
            }

            var targetObject = source.Target().targetObject;
            var sourceWeapon = source.Weapon();
            var sourceMessage = source.MmoMessage();

            float dmgMult = skill.GetFloatInput("dmg_mult");
            float dmgAreaMult = skill.GetFloatInput("dmg_area_mult");
            float radius = skill.GetFloatInput("radius");

            bool mastery = RollMastery(source);
            if(mastery) {
                dmgMult *= 2;
                dmgAreaMult *= 2;
            }

            WeaponHitInfo hit;
            var shot = sourceWeapon.Fire(out hit, skill.data.Id, dmgMult);
            if(hit.hitAllowed) {
                sourceMessage.SendShot(Common.EventReceiver.OwnerAndSubscriber, shot);
                var items = GetTargets(source, targetObject, radius);
                foreach(var pItem in items) {
                    var item = pItem.Value;
                    WeaponHitInfo hit2;
                    var shot2 = sourceWeapon.Fire(item, out hit2, skill.data.Id, dmgAreaMult);
                    if(hit2.hitAllowed) {
                        sourceMessage.SendShot(Common.EventReceiver.OwnerAndSubscriber, shot2);
                    }
                }
                return true;
            } else {
                sourceMessage.SendShot(Common.EventReceiver.ItemOwner, shot);
                return false;
            }
        }
    }
}
