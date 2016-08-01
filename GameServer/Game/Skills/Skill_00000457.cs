// Skill_00000457.cs
// Nebula
//
// Created by Oleg Zheleztsov on Saturday, September 26, 2015 4:05:20 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
namespace Nebula.Game.Skills {
    using Nebula.Engine;
    using Nebula.Game.Bonuses;
    using Nebula.Game.Components;
    using System.Collections;
    /// <summary>
    /// Remove single positive buff from target and from source, and make buff on crit damage and crit chance to source
    /// </summary>
    public class Skill_00000457 : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            if (NotEnemyCheck(source, skill, info)) {
                return false;
            }

            var sourceBonuses = source.Bonuses();
            var targetObject = source.Target().targetObject;
            var targetBonuses = targetObject.Bonuses();

            sourceBonuses.RemoveAnyPositiveBuff();
            targetBonuses.RemoveAnyPositiveBuff();

            float critDamagePc = skill.GetFloatInput("crit_dmg_pc");
            float critChancePc = skill.GetFloatInput("crit_chance_pc");
            float time = skill.GetFloatInput("time");

            bool mastery = RollMastery(source);
            if(mastery) {
                time *= 2;
            }

            Buff critDamageBuff = new Buff(skill.id, null, Common.BonusType.increase_crit_damage_on_pc, time, critDamagePc);
            Buff critChanceBuff = new Buff(skill.id, null, Common.BonusType.increase_crit_chance_on_cnt, time, critChancePc);
            sourceBonuses.SetBuff(critDamageBuff);
            sourceBonuses.SetBuff(critChanceBuff);
            return true;
        }
    }
}
