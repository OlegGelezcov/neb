// Skill_0000041A.cs
// Nebula
//
// Created by Oleg Zheleztsov on Sunday, September 20, 2015 3:33:30 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using Common;
using Nebula.Engine;
using Nebula.Game.Components;
using ServerClientCommon;
using System.Collections;

namespace Nebula.Game.Skills {

    //remove any buff from enemy
    public class Skill_0000041A : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            info.SetSkillUseState(SkillUseState.normal);

            if(!CheckForShotEnemy(source, skill)) {
                info.SetSkillUseState(SkillUseState.invalidTarget);
                return false;
            }

            if(NotCheckDistance(source)) {
                info.SetSkillUseState(SkillUseState.tooFar);
                return false;
            }

            info.SetMastery(false);

            var targetBonuses = source.Target().targetObject.Bonuses();
            targetBonuses.RemoveAnyBuff();
            info.Add((int)SPC.Target, targetBonuses.nebulaObject.Id);
            info.Add((int)SPC.TargetType, targetBonuses.nebulaObject.Type);

            return true;
        }
    }
}
