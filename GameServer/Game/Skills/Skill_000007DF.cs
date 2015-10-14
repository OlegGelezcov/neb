﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nebula.Engine;
using Nebula.Game.Components;
using Common;
using Space.Game;

namespace Nebula.Game.Skills {
    public class Skill_000007DF : SkillExecutor {
        public override bool TryCast(NebulaObject source, PlayerSkill skill, out Hashtable info) {
            info = new Hashtable();
            if(!source) {
                return false;
            }

            if(!CheckForShotEnemy(source, skill)) {
                return false;
            }

            float dmgMult = skill.data.Inputs.Value<float>("dmg_mult");
            float enPc = skill.data.Inputs.Value<float>("en_pc");

            var sourceWeapon = source.GetComponent<BaseWeapon>();

            WeaponHitInfo hit;
            var shotInfo = sourceWeapon.Fire(out hit, skill.data.Id, dmgMult);
            if(hit.hitAllowed ) {
                var energy = source.GetComponent<PlayerTarget>().targetObject.GetComponent<ShipEnergyBlock>();
                if(energy ) {
                    float removedEnergy = energy.maximumEnergy * enPc;
                    energy.RemoveEnergy(removedEnergy);
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
