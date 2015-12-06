using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;


namespace Nebula.Client.Res {
    public class ResRaces {

        private Dictionary<Race, ClientRace> races = new Dictionary<Race, ClientRace>();

        public void Load(string xml) {
            var document = XDocument.Parse(xml);
            this.races = document.Element("races").Elements("race").Select(re => {
                Race raceId = (Race)Enum.Parse(typeof(Race), re.GetString("id"));
                string raceNameId = re.GetString("name");
                string raceDescriptionId = re.GetString("description");
                var raceWorkshops = re.Element("workshops").Elements("workshop").Select(we => {
                    Workshop workshopId = (Workshop)Enum.Parse(typeof(Workshop), we.GetString("id"));
                    string workshopNameId = we.GetString("name");
                    string workshopDescriptionId = we.GetString("description");
                    return new ClientRaceWorkshop {
                        Id = workshopId,
                        NameStringId = workshopNameId,
                        DescriptionStringId = workshopDescriptionId
                    };
                }).ToList();

                return new ClientRace {
                    Id = raceId,
                    DescriptionStringId = raceDescriptionId,
                    NameStringId = raceNameId,
                    Workshops = raceWorkshops
                };
            }).ToDictionary(cr => cr.Id, cr => cr);
        }

        public bool TryGetRace(Race race, out ClientRace result) {
            return this.races.TryGetValue(race, out result);
        }

        public Dictionary<Race, ClientRace> Races() {
            return this.races;
        }

        /// <summary>
        /// Return Race which contains parameter workshop
        /// </summary>
        public Race RaceForWorkshop(Workshop workshop) {
            foreach (var pRace in this.races) {
                foreach(var w in pRace.Value.Workshops) {
                    if(w.Id == workshop) {
                        return pRace.Key;
                    }
                }
            }
            return Race.None;
        }
    }

    public class ClientRaceWorkshop {
        public Workshop Id;
        public string NameStringId;
        public string DescriptionStringId;
    }

    public class ClientRace {
        public Race Id;
        public string NameStringId;
        public string DescriptionStringId;
        public List<ClientRaceWorkshop> Workshops;

        public bool TryGetWorkshop(Workshop workshop, out ClientRaceWorkshop result) {

            result = null;
            foreach(var w in Workshops) {
                if(w.Id == workshop) {
                    result = w;
                    return true;
                }
            }
            return false;
        } 
    }
}
