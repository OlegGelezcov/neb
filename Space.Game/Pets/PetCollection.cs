using Space.Game;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Nebula.Pets {
    public class PetCollection {
        private readonly List<PetInfo> m_Pets;
        private readonly int m_MaxCount;

        public bool Add(PetInfo pet) {
            return AddPet(pet);
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

        public PetCollection(int maxCount) {
            m_MaxCount = maxCount;
            m_Pets = new List<PetInfo>();
        }

        public PetCollection(List<PetSave> pets, int maxCount) {
            m_MaxCount = maxCount;
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

        public bool AddPet(PetInfo pet) {
            if (pets.Count < m_MaxCount) {
                pets.Add(pet);
                return true;
            }
            return false;
        }

        public bool RemovePet(string id) {
            if(HasPet(id)) {
                var pet = GetPet(id);
                if(pet != null ) {
                    return pets.Remove(pet);
                }
            }
            return false;
        }

        public bool HasPet(string id) {
            foreach(var pet in pets ) {
                if(pet.id == id ) {
                    return true;
                }
            }
            return false;
        }

        public bool ImproveColor(string petId) {
            var pet = GetPet(petId);
            if(pet == null ) {
                return false;
            }
            if(pet.hasMaxColor) {
                return false;
            }
            pet.SetColor(pet.nextColor);
            return true;
        }

        public bool ImproveMastery(string petId ) {
            var pet = GetPet(petId);
            if(pet == null ) {
                return false;
            }
            return pet.ImproveMastery();
        }


        public bool hasFreeSpace {
            get {
                return (pets.Count < m_MaxCount);
            }
        }

        public bool SetModel( string petID, string model) {
            var pet = GetPet(petID);
            if(pet == null ) {
                return false;
            }
            pet.SetType(model);
            return true;
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
