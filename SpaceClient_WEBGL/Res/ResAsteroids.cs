
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Nebula.Client.Res {
    public class ResAsteroids {

        private Dictionary<string, AsteroidData> mAsteroids = new Dictionary<string, AsteroidData>();

        public void Load(string xml) {
            mAsteroids.Clear();
            XDocument document = XDocument.Parse(xml);
            mAsteroids = document.Element("asteroids").Elements("asteroid").Select(e => {
                AsteroidData data = new AsteroidData(e);
                return data;
            }).ToDictionary(d => d.ID, d => d);
        }

        public bool TryGetAsteroidData(string id, out AsteroidData data) {
            if(mAsteroids.ContainsKey(id)) {
                data = mAsteroids[id];
                return true;
            }
            data = null;
            return false;
        }
    }
}
