using Common;
using ServerClientCommon;
using System.Collections;
using System.Collections.Generic;

namespace Nebula.Game.Pets {
    public abstract class PetSkill  {

        private readonly List<Condition> m_Conditions = new List<Condition>();

        protected void AddCondition(Condition condition) {
            m_Conditions.Add(condition);
        }

        private bool ConditionsValid(PetObject source) {
            bool valid = true;
            foreach(var condition in m_Conditions) {
                if(!condition.Check(source.nebulaObject)) {
                    valid = false;
                    break;
                }
            }
            return valid;
        }

        private void ConditionsRenew() {
            foreach(var condition in m_Conditions) {
                condition.Renew();
            }
        }
        public bool Use(PetObject source) {
            if(ConditionsValid(source)) {
                bool success =  DoUse(source);
                if(success) {
                    ConditionsRenew();
                    source.SendPetSkill(GetProperties(source));
                }
                return success;
            }
            return false;
        }

        public abstract bool DoUse(PetObject source);
        public abstract int id { get; }

        protected virtual Hashtable GetProperties(PetObject source) {
            return new Hashtable {
                {(int)SPC.Source,  source.nebulaObject.Id },
                {(int)SPC.SourceType, source.nebulaObject.Type },
                {(int)SPC.Skill, id }
            };
        }
    }
}
