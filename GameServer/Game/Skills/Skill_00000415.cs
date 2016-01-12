// Skill_00000415.cs
// Nebula
//
// Created by Oleg Zheleztsov on Sunday, September 20, 2015 12:46:43 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using Nebula.Engine;
using Nebula.Game.Components;
using System.Collections;

namespace Nebula.Game.Skills {

    //heal self when healing others
    public class Skill_00000415 : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            float hpPc = skill.GetFloatInput("hp_pc");
            float hpTime = skill.GetFloatInput("hp_time");

            bool mastery = RollMastery(source);
            if(mastery) {
                hpTime *= 2;
            }
            source.Skills().Set415(hpTime, hpPc);
            return true;
        }
    }
}
