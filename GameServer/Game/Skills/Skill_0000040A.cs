using Nebula.Engine;
using Nebula.Game.Bonuses;
using Nebula.Game.Components;
using Space.Game;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_0000040A : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            if(!CheckForShotEnemy(source, skill)) {
                return false;
            }

            var sourceWeapon = source.Weapon();
            var sourceTarget = source.Target();

            var targetBonuses = sourceTarget.targetObject.Bonuses();
            float targetResistance = sourceTarget.targetObject.GetComponent<BaseShip>().damageResistance;
            float dmgMult = skill.GetFloatInput("dmg_mult");
            float resistTime = skill.GetFloatInput("resist_time");

            WeaponHitInfo hit;
            var shot = sourceWeapon.Fire(out hit, skill.data.Id, dmgMult);
            if(hit.hitAllowed) {
                Buff resistDebuff = new Buff(skill.data.Id.ToString(), null, Common.BonusType.decrease_resist_on_cnt, resistTime, targetResistance);
                targetBonuses.SetBuff(resistDebuff);
                source.MmoMessage().SendShot(Common.EventReceiver.OwnerAndSubscriber, shot);
                return true;
            } else {
                source.MmoMessage().SendShot(Common.EventReceiver.ItemOwner, shot);
                return false;
            }
        }
    }
}
