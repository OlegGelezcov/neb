using Common;
using GameMath;
using Nebula.Engine;
using Nebula.Game.Bonuses;
using Nebula.Game.Components;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_000003FB : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            if(!source) {
                return false;
            }
            var sourceTarget = source.GetComponent<PlayerTarget>();
            if(!sourceTarget) {
                return false;
            }

            if(!sourceTarget.hasTarget) {
                return false;
            }

            if(!sourceTarget.targetObject) {
                return false;
            }

            var sourceCharacter = source.GetComponent<CharacterObject>();
            if(!sourceCharacter) {
                return false;
            }

            var targetCharacter = sourceTarget.targetObject.GetComponent<CharacterObject>();
            if(!targetCharacter) {
                return false;
            }

            //affect only friends
            if(sourceCharacter.RelationTo(targetCharacter) == Common.FractionRelation.Enemy) {
                return false;
            }

            if(Mathf.Approximately(source.GetComponent<BaseWeapon>().HitProbTo(sourceTarget.targetObject), 0f)) {
                return false;
            }

            var targetBonuses = sourceTarget.targetObject.GetComponent<PlayerBonuses>();
            if(!targetBonuses) {
                return false;
            }

            string buffID = source.Id + skill.data.Id;
            float debuffDamagePercent = skill.data.Inputs.Value<float>("db_dmg_pc");
            float debuffDamageInterval = skill.data.Inputs.Value<float>("db_dmg_interval");

            Buff damageImmunityBuff = new Buff(buffID, null, BonusType.damage_immunity, debuffDamageInterval);
            Buff damageDebuff = new Buff(buffID, null, BonusType.decrease_damage_on_pc, debuffDamageInterval, debuffDamagePercent);
            targetBonuses.SetBuff(damageImmunityBuff);
            //targetBonuses.SetBuff(damageDebuff);
            source.GetComponent<PlayerBonuses>().SetBuff(damageDebuff);
            return true;
        }
    }
}
