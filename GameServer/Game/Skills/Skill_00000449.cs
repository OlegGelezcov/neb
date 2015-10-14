// Skill_00000449.cs
// Nebula
//
// Created by Oleg Zheleztsov on Wednesday, August 5, 2015 2:33:33 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using Common;
using Nebula.Engine;
using Nebula.Game.Components;
using Space.Game;
using System.Collections;

namespace Nebula.Game.Skills {
    /// <summary>
    /// Shot inflicts 140% damage and within 12 seconds with a 30% chance the ship will reflect incoming damage
    /// </summary>
    public class Skill_00000449 : SkillExecutor  {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            if(!source) {
                return false;
            }
            if(!CheckForShotEnemy(source, skill )) {
                return false;
            }

            float dmgMult = skill.data.Inputs.Value<float>("dmg_mult");
            float reflectDamagePc = skill.data.Inputs.Value<float>("reflect_dmg_pc");
            float reflectDamageTime = skill.data.Inputs.Value<float>("reflect_dmg_time");


            var sourceWeapon = source.GetComponent<BaseWeapon>();
            WeaponHitInfo hit;
            var shotInfo = sourceWeapon.Fire(out hit, skill.data.Id, dmgMult);
            if(hit.hitAllowed) {
                source.GetComponent<DamagableObject>().SetReflectParameters(reflectDamagePc, reflectDamageTime);
                source.GetComponent<MmoMessageComponent>().SendShot(EventReceiver.OwnerAndSubscriber, shotInfo);
                return true;
            } else {
                source.GetComponent<MmoMessageComponent>().SendShot(EventReceiver.OwnerAndSubscriber, shotInfo);
                return false;
            }
        }
    }
}
