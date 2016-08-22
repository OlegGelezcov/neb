using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nebula.Engine;
using Nebula.Game.Components;
using Common;
using Space.Game;
using Nebula.Game.Bonuses;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_000003EB : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            float dmgMult = skill.data.Inputs.Value<float>("dmg_mult");
            float resistPc = skill.data.Inputs.Value<float>("resist_pc");
            float resistTime = skill.data.Inputs.Value<float>("resist_time");

            string debuffID = skill.data.Id.ToString();
            info.SetSkillUseState(SkillUseState.normal);
            if(!CheckForShotEnemy(source, skill)) {
                return false;
            }
            if (NotCheckDistance(source)) {
                info.SetSkillUseState(SkillUseState.tooFar);
                return false;
            }

            var sourceWeapon = source.GetComponent<BaseWeapon>();
            var targetBonuses = source.GetComponent<PlayerTarget>().targetObject.GetComponent<PlayerBonuses>();

            WeaponHitInfo hit;
            bool mastery = RollMastery(source);

            if(mastery) {
                dmgMult *= 2;
            }

            var shotInfo = sourceWeapon.GetComponent<BaseWeapon>().Fire(out hit, skill.data.Id, dmgMult);
            if(hit.normalOrMissed) {
                if(mastery) {
                    resistTime *= 2;
                }
                Buff resistDebuff = new Buff(debuffID, null, BonusType.decrease_resist_on_pc, resistTime, resistPc);
                targetBonuses.SetBuff(resistDebuff, source);
                source.GetComponent<MmoMessageComponent>().SendShot(EventReceiver.OwnerAndSubscriber, shotInfo);
                return true;
            } else {
                source.GetComponent<MmoMessageComponent>().SendShot(EventReceiver.OwnerAndSubscriber, shotInfo);
                return false;
            }
        }
    }
}
