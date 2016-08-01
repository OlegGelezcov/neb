using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Client.Res {
    public class ResPlanetOreMap {
        public Dictionary<string, PlanetOreEntry> map { get; private set; }
        public Dictionary<string, string> planetZoneLinks { get; private set; }

        public void Load(string xml ) {
            map = new Dictionary<string, PlanetOreEntry>();
            UniXmlDocument document = new UniXmlDocument(xml);
            var dump = document.document.Element("map").Elements("planet").Select(e => {
                PlanetOreEntry entry = new PlanetOreEntry(new UniXMLElement(e));
                map.Add(entry.id, entry);
                return entry;
            }).ToList();

            planetZoneLinks = new Dictionary<string, string>();
            var dump2 = document.document.Element("map").Element("planet_zone_links").Elements("link").Select(e => {
                string planet = e.Attribute("planet").Value;
                string zone = e.Attribute("zone").Value;
                if (!planetZoneLinks.ContainsKey(planet)) {
                    planetZoneLinks.Add(planet, zone);
                }
                return e;
            }).ToList();
            
        }

        public PlanetOreEntry GetEntry(string id ) {
            if(map.ContainsKey(id)) {
                return map[id];
            }
            return null;
        }
    }
    public class PlanetOreEntry {
        public string id { get; private set; }
        public List<string> ores { get; private set; }

        public PlanetOreEntry(UniXMLElement element) {
            id = element.GetString("id");
            string oresStr = element.GetString("ores");
            ores = new List<string>();
            foreach(string sOre in oresStr.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries)) {
                ores.Add(sOre.Trim());
            }
        }
    }
}
