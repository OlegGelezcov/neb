using Common;
using Nebula.Engine;
using Nebula.Game.Bonuses;
using Nebula.Game.Components;
using Space.Game;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_000003F1 : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            if (!CheckForShotEnemy(source, skill)) {
                return false;
            }
            float dmgMult = skill.data.Inputs.GetValue<float>("dmg_mult", 0f);
            float blockHealInterval = skill.data.Inputs.GetValue<float>("block_heal_interval", 0f);
            var weapon = source.GetComponent<BaseWeapon>();
            WeaponHitInfo hit;
            var shot = weapon.Fire(out hit, skill.data.Id, dmgMult);
            if (hit.hitAllowed) {
                Buff buff = new Buff(skill.data.Id.ToString(), null, BonusType.block_heal, blockHealInterval);
                source.GetComponent<PlayerTarget>().targetObject.GetComponent<PlayerBonuses>().SetBuff(buff);
                source.GetComponent<MmoMessageComponent>().SendShot(EventReceiver.OwnerAndSubscriber, shot);
                return true;
            } else {
                source.GetComponent<MmoMessageComponent>().SendShot(EventReceiver.OwnerAndSubscriber, shot);
                return false;
            }
        }
    }
}
