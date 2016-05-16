using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Client.Res {
    public class StartPlayerWorkshopModel {

        public Workshop workshop { get; private set; }
        public Dictionary<ShipModelSlotType, string> model { get; private set; }

        public StartPlayerWorkshopModel(UniXMLElement parent) {
            workshop = (Workshop)System.Enum.Parse(typeof(Workshop), parent.GetString("id"));
            model = new Dictionary<ShipModelSlotType, string>();
            var dump = parent.Elements("module").Select(e => {
                ShipModelSlotType type = (ShipModelSlotType)Enum.Parse(typeof(ShipModelSlotType), e.GetString("type"));
                string id = e.GetString("id");
                model.Add(type, id);
                return type;
            }).ToList();
        }
    }
}
