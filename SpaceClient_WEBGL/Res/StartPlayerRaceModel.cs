using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Client.Res {
    public class StartPlayerRaceModel {
        public Race race { get; private set; }
        public Dictionary<Workshop, StartPlayerWorkshopModel> models;

        public StartPlayerRaceModel(UniXMLElement parent) {
            race = (Race)Enum.Parse(typeof(Race), parent.GetString("id"));
            models = new Dictionary<Workshop, StartPlayerWorkshopModel>();
            var dump = parent.Element("workshops").Elements("workshop").Select(we => {
                StartPlayerWorkshopModel wModel = new StartPlayerWorkshopModel(we);
                models.Add(wModel.workshop, wModel);
                return wModel;
            }).ToList();
        }

        public StartPlayerWorkshopModel GetWorkshopModel(Workshop w) {
            if(models.ContainsKey(w)) {
                return models[w];
            }
            return null;
        }
    }
}
