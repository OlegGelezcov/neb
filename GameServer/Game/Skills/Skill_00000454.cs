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
using ServerClientCommon;

namespace Nebula.Game.Skills {
    public class Skill_00000454 : SkillExecutor  {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();

            if (NotEnemyCheck(source, skill, info)) {
                return false;
            }

            float dmgMult = skill.data.Inputs.Value<float>("dmg_mult");
            float dmgTime = skill.data.Inputs.Value<float>("dmg_time");
            int dmgMultCount = skill.data.Inputs.Value<int>("dmg_mult_count");
            float dmgMultMult = skill.data.Inputs.Value<float>("dmg_mult_mult");

            var sourceWeapon = source.GetComponent<BaseWeapon>();
            var sourceSkills = source.GetComponent<PlayerSkills>();
            var targetDamagable = source.GetComponent<PlayerTarget>().targetObject.GetComponent<DamagableObject>();
            var sourceBonuses = source.GetComponent<PlayerBonuses>();

            if(targetDamagable.transform.DistanceTo(source.transform) > sourceWeapon.optimalDistance) {
                return false;
            }

            bool mastery = RollMastery(source);
            if(mastery) {
                dmgTime *= 2;
            }

            if ((sourceSkills.lastSkill != skill.data.Id) || 
                (sourceSkills.lastSkill == skill.data.Id && sourceSkills.sequenceSkillCounter == 0)) {

                Buff buff = new Buff(skill.data.Id.ToString() + "0", null, BonusType.increase_damage_on_pc, dmgTime, dmgMultMult);
                sourceBonuses.SetBuff(buff);
                var dmg = sourceWeapon.GetDamage(false).totalDamage * dmgMult;
                float dmgPerSec = dmg / dmgTime;
                targetDamagable.SetTimedDamage(dmgTime, dmgPerSec, sourceWeapon.myWeaponBaseType);
                info.Add((int)SPC.Damage, dmg);
            } else if(sourceSkills.lastSkill == skill.data.Id && sourceSkills.sequenceSkillCounter == 1) {
                Buff buff = new Buff(skill.data.Id.ToString() + "1", null, BonusType.increase_damage_on_pc, dmgTime, dmgMultMult);
                sourceBonuses.SetBuff(buff);
                var dmg = sourceWeapon.GetDamage(false).totalDamage * dmgMult;
                float dmgPerSec = dmg / dmgTime;
                targetDamagable.SetTimedDamage(dmgTime, dmgPerSec, sourceWeapon.myWeaponBaseType);
                info.Add((int)SPC.Damage, dmg);
            } else if(sourceSkills.lastSkill == skill.data.Id && sourceSkills.sequenceSkillCounter >= 2 ) {
                Buff buff = new Buff(skill.data.Id.ToString() + "2", null, BonusType.increase_damage_on_pc, dmgTime, dmgMultMult);
                sourceBonuses.SetBuff(buff);
                var dmg = sourceWeapon.GetDamage(false).totalDamage * dmgMult;
                float dmgPerSec = dmg / dmgTime;
                targetDamagable.SetTimedDamage(dmgTime, dmgPerSec, sourceWeapon.myWeaponBaseType);
                sourceSkills.SetSkillCounter(0);
                info.Add((int)SPC.Damage, dmg);
            }

            return true;
        }
    }
}
