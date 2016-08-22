// Skill_00000437.cs
// Nebula
//
// Created by Oleg Zheleztsov on Tuesday, August 4, 2015 5:08:47 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using Nebula.Engine;
using Nebula.Game.Components;
using Common;
using Space.Game;
using Nebula.Game.Bonuses;
using System.Collections;

namespace Nebula.Game.Skills {
    /// <summary>
    /// It creates around the ship in the area of interference 10 seconds, resulting in the field being applied to the opponents 10% less damage, 
    /// and the player inflicts on them an additional 10% weapon damage on every attack
    /// </summary>
    public class Skill_00000437 : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            if(!source) { return false; }
            float dmgDebuffPc = skill.data.Inputs.Value<float>("dmg_debuff_pc");
            float dmgDebuffTime = skill.data.Inputs.Value<float>("dmg_debuff_time");
            float dmgBuffPc = skill.data.Inputs.Value<float>("dmg_buff_pc");
            float radius = skill.data.Inputs.Value<float>("radius");

            bool mastery = RollMastery(source);
            if(mastery) {
                dmgDebuffTime *= 2;
            }

            var sourceBonuses = source.GetComponent<PlayerBonuses>();
            var sourceChar = source.GetComponent<CharacterObject>();

            var items = (source.world as MmoWorld).GetItems((it) => {
                if (it.GetComponent<PlayerBonuses>() && it.GetComponent<CharacterObject>()) {
                    float dist = source.transform.DistanceTo(it.transform);
                    if (dist < radius) {
                        if (sourceChar.RelationTo(it.GetComponent<CharacterObject>()) == FractionRelation.Enemy) {
                            return true;
                        }
                    }
                }
                return false;
            });

            foreach(var p in items ) {
                Buff buff = new Buff(skill.data.Id.ToString(), null, BonusType.decrease_damage_on_pc, dmgDebuffTime, dmgDebuffPc);
                p.Value.GetComponent<PlayerBonuses>().SetBuff(buff, source);
            }

            Buff dmgBuff = new Buff(skill.data.Id.ToString(), null, BonusType.increase_damage_on_pc, dmgDebuffTime, dmgBuffPc);
            sourceBonuses.SetBuff(dmgBuff, source);
            return true;
        }
    }
}
