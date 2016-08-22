// Skill_0000043D.cs
// Nebula
//
// Created by Oleg Zheleztsov on Tuesday, August 4, 2015 8:29:58 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using Common;
using Nebula.Engine;
using Nebula.Game.Bonuses;
using Nebula.Game.Components;
using System.Collections;

/// <summary>
/// Duration of effects already imposed on a player or those that will be imposed within 10 seconds after application of skills associated with the slowdown, reduced by 30%
/// </summary>
namespace Nebula.Game.Skills {
    public class Skill_0000043D : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();

            if(!source) { return false; }

            var bonuses = source.GetComponent<PlayerBonuses>();

            float time = skill.data.Inputs.Value<float>("time");
            float timePc = skill.data.Inputs.Value<float>("time_pc");

            float mult = 1f - timePc;
            bonuses.MultInterval(BonusType.decrease_speed_on_cnt, mult);
            bonuses.MultInterval(BonusType.decrease_speed_on_pc, mult);

            bool mastery = RollMastery(source);
            if(mastery) {
                time *= 2;
            }

            Buff buff = new Buff(skill.data.Id.ToString(), null, BonusType.decrease_time_of_negative_speed_buffs, time, mult);
            bonuses.SetBuff(buff, source);
            return true;
        }
    }
}
