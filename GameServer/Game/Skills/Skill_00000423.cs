// Skill_00000423.cs
// Nebula
//
// Created by Oleg Zheleztsov on Monday, September 21, 2015 10:54:05 AM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using Nebula.Engine;
using Nebula.Game.Components;
using System.Collections;

namespace Nebula.Game.Skills {

    //make visible invisible objects
    public class Skill_00000423 : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            float radius = skill.GetFloatInput("radius");
            bool mastery = RollMastery(source);
            if(mastery) {
                radius *= 2;
            }

            var targets = GetTargets(source, source, radius);
            foreach(var pTarget in targets) {
                var item = pTarget.Value;
                if(item.invisible) {
                    item.SetInvisibility(false);
                }
            }

            return true;
        }
    }
}
