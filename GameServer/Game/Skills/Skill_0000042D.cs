// Skill_0000042D.cs
// Nebula
//
// Created by Oleg Zheleztsov on Monday, September 21, 2015 2:38:32 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using Nebula.Engine;
using Nebula.Game.Components;
using Space.Game;
using Space.Server;
using System.Collections;
using System.Collections.Concurrent;

namespace Nebula.Game.Skills {
    public class Skill_0000042D : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            if(ShotToEnemyRestricted(source, skill)) {
                return false;
            }

            float dmgMult = skill.GetFloatInput("dmg_mult");
            float radius = skill.GetFloatInput("radius");

            bool mastery = RollMastery(source);
            if(mastery) {
                dmgMult *= 2;
                radius *= 2;
            }

            BaseWeapon sourceWeapon = source.Weapon();
            MmoMessageComponent message = source.MmoMessage();
            NebulaObject targetObject = source.Target().targetObject;
            WeaponHitInfo hit;

            Hashtable shot = sourceWeapon.Fire(out hit, skill.data.Id, dmgMult);
            if(hit.hitAllowed) {
                message.SendShot(Common.EventReceiver.OwnerAndSubscriber, shot);
                ConcurrentDictionary<string, Item> targets = GetTargets(source, targetObject, radius);
                int counter = 0;
                foreach(var pTarget in targets) {
                    var shot2 = sourceWeapon.Fire(out hit, skill.data.Id, dmgMult);
                    message.SendShot(Common.EventReceiver.OwnerAndSubscriber, shot2);
                    counter++;
                    if(counter == 2) {
                        break;
                    }
                }
                return true;
            } else {
                message.SendShot(Common.EventReceiver.ItemOwner, shot);
                return false;
            }
        }
    }
}
