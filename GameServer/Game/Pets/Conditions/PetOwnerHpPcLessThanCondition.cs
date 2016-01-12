using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nebula.Engine;

namespace Nebula.Game.Pets.Conditions {
    /// <summary>
    /// true when pet owner hp less than condition value
    /// </summary>
    public class PetOwnerHpPcLessThanCondition : Condition {
        private float m_Prob;

        public PetOwnerHpPcLessThanCondition(float prob, NebulaObject source) : base(source) {
            m_Prob = prob;
        }

        public override bool Check() {
            var pet = source.GetComponent<PetObject>();
            if( (!pet) || (!pet.owner)) {
                return false;
            }
            var ownerDamagable = pet.owner.Damagable();
            if(!ownerDamagable) {
                return false;
            }
            if(ownerDamagable.health < m_Prob * ownerDamagable.maximumHealth) {
                return true;
            }
            return false;
        }

        public override void Renew() {
            
        }
    }
}
