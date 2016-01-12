// Skill_000007D6.cs
// Nebula
//
// Created by Oleg Zheleztsov on Monday, September 21, 2015 11:09:16 AM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using Nebula.Engine;
using Nebula.Game.Components;
using Space.Game;
using System.Collections;

namespace Nebula.Game.Skills {

    //Heal allies and damage on enemies
    public class Skill_000007D6 : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            float radius = skill.GetFloatInput("radius");
            float healMult = skill.GetFloatInput("heal_mult");
            float dmgMult = skill.GetFloatInput("dmg_mult");

            if(!CheckForHealAlly(source)) {
                return false;
            }

            bool mastery = RollMastery(source);
            if(mastery) {
                healMult *= 2;
                dmgMult *= 2;
            }

            var weapon = source.Weapon();
            var message = source.MmoMessage();
            var targetObject = source.Target().targetObject;

            var healValue = weapon.GetDamage(false) * healMult;
            var firstHeal = weapon.Heal(targetObject, healValue, skill.data.Id);
            message.SendHeal(Common.EventReceiver.OwnerAndSubscriber, firstHeal);

            var healTargets = GetHealTargets(source, targetObject, radius);
            foreach(var pHealTarget in healTargets) {
                var secondHeal = weapon.Heal(pHealTarget.Value, healValue, skill.data.Id);
                message.SendHeal(Common.EventReceiver.OwnerAndSubscriber, secondHeal);
                break;
            }

            var dmgTargets = GetTargets(source, targetObject, radius);
            foreach(var pDmgTarget in dmgTargets) {
                WeaponHitInfo hit;
                var shot = weapon.Fire(pDmgTarget.Value, out hit, skill.data.Id, dmgMult);
                message.SendShot(Common.EventReceiver.OwnerAndSubscriber, shot);
            }
            return true;
        }
    }
}
