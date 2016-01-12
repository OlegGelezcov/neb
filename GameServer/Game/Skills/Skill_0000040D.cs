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

            if (!CheckForShotEnemy(source, skill)) {
                return false;
            }
            var sourceWeapon = source.GetComponent<BaseWeapon>();
            float dmgMult = skill.data.Inputs.Value<float>("dmg_mult");
            float damagePc = skill.data.Inputs.Value<float>("damage_pc");
            float damageTime = skill.data.Inputs.Value<float>("damage_time");

            bool mastery = RollMastery(source);
            if(mastery) {
                dmgMult *= 2;
                damageTime *= 2;
            }
            WeaponHitInfo hit;
            var shotInfo = sourceWeapon.Fire(out hit, skill.data.Id, dmgMult);
            if (hit.hitAllowed) {
                Buff damageDebuff = new Buff(skill.data.Id.ToString(), null, BonusType.decrease_damage_on_pc, damageTime, damagePc);
                source.GetComponent<PlayerTarget>().targetObject.GetComponent<PlayerBonuses>().SetBuff(damageDebuff);
                source.GetComponent<MmoMessageComponent>().SendShot(EventReceiver.OwnerAndSubscriber, shotInfo);
                return true;
            } else {
                source.GetComponent<MmoMessageComponent>().SendShot(EventReceiver.OwnerAndSubscriber, shotInfo);
                return false;
            }
        }
    }
}
