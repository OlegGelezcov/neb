using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nebula.Engine;
using Nebula.Game.Components;
using Common;
using Space.Game;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_000003EF : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();

            string id = skill.data.Id.ToString();
            if (!CheckForShotEnemy(source, skill)) {
                return false;
            }

            float dmgMult = skill.data.Inputs.Value<float>("dmg_mult");
            float dmgMultArea = skill.data.Inputs.Value<float>("dmg_area_mult");
            float radius = skill.data.Inputs.Value<float>("radius");

            var sourceWeapon = source.GetComponent<BaseWeapon>();
            var targetComp = source.GetComponent<PlayerTarget>();
            var sourceChar = source.GetComponent<CharacterObject>();

            var targetObj = targetComp.targetObject;

            WeaponHitInfo hit;
            var shotInfo = sourceWeapon.GetComponent<BaseWeapon>().Fire(out hit, skill.data.Id, dmgMult);
            if (hit.hitAllowed) {

                source.GetComponent<MmoMessageComponent>().SendShot(EventReceiver.OwnerAndSubscriber, shotInfo);
                var items = (source.world as MmoWorld).GetItems((it) => {
                    if (it.GetComponent<DamagableObject>() && it.GetComponent<CharacterObject>()) {
                        if (it.Id != targetObj.Id) {
                            if (it.transform.DistanceTo(targetObj.transform) <= radius) {
                                if (sourceChar.RelationTo(it.GetComponent<CharacterObject>()) == FractionRelation.Enemy) {
                                    return true;
                                }
                            }
                        }
                    }
                    return false;
                });

                float damage = sourceWeapon.GetDamage(false) * dmgMultArea;

                foreach(var p in items) {
                    p.Value.GetComponent<DamagableObject>().ReceiveDamage(source.Type, source.Id, damage, sourceChar.workshop, sourceChar.level, source.GetComponent<RaceableObject>().race);
                }

                return true;
            } else {
                source.GetComponent<MmoMessageComponent>().SendShot(EventReceiver.OwnerAndSubscriber, shotInfo);
                return false;
            }
        }
    }
}
