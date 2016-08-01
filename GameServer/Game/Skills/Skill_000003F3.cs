using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nebula.Engine;
using Nebula.Game.Components;
using Common;
using Nebula.Game.Bonuses;
using Space.Game;

namespace Nebula.Game.Skills {
    public class Skill_000003F3 : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            info.SetSkillUseState(SkillUseState.normal);
            if (!CheckForShotEnemy(source, skill)) {
                info.SetSkillUseState(SkillUseState.invalidTarget);
                return false;
            }
            if (NotCheckDistance(source)) {
                info.SetSkillUseState(SkillUseState.tooFar);
                return false;
            }
            float dmgMult = skill.data.Inputs.GetValue<float>("dmg_mult", 0f);
            float critChanceCnt = skill.data.Inputs.GetValue<float>("crit_chance_cnt", 0f);
            float critChanceTime = skill.data.Inputs.GetValue<float>("crit_chance_time", 0f);

            var weapon = source.GetComponent<BaseWeapon>();
            var sourceCharacter = source.GetComponent<CharacterObject>();

            WeaponHitInfo hit;
            bool mastery = RollMastery(source);
            if(mastery) {
                dmgMult *= 2;
            }
            var shot = weapon.Fire(out hit, skill.data.Id, dmgMult);
            if (hit.normalOrMissed) {
                source.GetComponent<MmoMessageComponent>().SendShot(EventReceiver.OwnerAndSubscriber, shot);

                var friends = (source.world as MmoWorld).GetItems(ItemType.Avatar, (it) => {
                    var character = it.GetComponent<CharacterObject>();
                    var bonuses = it.GetComponent<PlayerBonuses>();
                    float dist = it.transform.DistanceTo(source.transform);
                    if (character && bonuses && dist <= weapon.optimalDistance) {
                        if(sourceCharacter.RelationTo(character) == FractionRelation.Friend) {
                            return true;
                        }
                    }
                    return false;
                });

                if(mastery) {
                    critChanceTime *= 2;
                }
                foreach(var friend in friends ) {
                    Buff buff = new Buff(skill.data.Id.ToString(), null, BonusType.increase_crit_chance_on_cnt, critChanceTime, critChanceCnt);
                    friend.Value.GetComponent<PlayerBonuses>().SetBuff(buff);
                }

                return true;
            } else {
                source.GetComponent<MmoMessageComponent>().SendShot(EventReceiver.OwnerAndSubscriber, shot);
                return false;
            }
        }
    }
}
