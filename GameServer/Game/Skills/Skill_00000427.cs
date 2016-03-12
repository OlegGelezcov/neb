// Skill_00000427.cs
// Nebula
//
// Created by Oleg Zheleztsov on Monday, September 21, 2015 12:53:25 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using Nebula.Engine;
using Nebula.Game.Components;
using Space.Game;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_00000427 : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            if(ShotToEnemyRestricted(source, skill)) {
                return false;
            }

            float dmgMult = skill.GetFloatInput("dmg_mult");
            float areaDmgMult = skill.GetFloatInput("dmg_area_mult");
            float radius = skill.GetFloatInput("radius");

            var weapon = source.Weapon();
            var character = source.Character();
            var raceable = source.Raceable();

            bool mastery = RollMastery(source);
            if(mastery) {
                dmgMult *= 2;
            }
            WeaponHitInfo hit;
            var shot = weapon.Fire(out hit, skill.data.Id, dmgMult);
            if(hit.hitAllowed) {
                source.MmoMessage().SendShot(Common.EventReceiver.OwnerAndSubscriber, shot);

                var targets = GetTargets(source, source.Target().targetObject, radius);

                var genWeapDmg = weapon.GenerateDamage();
                genWeapDmg.Mult(areaDmgMult);
                InputDamage inpDamage = new InputDamage(source, genWeapDmg);
                if(mastery) {
                    //inpDamage.SetDamage(inpDamage.damage * 2);
                    inpDamage.damage.Mult(2);
                }
                foreach(var pTarget in targets ) {
                    pTarget.Value.Damagable().ReceiveDamage(inpDamage);
                }
                return true;
            } else {
                source.MmoMessage().SendShot(Common.EventReceiver.ItemOwner, shot);
                return false;
            }
        }
    }
}
