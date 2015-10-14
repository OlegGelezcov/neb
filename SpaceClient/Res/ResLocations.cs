using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Nebula.Client.Res {
    public class ResLocations {

        private Dictionary<string, Location> mLocations = new Dictionary<string, Location>();

        public ResLocations() {
           
        }

        public void Load(string xml) {
            XDocument document = XDocument.Parse(xml);
            var locations = document.Element("zones").Elements("zone").Select(ze => {
                return Location.Parse(ze);
            }).ToList();
            foreach(var loc in locations) {
                mLocations.Add(loc.id, loc);
            }
        }

        public Location GetLocation(string id) {
            if(mLocations.ContainsKey(id)) {
                return mLocations[id];
            }
            return null;
        }
    }
}
