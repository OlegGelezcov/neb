using Common;
using Space.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Nebula.Contracts {
    public class LocationDataCollection {
        private List<LocationData> m_Locations;

        public LocationDataCollection(XElement parent) {
            m_Locations = new List<LocationData>();
            var dump = parent.Elements("location").Select(le => {
                LocationData location = new LocationData(le);
                m_Locations.Add(location);
                return location;
            }).ToList();
        }

        public LocationDataCollection() {
            m_Locations = new List<LocationData>();
        }

        private List<LocationData> Available(Race race, int level) {
            List<LocationData> filtered = new List<LocationData>();
            foreach(var location in m_Locations) {
                if(location.IsValidRace(race) && location.IsValidLevel(level)) {
                    filtered.Add(location);
                }
            }
            return filtered;
        }

        public bool Has(Race race, int level) {
            return Available(race, level).Count > 0;
        }

        public LocationData GetRandom(Race race, int level) {
            var filtered = Available(race, level);
            if(filtered.Count > 0 ) {
                return filtered.AnyElement();
            }
            return null;
        }

        public List<LocationData> Get(Race race) {
            return Available(race, int.MaxValue);
        }

        public int GetCount(Race race) {
            return Get(race).Count;
        }
        public int GetCount(Race race, int level) {
            return Available(race, level).Count;
        }
    }
}
