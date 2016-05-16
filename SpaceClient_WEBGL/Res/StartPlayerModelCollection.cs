using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Client.Res {
    public class StartPlayerModelCollection {

        public Dictionary<Race, StartPlayerRaceModel> models { get; private set; }

        public void Load(string xml ) {
            UniXmlDocument document = new UniXmlDocument(xml);
            models = new Dictionary<Race, StartPlayerRaceModel>();
            UniXMLElement root = new UniXMLElement(document.document.Element("races"));
            var dump = root.Elements("race").Select(e => {
                StartPlayerRaceModel raceModel = new StartPlayerRaceModel(e);
                models.Add(raceModel.race, raceModel);
                return raceModel;
            }).ToList(); 
        }

        public StartPlayerRaceModel GetRaceModel(Race race) {
            if(models.ContainsKey(race)) {
                return models[race];
            }
            return null;
        }
    }
}
