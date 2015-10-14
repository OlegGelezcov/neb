// Skill_00000436.cs
// Nebula
//
// Created by Oleg Zheleztsov on Tuesday, August 4, 2015 4:37:17 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using Common;
using Nebula.Engine;
using Nebula.Game.Bonuses;
using Nebula.Game.Components;
using Space.Game;
using System.Collections;

namespace Nebula.Game.Skills {
    /// <summary>
    /// Increase the range of allies and the player by 15% for 8 seconds
    /// </summary>
    public class Skill_00000436 : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            if(!source) { return false; }

            float optimalDistancePc = skill.data.Inputs.Value<float>("optimal_distance_pc");
            float optimalDistanceTime = skill.data.Inputs.Value<float>("optimal_distance_time");
            float radius = skill.data.Inputs.Value<float>("radius");

            var sourceChar = source.GetComponent<CharacterObject>();

            var items = (source.world as MmoWorld).GetItems((it) => {
                if (it.GetComponent<PlayerBonuses>() && it.GetComponent<CharacterObject>()) {
                    float dist = source.transform.DistanceTo(it.transform);
                    if (dist < radius) {
                        if (sourceChar.RelationTo(it.GetComponent<CharacterObject>()) == FractionRelation.Friend) {
                            return true;
                        }
                    }
                }
                return false;
            });

            foreach(var p in items ) {
                Buff buff = new Buff(skill.data.Id.ToString(), null, BonusType.increase_optimal_distance_on_pc, optimalDistanceTime, optimalDistancePc);
                p.Value.GetComponent<PlayerBonuses>().SetBuff(buff);
            }
            return true;
        }
    }
}
