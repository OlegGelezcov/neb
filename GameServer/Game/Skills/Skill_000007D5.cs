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
    public class Skill_000007D5 : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            if(!source) {
                return false;
            }
            if(!CheckForShotEnemy(source, skill)) {
                return false;
            }

            float time = skill.data.Inputs.Value<float>("time");
            float dmgPc = skill.data.Inputs.Value<float>("dmg_pc");

            DamagableObject targetDamagable = source.GetComponent<PlayerTarget>().targetObject.GetComponent<DamagableObject>();

            source.GetComponent<PlayerSkills>().AddDamageHealReceiver(new PlayerSkills.DamageReceiver { damagePercent = dmgPc, expireTime = Time.curtime() + time, target = targetDamagable });
            return true;
        }
    }
}
