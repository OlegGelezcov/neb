// Skill_00000426.cs
// Nebula
//
// Created by Oleg Zheleztsov on Monday, September 21, 2015 12:35:28 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using Nebula.Engine;
using Nebula.Game.Bonuses;
using Nebula.Game.Components;
using Space.Game;
using System.Collections;

namespace Nebula.Game.Skills {

    //shot with buff
    public class Skill_00000426 : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            if (ShotToEnemyRestricted(source, skill)) {
                return false;
            }

            float dmgMult = skill.GetFloatInput("dmg_mult");
            float critDmgPc = skill.GetFloatInput("crit_dmg_mult");
            float critDmgTime = skill.GetFloatInput("crit_dmg_time");

            var weapon = source.Weapon();
            var bonuses = source.Bonuses();

            bool mastery = RollMastery(source);
            if(mastery) {
                dmgMult *= 2;
                critDmgTime *= 2;
            }

            WeaponHitInfo hit;
            var shot = weapon.Fire(out hit, skill.data.Id, dmgMult);
            if(hit.hitAllowed) {
                Buff buff = new Buff(skill.data.Id.ToString(), null, Common.BonusType.increase_crit_damage_on_pc, critDmgTime, critDmgPc);
                bonuses.SetBuff(buff);
                source.MmoMessage().SendShot(Common.EventReceiver.OwnerAndSubscriber, shot);
                return true;
            } else {
                source.MmoMessage().SendShot(Common.EventReceiver.ItemOwner, shot);
                return false;
            }
        }
    }
}
