﻿// Skill_00000411.cs
// Nebula
//
// Created by Oleg Zheleztsov on Sunday, September 20, 2015 7:00:31 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using Nebula.Engine;
using Nebula.Game.Components;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_00000411 : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            float healMult = skill.GetFloatInput("heal_mult");
            float radius = skill.GetFloatInput("radius");
            var weapon = source.Weapon();
            float damage = weapon.GetDamage(false);
            float totalHealing = healMult * damage;
            var items = GetHealTargets(source, source, radius);

            if(items.Count == 0 ) {
                return false;
            }

            float healPerItem = totalHealing / items.Count;
            bool mastery = RollMastery(source);
            if(mastery) {
                healPerItem *= 2;
            }

            var message = source.MmoMessage();
            foreach(var pitem in items) {
                var heal = weapon.Heal(pitem.Value, healPerItem, skill.data.Id);
                message.SendHeal(Common.EventReceiver.OwnerAndSubscriber, heal);
            }

            return true;
        }
    }
}
