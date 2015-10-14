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
            if(!CheckForShotEnemy(source, skill)) {
                return false;
            }
            var sourceWeapon = source.GetComponent<BaseWeapon>();
            var targetBonuses = source.GetComponent<PlayerTarget>().targetObject.GetComponent<PlayerBonuses>();

            WeaponHitInfo hit;
            var shotInfo = sourceWeapon.GetComponent<BaseWeapon>().Fire(out hit, skill.data.Id, dmgMult);
            if(hit.hitAllowed) {
                Buff resistDebuff = new Buff(debuffID, null, BonusType.decrease_resist_on_pc, resistTime, resistPc);
                targetBonuses.SetBuff(resistDebuff);
                source.GetComponent<MmoMessageComponent>().SendShot(EventReceiver.OwnerAndSubscriber, shotInfo);
                return true;
            } else {
                source.GetComponent<MmoMessageComponent>().SendShot(EventReceiver.OwnerAndSubscriber, shotInfo);
                return false;
            }
        }
    }
}
