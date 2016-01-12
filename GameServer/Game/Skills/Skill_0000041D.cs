using Nebula.Engine;
using Nebula.Game.Components;
using System.Collections;

namespace Nebula.Game.Skills {

    //restored targe
    public class Skill_0000041D : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            if(!CheckForHealAlly(source)) {
                return false;
            }

            float hpPc = skill.GetFloatInput("hp_pc");

            var targetObject = source.Target().targetObject;
            var targetDamagable = targetObject.Damagable();
            float restoreHP = targetDamagable.maximumHealth * hpPc;

            var weapon = source.Weapon();

            bool mastery = RollMastery(source);
            if(mastery) {
                restoreHP *= 2;
            }

            var heal = weapon.Heal(targetObject, restoreHP, skill.data.Id);
            source.MmoMessage().SendHeal(Common.EventReceiver.OwnerAndSubscriber, heal);

            var targetEnergy = targetObject.GetComponent<ShipEnergyBlock>();
            if(targetEnergy ) {
                float restoredEnergy = hpPc * targetEnergy.maximumEnergy;
                if(mastery) {
                    restoredEnergy *= 2;
                }
                targetEnergy.RestoreEnergy(restoredEnergy);
            }
            return true;
        }
    }
}
