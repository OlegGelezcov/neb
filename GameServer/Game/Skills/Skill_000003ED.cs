﻿using Common;
using Nebula.Engine;
using Nebula.Game.Components;
using Space.Game;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_000003ED : SkillExecutor  {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            float dmgMult = skill.data.Inputs.Value<float>("dmg_mult");
            float dmgPerSecPC = skill.data.Inputs.Value<float>("dmg_per_sec_pc");
            float dmgPerSecTime = skill.data.Inputs.Value<float>("dmg_per_sec_time");

            string id = skill.data.Id.ToString();

            info.SetSkillUseState(SkillUseState.normal);
            if (!CheckForShotEnemy(source, skill)) {
                info.SetSkillUseState(SkillUseState.invalidTarget);
                return false;
            }
            if(NotCheckDistance(source)) {
                info.SetSkillUseState(SkillUseState.tooFar);
                return false;
            }
            
            var sourceWeapon = source.GetComponent<BaseWeapon>();

            bool mastery = RollMastery(source);

            if(mastery) {
                dmgMult *= 2;
            }

            WeaponHitInfo hit;
            var shotInfo = sourceWeapon.GetComponent<BaseWeapon>().Fire(out hit, skill.data.Id, dmgMult);
            if (hit.normalOrMissed) {
                var targ = source.GetComponent<PlayerTarget>().targetObject;
                if(targ ) {
                    if(mastery) {
                        dmgPerSecTime *= 2;
                    }

                    targ.GetComponent<DamagableObject>().SetTimedDamage(dmgPerSecTime, hit.actualDamage.totalDamage * dmgPerSecPC, sourceWeapon.myWeaponBaseType, skill.idInt);
                }
                source.GetComponent<MmoMessageComponent>().SendShot(EventReceiver.OwnerAndSubscriber, shotInfo);
                return true;
            } else {
                source.GetComponent<MmoMessageComponent>().SendShot(EventReceiver.OwnerAndSubscriber, shotInfo);
                return false;
            }
        }
    }
}
