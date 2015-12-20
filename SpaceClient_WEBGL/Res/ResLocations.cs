
using System.Collections.Generic;
using System.Linq;
#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif

namespace Nebula.Client.Res {
    public class ResLocations {

        private Dictionary<string, Location> mLocations = new Dictionary<string, Location>();

        public ResLocations() {
           
        }

        public void Load(string xml) {
#if UP
            UPXDocument document = new UPXDocument(xml);
#else
            XDocument document = XDocument.Parse(xml);
#endif
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
