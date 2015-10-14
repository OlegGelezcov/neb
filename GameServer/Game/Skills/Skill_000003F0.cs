using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nebula.Engine;
using Nebula.Game.Components;
using Common;
using Space.Game;
using Nebula.Game.Bonuses;

namespace Nebula.Game.Skills {
    public class Skill_000003F0  : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            if(!CheckForShotEnemy(source, skill)) {
                return false;
            }
            float dmgMult = skill.data.Inputs.GetValue<float>("dmg_mult", 0f);
            float blockWeaponInterval = skill.data.Inputs.GetValue<float>("block_weapon_interval", 0f);

            var weapon = source.GetComponent<BaseWeapon>();
            WeaponHitInfo hit;
            var shot = weapon.Fire(out hit, skill.data.Id, dmgMult);
            if(hit.hitAllowed) {
                Buff blockWeaponBuff = new Buff(skill.data.Id.ToString(), null, BonusType.block_weapon, blockWeaponInterval);
                source.GetComponent<PlayerTarget>().targetObject.GetComponent<PlayerBonuses>().SetBuff(blockWeaponBuff);
                source.GetComponent<MmoMessageComponent>().SendShot(EventReceiver.OwnerAndSubscriber, shot);
                return true;
            } else {
                source.GetComponent<MmoMessageComponent>().SendShot(EventReceiver.OwnerAndSubscriber, shot);
                return false;
            }
        }
    }
}
