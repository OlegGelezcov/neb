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
            info.SetSkillUseState(Common.SkillUseState.normal);

            //if(!CheckForHealAlly(source)) {
            //    info.SetSkillUseState(Common.SkillUseState.invalidTarget);
            //    return false;
            //}
            //if(NotCheckDistance(source)) {
            //    info.SetSkillUseState(Common.SkillUseState.tooFar);
            //    return false;
            //}

            if(!source) {
                info.SetSkillUseState(Common.SkillUseState.invalidTarget);
                return false;
            }

            float healMult = skill.GetFloatInput("heal_mult");
            float healAreaMult = skill.GetFloatInput("area_heal");
            float radius = skill.GetFloatInput("radius");

            float damage = source.Weapon().GetDamage().totalDamage;
            float selfHeal = damage * healMult;
            float areaHeal = damage * healAreaMult;

            bool mastery = RollMastery(source);
            if(mastery) {
                info.SetMastery(true);
                selfHeal *= 2;
                areaHeal *= 2;
            } else {
                info.SetMastery(false);
            }

            var weapon = source.Weapon();
            var message = source.MmoMessage();

            foreach(var friend in GetNearestFriends(source, radius)) {
                if(friend.Value.Id == source.Id ) {
                    message.SendHeal(Common.EventReceiver.OwnerAndSubscriber, weapon.HealSelf(selfHeal, skill.idInt));
                } else {
                    message.SendHeal(Common.EventReceiver.OwnerAndSubscriber, weapon.Heal(friend.Value, areaHeal, skill.idInt));
                }
            }
            return true;
            /*
            var weapon = source.Weapon();
            var heal = weapon.Heal(source.Target().targetObject, targetHeal, skill.data.Id);

            var message = source.MmoMessage();
            message.SendHeal(Common.EventReceiver.OwnerAndSubscriber, heal);

            var items = GetHealTargets(source, source.Target().targetObject, radius);
            foreach(var pitem in items) {
                var additionalHeal = weapon.Heal(pitem.Value, areaHeal);
                message.SendHeal(Common.EventReceiver.OwnerAndSubscriber, additionalHeal);
            }
            return true;*/
        }
    }
}
