using Common;
using Nebula.Engine;
using Nebula.Game.Components;
using Space.Game;
using System;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_00000407 : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            if(!CheckForShotEnemy(source, skill)) {
                return false;
            }

            float hp_pc = skill.data.Inputs.Value<float>("hp_pc");
            float hp_radius = skill.data.Inputs.Value<float>("hp_radius");
            float dmgMult = skill.data.Inputs.Value<float>("dmg_mult");


            var sourceWeapon = source.GetComponent<BaseWeapon>();
            var sourceChar = source.GetComponent<CharacterObject>();
            var sourceDamagable = source.GetComponent<DamagableObject>();

            bool mastery = RollMastery(source);
            if(mastery) {
                dmgMult *= 2;
                hp_pc *= 2;
            }

            WeaponHitInfo hit;
            var shotInfo = sourceWeapon.GetComponent<BaseWeapon>().Fire(out hit, skill.data.Id, dmgMult);

            if (hit.hitAllowed) {

                source.GetComponent<MmoMessageComponent>().SendShot(EventReceiver.OwnerAndSubscriber, shotInfo);

                var items = (source.world as MmoWorld).GetItems((it) => {
                    if (it.GetComponent<DamagableObject>() && it.GetComponent<CharacterObject>()) {
                        if (source.transform.DistanceTo(it.transform) <= hp_radius) {
                            if (sourceChar.RelationTo(it.GetComponent<CharacterObject>()) == FractionRelation.Friend) {
                                return true;
                            }
                        }
                    }
                    return false;
                });

                float hp = hp_pc * sourceDamagable.maximumHealth;

                foreach(var p in items) {
                    var d = p.Value.GetComponent<DamagableObject>();
                    d.RestoreHealth(source, hp);
                    //d.SetHealth(d.health + hp);
                    info.Add(p.Value.Id, p.Value.Type);
                }

                return true;
            } else {
                source.GetComponent<MmoMessageComponent>().SendShot(EventReceiver.OwnerAndSubscriber, shotInfo);
                return false;
            }
        }
    }
}
