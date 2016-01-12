using Nebula.Engine;
using Nebula.Game.Components;
using Nebula.Pets;

namespace Nebula.Game.Pets.PassiveBonuses {
    public abstract class PassivePetBonus {

        private PetPassiveBonusInfo m_Data;
        private NebulaObject m_Source;
        private PetObject m_Pet;

        public PassivePetBonus(PetPassiveBonusInfo data, NebulaObject source) {
            m_Data = data;
            m_Source = source;
            m_Pet = source.GetComponent<PetObject>();
        }

        protected PetPassiveBonusInfo data {
            get {
                return m_Data;
            }
        }

        protected NebulaObject source {
            get {
                return m_Source;
            }
        }

        public abstract bool Check();

        public int id {
            get {
                if(m_Data == null ) {
                    return 0;
                }
                return m_Data.id;
            }
        }

        protected PetObject pet {
            get {
                return m_Pet;
            }
        }

        protected PlayerBonuses ownerBonuses {
            get {
                if(pet && pet.owner) {
                    return pet.owner.Bonuses();
                }
                return null;
            }
        }
    }
}
