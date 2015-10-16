﻿using Common;
// Skill_000003E8.cs
// Nebula
//
// Created by Oleg Zheleztsov on Thursday, July 30, 2015 1:35:21 AM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using Nebula.Engine;
using Nebula.Game.Components;
using Space.Game;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_000003E8  : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info ) {
            info = new Hashtable();
            if (!CheckForShotEnemy(source, skill)) {
                return false;
            }

            float dmgMult = skill.data.Inputs.Value<float>("dmg_mult");
            string id = source.Id + skill.data.Id;



            WeaponHitInfo hit;
            var sourceWeapon = source.Weapon();
            var shotInfo = sourceWeapon.Fire(out hit, skill.data.Id, dmgMult);
            
            if (hit.hitAllowed) {
                source.MmoMessage().SendShot(EventReceiver.OwnerAndSubscriber, shotInfo);
                return true;
            } else {
                source.MmoMessage().SendShot(EventReceiver.ItemOwner, shotInfo);
                return false;
            }
            
        }
    }
}