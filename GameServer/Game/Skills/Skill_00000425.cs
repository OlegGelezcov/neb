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
    public class Skill_00000425 : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();

            info.SetSkillUseState(SkillUseState.normal);
            if(!CheckForShotEnemy(source, skill)) {
                info.SetSkillUseState(SkillUseState.invalidTarget);
                return false; }
            if(NotCheckDistance(source)) {
                info.SetSkillUseState(SkillUseState.tooFar);
                return false;
            }

            float dmgMult = skill.data.Inputs.Value<float>("dmg_mult");
            float resistPc = skill.data.Inputs.Value<float>("resist_pc");
            float resistTime = skill.data.Inputs.Value<float>("resist_time");

            var sourceWeapon = source.GetComponent<BaseWeapon>();
            var targetBonuses = source.GetComponent<PlayerTarget>().targetObject.GetComponent<PlayerBonuses>();

            bool mastery = RollMastery(source);
            if(mastery) {
                dmgMult *= 2;
                resistTime *= 2;
            }

            WeaponHitInfo hit;
            var shotInfo = sourceWeapon.Fire(out hit, skill.data.Id, dmgMult);
            if(hit.normalOrMissed) {
                Buff buff = new Buff(skill.data.Id.ToString(), null, BonusType.decrease_resist_on_pc, resistTime, resistPc);
                targetBonuses.SetBuff(buff, source);
                source.GetComponent<MmoMessageComponent>().SendShot(EventReceiver.OwnerAndSubscriber, shotInfo);
                return true;
            } else {
                source.GetComponent<MmoMessageComponent>().SendShot(EventReceiver.OwnerAndSubscriber, shotInfo);
                return false;
            }
        }
    }
}
