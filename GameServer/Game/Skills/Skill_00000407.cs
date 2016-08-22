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
            info.SetSkillUseState(SkillUseState.normal);

            if (ShotToEnemyRestricted(source, skill)) {
                info.SetSkillUseState(SkillUseState.invalidTarget);
                return false;
            }

            if(NotCheckDistance(source)) {
                info.SetSkillUseState(SkillUseState.tooFar);
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
                info.SetMastery(true);
            } else {
                info.SetMastery(false);
            }

            WeaponHitInfo hit;
            var shotInfo = sourceWeapon.GetComponent<BaseWeapon>().Fire(out hit, skill.data.Id, dmgMult);

            if (hit.normalOrMissed) {

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

                var sourceMmoMessage = source.MmoMessage();
                foreach(var p in items) {
                    /*
                    var d = p.Value.GetComponent<DamagableObject>();
                    d.RestoreHealth(source, hp);*/
                    //d.SetHealth(d.health + hp);
                    var heal = sourceWeapon.Heal(p.Value, hp, skill.idInt);
                    sourceMmoMessage.SendHeal(EventReceiver.OwnerAndSubscriber, heal);

                    info.Add(p.Value.Id, p.Value.Type);
                }

                return true;
            }
            return false;

        }
    }
}
