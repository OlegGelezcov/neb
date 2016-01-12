using Space.Game;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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

        public bool ActivatePet(string id) {
            var pet = GetPet(id);
            if(pet != null ) {
                if(!pet.active) {
                    pet.SetActive(true);
                    return true;
                }
            }
            return false;
        }

        public bool DeactivatePet(string id) {
            var pet = GetPet(id);
            if(pet != null ) {
                if(pet.active) {
                    pet.SetActive(false);
                    return true;
                }
            }
            return false;
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

        public List<PetInfo> pets {
            get {
                return m_Pets;
            }
        }

        public void AddExp(string id, int exp) {
            foreach(var pet in pets) {
                if(pet.id == id ) {
                    pet.AddExp(exp);
                    break;
                }
            }
        }

        public void AddPet(PetInfo pet) {
            pets.Add(pet);
        }

        public bool HasPet(string id) {
            foreach(var pet in pets ) {
                if(pet.id == id ) {
                    return true;
                }
            }
            return false;
        }

        public Hashtable GetInfo(IRes res) {
            Hashtable hash = new Hashtable();
            foreach(var pet in pets) {
                hash.Add(pet.id, pet.GetInfo(res));
            }
            return hash;
        }
     
    }
}
