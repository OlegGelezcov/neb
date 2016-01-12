// Skill_00000429.cs
// Nebula
//
// Created by Oleg Zheleztsov on Monday, September 21, 2015 1:26:42 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using Nebula.Engine;
using Nebula.Game.Bonuses;
using Nebula.Game.Components;
using Space.Game;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_00000429 : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            if(ShotToEnemyRestricted(source, skill)) {
                return false;
            }

            float dmgMult = skill.GetFloatInput("dmg_mult");
            float speedPc = skill.GetFloatInput("speed_pc");
            float speedTime = skill.GetFloatInput("speed_time");

            var weapon = source.Weapon();
            WeaponHitInfo hit;

            bool mastery = RollMastery(source);
            if(mastery) {
                dmgMult *= 2;
                speedTime *= 2;
            }

            var shot = weapon.Fire(out hit, skill.data.Id, dmgMult);
            if(hit.hitAllowed) {
                Buff buff = new Buff(skill.data.Id.ToString(), null, Common.BonusType.increase_speed_on_pc, speedTime, speedPc);
                source.Bonuses().SetBuff(buff);
                source.MmoMessage().SendShot(Common.EventReceiver.OwnerAndSubscriber, shot);
                return true;
             } else {
                source.MmoMessage().SendShot(Common.EventReceiver.ItemOwner, shot);
                return false;
            }
        }
    }
}
