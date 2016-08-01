using Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Nebula.Resources {
    public class Planet2OreMapRes {

        private ConcurrentDictionary<string, PlanetOreEntry> map { get; set; }

        public void Load(string file ) {
            XDocument document = XDocument.Load(file);
            map = new ConcurrentDictionary<string, PlanetOreEntry>();
            var dump = document.Element("map").Elements("planet").Select(e => {
                PlanetOreEntry entry = new PlanetOreEntry(e);
                map.TryAdd(entry.id, entry);
                return entry;
            }).ToList();
        }

        public PlanetOreEntry GetEntry(string id ) {
            PlanetOreEntry entry;
            if(map.TryGetValue(id, out entry)) {
                return entry;
            }
            return null;
        }
    }

    public class PlanetOreEntry {
        public string id { get; private set; }
        public List<string> ores { get; private set; }

        public PlanetOreEntry(XElement element ) {
            id = element.GetString("id");
            ores = element.GetString("ores").Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries).ToList();
        }
    }
}
