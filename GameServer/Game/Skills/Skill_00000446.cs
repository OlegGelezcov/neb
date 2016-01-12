using Nebula.Engine;
using Nebula.Game.Bonuses;
using Nebula.Game.Components;
using Space.Game;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_00000446 : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            if (ShotToEnemyRestricted(source, skill)) {
                return false;
            }
            var targetObject = source.Target().targetObject;
            var sourceWeapon = source.Weapon();
            var sourceBonuses = source.Bonuses();
            WeaponHitInfo hit;

            float dmgMult = skill.GetFloatInput("dmg_mult");
            float speedPc = skill.GetFloatInput("speed_pc");
            float speedTime = skill.GetFloatInput("speed_time");
            float radius = skill.GetFloatInput("radius");

            bool mastery = RollMastery(source);
            if(mastery) {
                dmgMult *= 2;
                speedTime *= 2;
            }

            var shot = sourceWeapon.Fire(out hit, skill.data.Id, dmgMult);
            if (hit.hitAllowed) {
                source.MmoMessage().SendShot(Common.EventReceiver.OwnerAndSubscriber, shot);

                var items = GetTargets(source, targetObject, radius);
                foreach(var pItem in items ) {
                    var item = pItem.Value;
                    var itemBonuses = item.Bonuses();
                    Buff speedDebuff = new Buff(skill.id, null, Common.BonusType.decrease_speed_on_pc, speedTime, speedPc);
                    itemBonuses.SetBuff(speedDebuff);
                }
                return true;
            } else {
                source.MmoMessage().SendShot(Common.EventReceiver.ItemOwner, shot);
                return false;
            }
        }
    }
}
