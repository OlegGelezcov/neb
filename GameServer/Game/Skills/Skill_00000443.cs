// Skill_00000443.cs
// Nebula
//
// Created by Oleg Zheleztsov on Tuesday, August 4, 2015 10:54:58 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using Common;
using Nebula.Engine;
using Nebula.Game.Bonuses;
using Nebula.Game.Components;
using Space.Game;
using System.Collections;

namespace Nebula.Game.Skills {

    /// <summary>
    /// Shot deals 100% damage and increases the critical damage by 100% (in this case: CriticalDamageShip + 1) 20 seconds
    /// </summary>
    public class Skill_00000443 : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();

            if(!source) { return false; }
            float dmgMult = skill.data.Inputs.Value<float>("dmg_mult");
            float critDmgPc = skill.data.Inputs.Value<float>("crit_dmg_pc");
            float critDmgPcTime = skill.data.Inputs.Value<float>("crit_dmg_pc_time");

            var sourceBonuses = source.GetComponent<PlayerBonuses>();
            var sourceWeapon = source.GetComponent<BaseWeapon>();

            if(!CheckForShotEnemy(source, skill)) {
                return false;
            }

            bool mastery = RollMastery(source);
            if(mastery) {
                dmgMult *= 2;
                critDmgPcTime *= 2;
            }

            WeaponHitInfo hit;
            var shotInfo = sourceWeapon.Fire(out hit, skill.data.Id, dmgMult);
            if(hit.hitAllowed) {
                Buff buff = new Buff(skill.data.Id.ToString(), null, BonusType.increase_crit_damage_on_pc, critDmgPcTime, critDmgPc);
                sourceBonuses.SetBuff(buff);
                source.GetComponent<MmoMessageComponent>().SendShot(EventReceiver.OwnerAndSubscriber, shotInfo);
                return true;
            } else {
                source.GetComponent<MmoMessageComponent>().SendShot(EventReceiver.OwnerAndSubscriber, shotInfo);
                return false;
            }
        }
    }
}
