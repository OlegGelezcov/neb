
using System.Collections.Generic;
using System.Linq;
#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif

namespace Nebula.Client.Res {
    public class ResAsteroids {

        private Dictionary<string, AsteroidData> mAsteroids = new Dictionary<string, AsteroidData>();

        public void Load(string xml) {
            mAsteroids.Clear();
#if UP
            UPXDocument document = new UPXDocument(xml);
#else
            XDocument document = XDocument.Parse(xml);
#endif
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
