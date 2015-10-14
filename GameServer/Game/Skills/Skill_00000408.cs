using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nebula.Engine;
using Nebula.Game.Components;
using Space.Game;
using Nebula.Game.Bonuses;

namespace Nebula.Game.Skills {
    public class Skill_00000408 : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            if(!CheckForShotEnemy(source, skill)) {
                return false;
            }

            float dmgMult = skill.GetFloatInput("dmg_mult");
            float critChancePc = skill.GetFloatInput("crit_chance_pc");
            float critChanceTime = skill.GetFloatInput("time");

            WeaponHitInfo hit;
            var shot = source.Weapon().Fire(out hit, skill.data.Id, dmgMult);
            if(hit.hitAllowed) {
                Buff critChanceDebuff = new Buff(skill.data.Id.ToString(), null, Common.BonusType.decrease_crit_chance_on_pc, critChanceTime, critChancePc);
                source.Target().targetObject.Bonuses().SetBuff(critChanceDebuff);
                source.MmoMessage().SendShot(Common.EventReceiver.OwnerAndSubscriber, shot);
                return true;
            } else {
                source.MmoMessage().SendShot(Common.EventReceiver.OwnerAndSubscriber, shot);
                return false;
            }
        }
    }
}
