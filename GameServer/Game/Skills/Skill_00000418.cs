using Common;
using Nebula.Engine;
using Nebula.Game.Bonuses;
using Nebula.Game.Components;
using Space.Game;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_00000418 : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            info.SetSkillUseState(SkillUseState.normal);

            if(!source) {
                return false;
            }
            float cooldownPc = skill.GetFloatInput("cooldown_pc");
            float cooldownTime = skill.GetFloatInput("cooldown_time");
            float radius = skill.GetFloatInput("radius");

            var sourceChar = source.GetComponent<CharacterObject>();

            //var items = (source.world as MmoWorld).GetItems((it) => {
            //    if(!source) {
            //        return false;
            //    }
            //    if(!it) {
            //        return false;
            //    }

            //    if (it.GetComponent<PlayerBonuses>() && it.GetComponent<CharacterObject>()) {
            //        if (source.transform.DistanceTo(it.transform) <= radius) {
            //            if (sourceChar.RelationTo(it.GetComponent<CharacterObject>()) == FractionRelation.Enemy) {
            //                return true;
            //            }
            //        }
            //    }
            //    return false;
            //});

            var enemies = GetEnemiesAndNeutrals(source, radius);


            bool mastery = RollMastery(source);
            if(mastery) {
                cooldownTime *= 2;
                info.SetMastery(true);
            } else {
                info.SetMastery(false);
            }


            foreach (var p in enemies ) {
                Buff buff = new Buff(skill.idInt.ToString(), null, BonusType.increase_cooldown_on_pc, cooldownTime, cooldownPc);
                //Buff buff = new Buff(skill.data.Id.ToString(), null, BonusType.increase_cooldown_on_cnt, cooldownTime, cooldownCnt);
                p.Value.GetComponent<PlayerBonuses>().SetBuff(buff, source);
            }
            return true;
        }
    }
}
