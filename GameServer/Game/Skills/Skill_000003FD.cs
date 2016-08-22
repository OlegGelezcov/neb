// Skill_000003FD.cs
// Nebula
//
// Created by Oleg Zheleztsov on Friday, September 18, 2015 9:22:37 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using Nebula.Engine;
using Nebula.Game.Bonuses;
using Nebula.Game.Components;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_000003FD : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            float hpSpeed = skill.GetDataInput<float>("hp_speed", 0f);
            float hpSpeedTime = skill.GetDataInput<float>("hp_speed_time", 0f);
            float speedPc = skill.GetDataInput<float>("speed_pc", 0f);
            float speedTime = skill.GetDataInput<float>("speed_time", 0f);

            bool mastery = RollMastery(source);
            if(mastery) {
                hpSpeedTime *= 2;
                speedTime *= 2;
            }

            var bonuses = source.Bonuses();
            if(bonuses) {
                Buff healingBuff = new Buff(skill.data.Id.ToString(), null, Common.BonusType.increase_healing_speed_on_pc, hpSpeedTime, hpSpeed);
                Buff speedBuff = new Buff(skill.data.Id.ToString(), null, Common.BonusType.increase_speed_on_pc, speedTime, speedPc);
                bonuses.SetBuff(healingBuff, source);
                bonuses.SetBuff(speedBuff, source);
            }
            return true;
        }
    }
}
