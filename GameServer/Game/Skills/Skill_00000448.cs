// Skill_00000448.cs
// Nebula
//
// Created by Oleg Zheleztsov on Wednesday, August 5, 2015 2:24:11 PM
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
    /// Выстрел наносит 140% урона и повышает шанс критического удара на 2% (т.е. СriticalChanceShip + 0,02) на 6 секунд за каждое применение этого умения подряд. 
    /// Эффект суммируется не более 5 раз (т.е. максимум + 10%).
    /// </summary>
    public class Skill_00000448 : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();

            if(!source ) {
                return false;
            }
            if(!CheckForShotEnemy(source, skill)) {
                return false;
            }

            float dmgMult = skill.data.Inputs.Value<float>("dmg_mult");
            float critChancePc = skill.data.Inputs.Value<float>("crit_chance_pc");
            float critChanceTime = skill.data.Inputs.Value<float>("crit_chance_time");
            int skillCount = skill.data.Inputs.Value<int>("skill_counter");

            var sourceWeapon = source.GetComponent<BaseWeapon>();
            var skills = source.GetComponent<PlayerSkills>();

            bool mastery = RollMastery(source);
            if(mastery) {
                dmgMult *= 2;
                critChanceTime *= 2;
            }
            WeaponHitInfo hit;
            var shotInfo = sourceWeapon.Fire(out hit, skill.data.Id, dmgMult);
            if(hit.hitAllowed ) {
                if( (skills.lastSkill != skill.data.Id ) || (skills.lastSkill == skill.data.Id && skills.sequenceSkillCounter < skillCount )) {
                    Buff buff = new Buff(skill.data.Id.ToString() + skills.sequenceSkillCounter, null, BonusType.increase_crit_chance_on_cnt, critChanceTime, critChancePc);
                    source.GetComponent<PlayerBonuses>().SetBuff(buff);
                }
                source.GetComponent<MmoMessageComponent>().SendShot(EventReceiver.OwnerAndSubscriber, shotInfo);
                return true;
            } else {
                source.GetComponent<MmoMessageComponent>().SendShot(EventReceiver.OwnerAndSubscriber, shotInfo);
                return false;
            }
        }
    }
}
