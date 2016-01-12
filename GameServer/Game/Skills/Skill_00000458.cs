// Skill_00000458.cs
// Nebula
//
// Created by Oleg Zheleztsov on Saturday, September 26, 2015 4:21:08 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using Nebula.Engine;
using Nebula.Game.Components;
using Space.Game;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_00000458 : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            if(ShotToEnemyRestricted(source, skill)) {
                return false;
            }

            var sourceWeapon = source.Weapon();
            var targetObject = source.Target().targetObject;
            var targetDamagable = targetObject.Damagable();

            float damageMult = skill.GetFloatInput("dmg_mult");
            float hpPc = skill.GetFloatInput("hp_pc");
            float hpRestoredPc = skill.GetFloatInput("hp_restored_pc");

            bool targetHpLow = true;
            if(targetDamagable.health > targetDamagable.maximumHealth * hpPc) {
                targetHpLow = false;
            }

            if(!targetHpLow) {
                damageMult = 1;
            }

            bool mastery = RollMastery(source);
            if(mastery) {
                damageMult *= 2;
            }
            WeaponHitInfo hit;
            var shot = sourceWeapon.Fire(out hit, skill.data.Id, damageMult);
            if(hit.hitAllowed) {
                if(targetDamagable.killed) {
                    float resotedHp = source.Damagable().maximumHealth * hpRestoredPc;
                    if(mastery) {
                        resotedHp *= 2;
                    }

                    source.Damagable().RestoreHealth(source, resotedHp);
                }
                source.MmoMessage().SendShot(Common.EventReceiver.OwnerAndSubscriber, shot);
                return true;
            } else {
                source.MmoMessage().SendShot(Common.EventReceiver.ItemOwner, shot);
                return false;
            }
        }
    }
}
