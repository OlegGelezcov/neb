// Skill_0000042C.cs
// Nebula
//
// Created by Oleg Zheleztsov on Monday, September 21, 2015 2:29:10 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using GameMath;
using Nebula.Engine;
using Nebula.Game.Components;
using Space.Game;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_0000042C : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            if(ShotToEnemyRestricted(source, skill)) {
                return false;
            }
            float dmgMult = skill.GetFloatInput("dmg_mult");
            float blockProb = skill.GetFloatInput("block_prob");

            bool mastery = RollMastery(source);
            if(mastery) {
                dmgMult *= 2;
                blockProb *= 2;
            }

            WeaponHitInfo hit;
            var shot = source.Weapon().Fire(out hit, skill.data.Id, dmgMult);
            if(hit.hitAllowed) {
                var targetWeapon = source.Target().targetObject.Weapon();
                if(targetWeapon) {
                    if(Rand.Float01() < blockProb) {
                        targetWeapon.BlockSingleShot();
                    }
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
