using Common;
using ExitGames.Client.Photon;
using System.Collections.Generic;

namespace Nebula.Client.Pets {
    public class PetCollection : IInfoParser {

        private readonly Dictionary<string, PetInfo> m_Pets = new Dictionary<string, PetInfo>();

        public int MaxCount { get; private set; } = 10;

        public void ParseInfo(Hashtable hash) {
            m_Pets.Clear();

            foreach(System.Collections.DictionaryEntry entry in hash) {
                string id = entry.Key.ToString();
                if (id == "max_count") {
                    int val = 0;
                    if(int.TryParse(entry.Value.ToString(), out val)) {
                        MaxCount = val;
                    }
                } else {
                    Hashtable petHash = entry.Value as Hashtable;
                    if (petHash != null) {
                        m_Pets.Add(id, new PetInfo(petHash));
                    }
                }
            }
        }

        public int Count {
            get {
                return m_Pets.Count;
            }
        }


        public Dictionary<string, PetInfo> pets {
            get {
                return m_Pets;
            }
        }

        public bool HasPet(string id) {
            return m_Pets.ContainsKey(id);
        }

        public PetInfo GetPet(string id) {
            if(HasPet(id)) {
                return m_Pets[id];
            }
            return null;
        }

        public void Clear() {
            m_Pets.Clear();
        }

        public List<PetInfo> GetAssignedPets() {
            List<PetInfo> assignedPets = new List<PetInfo>();
            foreach(var kvp in pets ) {
                if(kvp.Value.active) {
                    assignedPets.Add(kvp.Value);
                }
            }
            assignedPets.Sort((first, second) => {
                return first.id.CompareTo(second.id);
            });
            return assignedPets;
        }

        public List<PetInfo> GetNotAssignedPets() {
            List<PetInfo> notAssignedPets = new List<PetInfo>();
            foreach(var kvp in pets) {
                if(false == kvp.Value.active) {
                    notAssignedPets.Add(kvp.Value);
                }
            }
            notAssignedPets.Sort((first, second) => {
                return first.id.CompareTo(second.id);
            });
            return notAssignedPets;
        }
    }
}
