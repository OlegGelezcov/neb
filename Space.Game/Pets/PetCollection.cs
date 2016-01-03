using System.Collections.Generic;

namespace Nebula.Pets {
    public class PetCollection {
        private readonly List<PetInfo> m_Pets;

        public void Add(PetInfo pet) {
            m_Pets.Add(pet);
        }

        public PetInfo GetPet(string id) {
            foreach(var pet in m_Pets) {
                if(pet.id == id ) {
                    return pet;
                }
            }
            return null;
        }

        public PetCollection() {
            m_Pets = new List<PetInfo>();
        }

        public PetCollection(List<PetSave> pets) {
            m_Pets = new List<PetInfo>();
            if(pets != null ) {
                foreach(var save in pets) {
                    m_Pets.Add(new PetInfo(save));
                }
            }
        }

        public List<PetSave> GetSave() {
            List<PetSave> saves = new List<PetSave>();
            if(m_Pets != null ) {
                foreach(var pet in m_Pets ) {
                    saves.Add(pet.GetSave());
                }
            }
            return saves;
        }
     
    }
}
