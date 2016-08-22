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
            info.SetSkillUseState(SkillUseState.normal);
            if (!CheckForShotEnemy(source, skill)) {
                info.SetSkillUseState(SkillUseState.invalidTarget);
                return false;
            }
            if (NotCheckDistance(source)) {
                info.SetSkillUseState(SkillUseState.tooFar);
                return false;
            }

            float dmgMult = skill.data.Inputs.GetValue<float>("dmg_mult", 0f);
            float blockWeaponInterval = skill.data.Inputs.GetValue<float>("block_weapon_interval", 0f);

            bool mastery = RollMastery(source);
            if(mastery) {
                dmgMult *= 2;
            }
            var weapon = source.GetComponent<BaseWeapon>();
            WeaponHitInfo hit;
            var shot = weapon.Fire(out hit, skill.data.Id, dmgMult);
            if(hit.normalOrMissed) {
                if(mastery) {
                    blockWeaponInterval *= 2;
                }
                Buff blockWeaponBuff = new Buff(skill.data.Id.ToString(), null, BonusType.block_weapon, blockWeaponInterval);
                source.GetComponent<PlayerTarget>().targetObject.GetComponent<PlayerBonuses>().SetBuff(blockWeaponBuff, source);
                source.GetComponent<MmoMessageComponent>().SendShot(EventReceiver.OwnerAndSubscriber, shot);
                return true;
            } else {
                source.GetComponent<MmoMessageComponent>().SendShot(EventReceiver.OwnerAndSubscriber, shot);
                return false;
            }
        }
    }
}
