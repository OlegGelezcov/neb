using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nebula.Engine;
using Nebula.Game.Components;
using Common;
using Space.Game;

namespace Nebula.Game.Skills {
    public class Skill_000007DA : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            if(!source ) {
                return false;
            }

            if(!CheckForShotEnemy(source, skill)) {
                return false;
            }

            var sourceWeapon = source.GetComponent<ShipWeapon>();
            var sourceCharacter = source.GetComponent<CharacterObject>();
            var race = source.GetComponent<RaceableObject>();

            float dmgMult = skill.data.Inputs.Value<float>("dmg_mult");
            float secondDmgMult = skill.data.Inputs.Value<float>("second_dmg_mult");
            float radius = skill.data.Inputs.Value<float>("radius");
            float time = skill.data.Inputs.Value<float>("time");

            var targetObject = source.GetComponent<PlayerTarget>().targetObject;

            bool mastery = RollMastery(source);
            if(mastery) {
                dmgMult *= 2;
                secondDmgMult *= 2;
                time *= 2;
            }

            WeaponHitInfo hit;
            var shotInfo = sourceWeapon.Fire(out hit, skill.data.Id, dmgMult);

            if(hit.hitAllowed) {
                source.GetComponent<MmoMessageComponent>().SendShot(EventReceiver.OwnerAndSubscriber, shotInfo);

                var items = (source.world as MmoWorld).GetItems((it) => {
                    if (it.GetComponent<DamagableObject>() && it.GetComponent<CharacterObject>()) {
                        if (it.transform.DistanceTo(targetObject.transform) < radius) {
                            if (sourceCharacter.RelationTo(it.GetComponent<CharacterObject>()) == FractionRelation.Enemy) {
                                return true;
                            }
                        }
                    }
                    return false;
                });

                float curTime = Time.curtime();

                float damage = sourceWeapon.GetDamage(false) * dmgMult;

                InputDamage inpDamage = new InputDamage(source, damage);
                foreach(var pair in items) {
                    if(pair.Value.Id != targetObject.Id ) {
                        pair.Value.GetComponent<DamagableObject>().ReceiveDamage(inpDamage);
                    }

                    sourceWeapon.AddAdditionalDamager(pair.Value.Id, new ShipWeapon.AdditionalDamage { damageMult = secondDmgMult, expireTime = curTime + time });
                }
                return true;
            } else {
                source.GetComponent<MmoMessageComponent>().SendShot(EventReceiver.OwnerAndSubscriber, shotInfo);
                return false;
            }

        }
    }
}
