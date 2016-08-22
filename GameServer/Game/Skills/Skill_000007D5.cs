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
    public class Skill_000007D5 : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            info.SetSkillUseState(SkillUseState.normal);

            float time = skill.GetFloatInput("time");
            float dmgPc = skill.GetFloatInput("dmg_pc");
            float healPc = skill.GetFloatInput("heal_pc");

            bool mastery = RollMastery(source);
            if(mastery) {
                time *= 2;
                info.SetMastery(true);
            } else {
                info.SetMastery(false);
            }

            //DamagableObject targetDamagable = source.GetComponent<PlayerTarget>().targetObject.GetComponent<DamagableObject>();

            //source.GetComponent<PlayerSkills>().AddDamageHealReceiver(new PlayerSkills.DamageReceiver { damagePercent = dmgPc, expireTime = Time.curtime() + time, target = targetDamagable });

            Buff hbuff = new Buff(skill.id + "_heal", null, BonusType.increase_healing_speed_on_pc, time, healPc);
            Buff dbuff = new Buff(skill.id + "_dmg", null, BonusType.increase_damage_on_pc, time, dmgPc);
            source.Bonuses().SetBuff(hbuff, source);
            source.Bonuses().SetBuff(dbuff, source);

            return true;
        }
    }
}
