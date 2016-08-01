using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nebula.Engine;
using Nebula.Game.Components;
using Common;
using Space.Game;
using System.Collections;
using Nebula.Drop;

namespace Nebula.Game.Skills {
    public class Skill_000003EF : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();

            string id = skill.data.Id.ToString();

            info.SetSkillUseState(SkillUseState.normal);
            if (!CheckForShotEnemy(source, skill)) {
                info.SetSkillUseState(SkillUseState.invalidTarget);
                return false;
            }
            if (NotCheckDistance(source)) {
                info.SetSkillUseState(SkillUseState.tooFar);
                return false;
            }

            float dmgMult = skill.data.Inputs.Value<float>("dmg_mult");
            float dmgMultArea = skill.data.Inputs.Value<float>("dmg_area_mult");
            float radius = skill.data.Inputs.Value<float>("radius");

            var sourceWeapon = source.GetComponent<BaseWeapon>();
            var targetComp = source.GetComponent<PlayerTarget>();
            var sourceChar = source.GetComponent<CharacterObject>();

            var targetObj = targetComp.targetObject;

            bool mastery = RollMastery(source);
            if(mastery) {
                dmgMult *= 2;
            }
            WeaponHitInfo hit;
            var shotInfo = sourceWeapon.GetComponent<BaseWeapon>().Fire(out hit, skill.data.Id, dmgMult);
            if (hit.normalOrMissed) {

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

                float damage = sourceWeapon.GetDamage(false).totalDamage * dmgMultArea;

                WeaponDamage sInpWeapDmg = new WeaponDamage(sourceWeapon.myWeaponBaseType);
                sInpWeapDmg.SetBaseTypeDamage(damage);
                InputDamage inpDamage = new InputDamage(source, sInpWeapDmg);
                if(mastery) {
                    inpDamage.Mult(2);
                }
                foreach (var p in items) {
                    
                    p.Value.GetComponent<DamagableObject>().ReceiveDamage(inpDamage);
                }

                return true;
            } else {
                source.GetComponent<MmoMessageComponent>().SendShot(EventReceiver.OwnerAndSubscriber, shotInfo);
                return false;
            }
        }
    }
}
