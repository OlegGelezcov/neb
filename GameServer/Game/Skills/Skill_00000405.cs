using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nebula.Engine;
using Nebula.Game.Components;
using Nebula.Game.Bonuses;

namespace Nebula.Game.Skills {
    public class Skill_00000405 : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            var sourceTarget = source.Target();

            bool settedOnTarget = false;
            float time = skill.GetFloatInput("time");

            if(sourceTarget.targetObject) {
                var targetCharacter = sourceTarget.targetObject.Character();
                if(targetCharacter) {
                    var sourceCharacter = source.Character();
                    if(sourceCharacter.RelationTo(targetCharacter) == Common.FractionRelation.Friend) {
                        var sourceWeapon = source.Weapon();
                        if(source.transform.DistanceTo(sourceTarget.targetObject.transform) <= sourceWeapon.optimalDistance) {
                            var targetBonuses = sourceTarget.targetObject.Bonuses();
                            if(targetBonuses) {
                                Buff buff = new Buff(skill.data.Id.ToString(), null, Common.BonusType.speed_debuff_immunity, time);
                                targetBonuses.SetBuff(buff);
                                settedOnTarget = true;
                                
                            }
                        }
                    }
                }
            }

            if(!settedOnTarget) {
                Buff buff = new Buff(skill.data.Id.ToString(), null, Common.BonusType.speed_debuff_immunity, time);
                source.Bonuses().SetBuff(buff);
                
            }
            return true;
        }
    }
}
