// Skill_0000043C.cs
// Nebula
//
// Created by Oleg Zheleztsov on Tuesday, August 4, 2015 5:50:04 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using Nebula.Engine;
using Nebula.Game.Components;
using Common;
using Nebula.Game.Bonuses;
using System.Collections;

namespace Nebula.Game.Skills {
    /// <summary>
    /// If the index HP player below 20% recovery time of all abilities reduced by 10% for 15 seconds and Resistance figure increased by 20% from the current value to 8 seconds
    /// </summary>
    public class Skill_0000043C : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            if(!source) { return false; }

            float hpLevelPc = skill.data.Inputs.Value<float>("hp_level_pc");
            float cooldownPc = skill.data.Inputs.Value<float>("cooldown_pc");
            float cooldownTime = skill.data.Inputs.Value<float>("cooldown_time");
            float resistPc = skill.data.Inputs.Value<float>("resist_pc");
            float resistTime = skill.data.Inputs.Value<float>("resist_time");

            var damagable = source.GetComponent<DamagableObject>();
            var bonuses = source.GetComponent<PlayerBonuses>();

            float hp = damagable.maximumHealth * hpLevelPc;
            if(damagable.health >= hp ) {
                return false;
            }

            bool mastery = RollMastery(source);
            if(mastery) {
                cooldownTime *= 2;
                resistTime *= 2;
            }

            Buff cooldownBuff = new Buff(skill.data.Id.ToString(), null, BonusType.decrease_cooldown_on_pc, cooldownTime, cooldownPc);
            Buff resistBuff = new Buff(skill.data.Id.ToString(), null, BonusType.increase_resist_on_pc, resistTime, resistPc);
            bonuses.SetBuff(cooldownBuff, source);
            bonuses.SetBuff(resistBuff, source);
            return true;
        }
    }
}
