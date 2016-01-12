using Nebula.Engine;
using Nebula.Game.Bonuses;
using Nebula.Game.Components;
using System;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_0000043E : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            if(ShotToEnemyRestricted(source, skill)) {
                return false;
            }

            float radius = skill.GetFloatInput("radius");
            var targetObject = source.Target().targetObject;

            var buffInfoCollection = targetObject.Bonuses().GetAllDebuffInfo();

            var items = GetHealTargets(targetObject, targetObject, radius);

            float buffMult = 1;
            bool mastery = RollMastery(source);
            if(mastery) {
                buffMult *= 2;
            }

            foreach (var pItem in items) {
                var itemBonuses = pItem.Value.Bonuses();
                foreach(var buffInfo in buffInfoCollection) {
                    itemBonuses.SetBuff(new Buff(Guid.NewGuid().ToString(), null, buffInfo.bonusType, buffInfo.time * buffMult, buffInfo.value));
                }
            }
            return true;
        }
    }
}
