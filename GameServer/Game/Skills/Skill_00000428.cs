// Skill_00000428.cs
// Nebula
//
// Created by Oleg Zheleztsov on Monday, September 21, 2015 1:15:31 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using Nebula.Engine;
using Nebula.Game.Bonuses;
using Nebula.Game.Components;
using Space.Game;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_00000428 : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            if(ShotToEnemyRestricted(source, skill)) {
                return false;
            }

            float dmgMult =     skill.GetFloatInput("dmg_mult");
            float dmgPc =       skill.GetFloatInput("dmg_pc");
            float dmgTime =     skill.GetFloatInput("dmg_time");

            var sourceWeapon = source.Weapon();
            var targetObject = source.Target().targetObject;
            var targetBonuses = targetObject.Bonuses();

            bool mastery = RollMastery(source);
            if(mastery) {
                dmgMult *= 2;
                dmgTime *= 2;
            }

            WeaponHitInfo hit;
            var shot = sourceWeapon.Fire(out hit, skill.data.Id, dmgMult);
            if(hit.hitAllowed) {
                Buff buff = new Buff(skill.data.Id.ToString(), null, Common.BonusType.decrease_damage_on_pc, dmgTime, dmgPc);
                targetBonuses.SetBuff(buff);
                source.MmoMessage().SendShot(Common.EventReceiver.OwnerAndSubscriber, shot);
                return true;
            } else {
                source.MmoMessage().SendShot(Common.EventReceiver.ItemOwner, shot);
                return false;
            }
        }
    }
}
