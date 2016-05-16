#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;

namespace Nebula.Client.Res {
    public class ResPets {

        private Dictionary<string, PetData> m_Pets;
        private List<PetPlayerCount> m_PetPlayerCountList;


        public void Load(string xml) {
            m_Pets = new Dictionary<string, PetData>();
            m_PetPlayerCountList = new List<PetPlayerCount>();
#if UP
            UPXDocument document = new UPXDocument(xml);
#else
            XDocument document = XDocument.Parse(xml);
#endif       

            var dump = document.Element("pets").Element("types").Elements("type").Select(typeElement => {
                PetData petData = new PetData(typeElement);
                m_Pets.Add(petData.id, petData);
                return petData;
            }).ToList();

            m_PetPlayerCountList = document.Element("pets").Element("player").Elements("level").Select(levelElement => {
                return new PetPlayerCount(levelElement);
            }).ToList();
        }

        public PetData GetData(string id) {
            if(m_Pets.ContainsKey(id)) {
                return m_Pets[id];
            }
            return null;
        }

        public PetPlayerCount GetPetPlayerCount(int level) {
            foreach(var pCount in m_PetPlayerCountList) {
                if(pCount.minLevel <= level && level <= pCount.maxLevel) {
                    return pCount;
                }
            }
            return null;
        }

        public int petCount {
            get {
                return m_Pets.Count;
            }
        }

        public int minPlayerLevelForUnlockPet {
            get {
                PetPlayerCount min = null;
                foreach(var pc in m_PetPlayerCountList) {
                    if( pc.count > 0 ) {
                        if(min == null ) {
                            min = pc;
                        } else {
                            if(pc.count < min.count) {
                                min = pc;
                            }
                        }
                        
                    } 
                }
                return min.minLevel;
            }
        }

        public List<PetData> GetPets(Race race) {
            List<PetData> list = new List<PetData>();
            foreach(var pp in m_Pets ) {
                if(pp.Value.race == race ) {
                    list.Add(pp.Value);
                }
            }
            return list;
        }
    }
}
