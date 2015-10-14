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
            if(!source) {
                return false;
            }
            float cooldownCnt = skill.data.Inputs.Value<float>("cooldown_cnt");
            float cooldownTime = skill.data.Inputs.Value<float>("cooldown_time");
            float radius = skill.data.Inputs.Value<float>("radius");

            var sourceChar = source.GetComponent<PlayerCharacterObject>();

            var items = (source.world as MmoWorld).GetItems((it) => {
                if (it.GetComponent<PlayerBonuses>() && it.GetComponent<CharacterObject>()) {
                    if (source.transform.DistanceTo(it.transform) <= radius) {
                        if (sourceChar.RelationTo(it.GetComponent<CharacterObject>()) == FractionRelation.Enemy) {
                            return true;
                        }
                    }
                }
                return false;
            });

            foreach(var p in items ) {
                Buff buff = new Buff(skill.data.Id.ToString(), null, BonusType.increase_cooldown_on_cnt, cooldownTime, cooldownCnt);
                p.Value.GetComponent<PlayerBonuses>().SetBuff(buff);
            }
            return true;
        }
    }
}
