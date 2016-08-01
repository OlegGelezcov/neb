using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nebula.Engine;
using Nebula.Game.Components;
using Common;
using Space.Game;

namespace Nebula.Game.Skills {
    public class Skill_000003FA : SkillExecutor  {
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

            float dmgMult = skill.data.Inputs.Value<float>("dmg_mult");
            float hpPc = skill.data.Inputs.Value<float>("hp_pc");

            var targetDamagable = source.GetComponent<PlayerTarget>().targetObject.GetComponent<DamagableObject>();

            if(targetDamagable.health > targetDamagable.maximumHealth * hpPc ) {
                return false;
            }

            bool mastery = RollMastery(source);
            if(mastery) {
                dmgMult *= 2;
            }

            WeaponHitInfo hit;
            var shotInfo = source.GetComponent<BaseWeapon>().Fire(out hit, skill.data.Id, dmgMult);
            if(hit.normalOrMissed) {
                source.GetComponent<MmoMessageComponent>().SendShot(EventReceiver.OwnerAndSubscriber, shotInfo);
                return true;
            } else {
                source.GetComponent<MmoMessageComponent>().SendShot(EventReceiver.OwnerAndSubscriber, shotInfo);
                return false;
            }
        }
    }
}
