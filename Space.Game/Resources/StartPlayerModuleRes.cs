using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using System.Xml.Linq;
using System.IO;

namespace Space.Game.Resources {
    public class StartPlayerModuleRes : IResourceLoader {

        private bool loaded;
        private Dictionary<Race, Dictionary<Workshop, Dictionary<ShipModelSlotType, string>>> modules;

        [Obsolete]
        public bool Load(string basePath) {
            string fullPath = Path.Combine(basePath, "Data/start_player_modules.xml");
            return LoadFromFile(fullPath);
        }

        public bool LoadFromFile(string fullPath) {
            XDocument document = XDocument.Load(fullPath);

            this.modules = document.Element("races").Elements("race").Select(re => {
                Race race = (Race)Enum.Parse(typeof(Race), re.GetString("id"));
                Dictionary<Workshop, Dictionary<ShipModelSlotType, string>> workshopsDict = re.Element("workshops").Elements("workshop").Select(we => {
                    Workshop workshop = (Workshop)Enum.Parse(typeof(Workshop), we.GetString("id"));
                    Dictionary<ShipModelSlotType, string> modulesDict = we.Elements("module").Select(me => {
                        ShipModelSlotType t = (ShipModelSlotType)Enum.Parse(typeof(ShipModelSlotType), me.GetString("type"));
                        string id = me.GetString("id");
                        return new { TYPE = t, ID = id };
                    }).ToDictionary(o => o.TYPE, o => o.ID);
                    return new { KEY = workshop, VALUE = modulesDict };
                }).ToDictionary(o2 => o2.KEY, o2 => o2.VALUE);
                return new { RESKEY = race, RESVALUE = workshopsDict };
            }).ToDictionary(o3 => o3.RESKEY, o3 => o3.RESVALUE);
            this.loaded = true;
            return this.loaded;
        }

        public bool Loaded {
            get { return this.loaded; }
        }

        public Dictionary<ShipModelSlotType, string> StartModuleFor(Race race, Workshop workshop) {
            Dictionary<Workshop, Dictionary<ShipModelSlotType, string>> workshopsDict = null;
            if (!this.modules.TryGetValue(race, out workshopsDict)) {
                return null;
            }
            Dictionary<ShipModelSlotType, string> resultDict = null;
            if (!workshopsDict.TryGetValue(workshop, out resultDict)) {
                return null;
            }
            return resultDict;
        }
    }
}
