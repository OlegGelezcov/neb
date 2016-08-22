using Common;
using Nebula.Drop;
using Nebula.Engine;
using Nebula.Game.Components;
using Space.Game;
using System.Collections;

namespace Nebula.Game.Skills {
    public class Skill_00000409 : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();

            float dmgMult = skill.GetFloatInput("dmg_mult");
            float dmgTimedMult = skill.GetFloatInput("dmg_timed_mult");
            float time = skill.GetFloatInput("time");
            var sourceCharacter = source.Character();
            var sourceRaceable = source.Raceable();

            info.SetSkillUseState(SkillUseState.normal);
            if (ShotToEnemyRestricted(source, skill)) {
                info.SetSkillUseState(SkillUseState.invalidTarget);
                return false;
            }
            if (NotCheckDistance(source)) {
                info.SetSkillUseState(SkillUseState.tooFar);
                return false;
            }

            WeaponHitInfo hit;

            var sourceWeapon = source.Weapon();
            float baseDamage = sourceWeapon.GetDamage().totalDamage;
            float timedDamage = baseDamage * dmgTimedMult;

            //float secondDamage = baseDamage * dmgAreaMult;

            bool mastery = RollMastery(source);
            if(mastery) {
                dmgMult *= 2;
                timedDamage *= 2;
                info.SetMastery(true);
            } else {
                info.SetMastery(false);
            }

            var shot = source.Weapon().Fire(out hit, skill.data.Id, dmgMult);
            if(hit.normalOrMissed) {
                source.MmoMessage().SendShot(Common.EventReceiver.OwnerAndSubscriber, shot);

                source.Target().targetObject.Damagable().SetTimedDamage(time, timedDamage / time, sourceWeapon.myWeaponBaseType, skill.idInt);
                /*
                WeaponDamage sInpWeapDmg = new WeaponDamage(sourceWeapon.myWeaponBaseType);
                sInpWeapDmg.SetBaseTypeDamage(secondDamage);
                InputDamage inpDamage = new InputDamage(source, sInpWeapDmg);
                if(mastery) {
                    inpDamage.Mult(2);
                    //inpDamage.SetDamage(inpDamage.damage * 2);
                }
                foreach(var pitem in GetTargets(source, source.Target().targetObject, radius)) {
                    pitem.Value.Damagable().ReceiveDamage(inpDamage);
                }*/
                return true;
            }

            return false;
        }
    }
}
