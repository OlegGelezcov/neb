// Skill_00000422.cs
// Nebula
//
// Created by Oleg Zheleztsov on Monday, September 21, 2015 12:03:15 AM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using Common;
using Nebula.Engine;
using Nebula.Game.Components;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_00000422 : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            info.SetSkillUseState(SkillUseState.normal);

            float odMult = skill.GetFloatInput("od_mult");

            bool mastery = RollMastery(source);
            if(mastery) {
                odMult *= 2;
                info.SetMastery(true);
            } else {
                info.SetMastery(false);
            }

            var sourceTarget = source.Target();

            if(sourceTarget.targetObject) {

                var targetObject = sourceTarget.targetObject;

                var targetCharacter = targetObject.Character();
                if (targetCharacter) {

                    var relation = source.Character().RelationTo(targetCharacter);

                    if (relation == Common.FractionRelation.Friend  ) {

                        var sourceWeapon = source.Weapon();

                        if (source.transform.DistanceTo(targetObject.transform) <= odMult * sourceWeapon.optimalDistance) {


                            source.MmoMessage().StartJumpToPosition(targetObject.transform.position, skill.data.Id);

                            return true;

                        } else {
                            info.SetSkillUseState(SkillUseState.tooFar);
                        }
                    } else {
                        info.SetSkillUseState(SkillUseState.invalidTarget);
                    }
                } else {
                    info.SetSkillUseState(SkillUseState.invalidTarget);
                }
            } else {
                info.SetSkillUseState(SkillUseState.invalidTarget);
            }
            return false;
        }
    }
}
