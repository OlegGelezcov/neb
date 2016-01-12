// Skill_00000410.cs
// Nebula
//
// Created by Oleg Zheleztsov on Sunday, September 20, 2015 12:41:24 AM
// Copyright (c) 2015 KomarGames. All rights reserved.
//


namespace Nebula.Game.Skills {
    using Nebula.Engine;
    using Nebula.Game.Components;
    using System.Collections;

    public class Skill_00000410 : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            if(!CheckForHealAlly(source)) {
                return false;
            }
            float healMult = skill.GetFloatInput("heal_mult");
            float healAreaMult = skill.GetFloatInput("area_heal");
            float radius = skill.GetFloatInput("radius");

            float damage = source.Weapon().GetDamage(false);
            float targetHeal = damage * healMult;
            float areaHeal = damage * healAreaMult;

            bool mastery = RollMastery(source);
            if(mastery) {
                targetHeal *= 2;
                areaHeal *= 2;
            }
            var weapon = source.Weapon();
            var heal = weapon.Heal(source.Target().targetObject, targetHeal, skill.data.Id);

            var message = source.MmoMessage();
            message.SendHeal(Common.EventReceiver.OwnerAndSubscriber, heal);

            var items = GetHealTargets(source, source.Target().targetObject, radius);
            foreach(var pitem in items) {
                var additionalHeal = weapon.Heal(pitem.Value, areaHeal);
                message.SendHeal(Common.EventReceiver.OwnerAndSubscriber, additionalHeal);
            }
            return true;
        }
    }
}
