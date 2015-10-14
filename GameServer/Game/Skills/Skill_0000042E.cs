// Skill_0000042E.cs
// Nebula
//
// Created by Oleg Zheleztsov on Monday, September 21, 2015 2:54:19 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using Nebula.Engine;
using Nebula.Game.Bonuses;
using Nebula.Game.Components;
using Space.Game;
using System;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_0000042E : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            if(ShotToEnemyRestricted(source, skill)) {
                return false;
            }

            float dmgMult = skill.GetFloatInput("dmg_mult");
            float dmgAreaMult = skill.GetFloatInput("dmg_area_mult");
            float dmgPc = skill.GetFloatInput("dmg_pc");
            float dmgTime = skill.GetFloatInput("dmg_time");
            int stackCount = skill.GetIntInput("stack_count");
            float radius = skill.GetFloatInput("radius");

            var sourceWeapon = source.Weapon();
            var sourceBonuses = source.Bonuses();
            var sourceMessage = source.MmoMessage();
            var sourceCharacter = source.Character();
            var sourceRace = source.Raceable();


            WeaponHitInfo hit;
            var shot = sourceWeapon.Fire(out hit, skill.data.Id, dmgMult);
            if(hit.hitAllowed ) {
                sourceMessage.SendShot(Common.EventReceiver.OwnerAndSubscriber, shot);
                foreach(var pTarget in GetTargets(source, source.Target().targetObject, radius)) {
                    pTarget.Value.Damagable().ReceiveDamage(source.Type, source.Id, sourceWeapon.GenerateDamage() * dmgAreaMult, sourceCharacter.workshop, sourceCharacter.level, sourceRace.race);
                    if(sourceBonuses.GetBuffCountWithTag( Common.BonusType.increase_damage_on_pc, skill.data.Id) < stackCount ) {
                        Buff buff = new Buff(Guid.NewGuid().ToString(), null, Common.BonusType.increase_damage_on_pc, dmgTime, dmgPc);
                        buff.SetTag(skill.data.Id);
                        sourceBonuses.SetBuff(buff);
                    }
                }
                return true;
            } else {
                sourceMessage.SendShot(Common.EventReceiver.ItemOwner, shot);
                return false;
            }
        }
    }
}
