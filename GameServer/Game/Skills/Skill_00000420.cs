// Skill_00000420.cs
// Nebula
//
// Created by Oleg Zheleztsov on Sunday, September 20, 2015 6:59:57 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using Nebula.Engine;
using Nebula.Game.Bonuses;
using Nebula.Game.Components;
using System.Collections;

namespace Nebula.Game.Skills {

    //speed debuff on target
    public class Skill_00000420 : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            if(ShotToEnemyRestricted(source, skill)) {
                return false;
            }

            float speedPc = skill.GetFloatInput("speed_pc");
            float speedTime = skill.GetFloatInput("speed_time");

            var target = source.Target().targetObject;
            var targetMovable = target.Movable();
            if(!targetMovable) {
                return false;
            }

            var sourceMovable = source.Movable();
            float speedValue = sourceMovable.maximumSpeed * speedPc;
            Buff buff = new Buff(skill.data.Id.ToString(), null, Common.BonusType.decrease_speed_on_cnt, speedTime, speedValue);
            target.Bonuses().SetBuff(buff);
            return true;
        }
    }
}
