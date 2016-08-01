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
    public class Skill_000003EC : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();

            float dmgMult = skill.data.Inputs.Value<float>("dmg_mult");
            float speed_pc = skill.data.Inputs.Value<float>("speed_pc");
            float speed_time = skill.data.Inputs.Value<float>("speed_time");
            string id = skill.data.Id.ToString();
            info.SetSkillUseState(SkillUseState.normal);
            if(!CheckForShotEnemy(source, skill)) {
                return false;
            }
            if (NotCheckDistance(source)) {
                info.SetSkillUseState(SkillUseState.tooFar);
                return false;
            }
            var sourceWeapon = source.GetComponent<BaseWeapon>();
            WeaponHitInfo hit;

            bool mastery = RollMastery(source);
            if(mastery) {
                dmgMult *= 2;
            }

            var shotInfo = sourceWeapon.GetComponent<BaseWeapon>().Fire(out hit, skill.data.Id, dmgMult);
            if(hit.normalOrMissed) {
                if(mastery) {
                    speed_time *= 2;
                }
                Buff buff = new Buff(id, null, BonusType.increase_speed_on_pc, speed_time, speed_pc);
                source.GetComponent<PlayerBonuses>().SetBuff(buff);
                source.GetComponent<MmoMessageComponent>().SendShot(EventReceiver.OwnerAndSubscriber, shotInfo);
                return true;
            } else {
                source.GetComponent<MmoMessageComponent>().SendShot(EventReceiver.OwnerAndSubscriber, shotInfo);
                return false;
            }
        }
    }
}
