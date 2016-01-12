// Skill_00000422.cs
// Nebula
//
// Created by Oleg Zheleztsov on Monday, September 21, 2015 12:03:15 AM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using Nebula.Engine;
using Nebula.Game.Components;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_00000422 : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            float odMult = skill.GetFloatInput("od_mult");

            bool mastery = RollMastery(source);
            if(mastery) {
                odMult *= 2;
            }

            var sourceTarget = source.Target();
            if(sourceTarget.targetObject) {
                var targetObject = sourceTarget.targetObject;

                var targetCharacter = targetObject.Character();
                if(targetCharacter) {
                    if(source.Character().RelationTo(targetCharacter) == Common.FractionRelation.Friend ) {

                        var sourceWeapon = source.Weapon();

                        if(source.transform.DistanceTo(targetObject.transform) <= odMult * sourceWeapon.optimalDistance) {

                            
                            source.MmoMessage().StartJumpToPosition(targetObject.transform.position, skill.data.Id );

                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
