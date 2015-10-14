// Skill_0000041A.cs
// Nebula
//
// Created by Oleg Zheleztsov on Sunday, September 20, 2015 3:33:30 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using Nebula.Engine;
using Nebula.Game.Components;
using System.Collections;

namespace Nebula.Game.Skills {

    //remove any buff from enemy
    public class Skill_0000041A : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            if(!CheckForShotEnemy(source, skill)) {
                return false;
            }

            var targetBonuses = source.Target().targetObject.Bonuses();
            targetBonuses.RemoveAnyBuff();
            return true;
        }
    }
}
