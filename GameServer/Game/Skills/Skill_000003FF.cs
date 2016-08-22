using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nebula.Engine;
using Nebula.Game.Components;
using Space.Game;
using Common;
using GameMath;
using Nebula.Game.Bonuses;

namespace Nebula.Game.Skills {
    public class Skill_000003FF : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();

            var targetObject = source.Target().targetObject;
            if (!targetObject) {
                return false;
            }

            var weapon = source.Weapon();
            if (Mathf.Approximately(weapon.HitProbTo(targetObject), 0f)) {
                log.InfoFormat("Skill {0} error: hit prob is 0", skill.data.Id.ToString("X8"));
                return false;
            }

            float dmgMult = skill.GetFloatInput("dmg_mult");
            float critChanceForBot = skill.GetFloatInput("crit_chance");
            float maxCrit = skill.GetFloatInput("max_crit");
            float time = skill.GetFloatInput("time");
            float radius = skill.GetFloatInput("radius");

            var character = source.Character();


            var targets = source.mmoWorld().GetItems((item) => {
                var damagable = item.Damagable();
                var itemCharacter = item.Character();
                if (damagable && itemCharacter) {
                    var relation = character.RelationTo(itemCharacter);
                    if (relation == FractionRelation.Enemy || relation == FractionRelation.Neutral) {
                        if (targetObject.transform.DistanceTo(item.transform) <= radius) {
                            return true;
                        }
                    }
                }
                return false;
            });

            bool mastery = RollMastery(source);
            if(mastery) {
                dmgMult *= 2;
                time *= 2;
            }

            foreach (var pTarget in targets) {
                WeaponHitInfo hit;
                var shot = weapon.Fire(pTarget.Value, out hit, skill.data.Id, dmgMult, forceShot: true);
                source.GetComponent<MmoMessageComponent>().SendShot(EventReceiver.OwnerAndSubscriber, shot);
            }

            float critChanceBuffValue = Mathf.Clamp(targets.Count * critChanceForBot, 0, maxCrit);
            if (!Mathf.Approximately(0f, critChanceBuffValue)) {
                Buff buff = new Buff(skill.data.Id.ToString(), null, BonusType.increase_crit_chance_on_cnt, time, critChanceBuffValue);
                source.Bonuses().SetBuff(buff, source);
            }
            return true;
        }
    }
}
