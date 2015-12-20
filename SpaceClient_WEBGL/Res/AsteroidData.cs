#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif
using Common;

namespace Nebula.Client.Res {
    public class AsteroidData {
        public string ID { get; private set; }
        public string nameID { get; private set; }
        public int quality { get; private set; }

#if UP
        public AsteroidData(UPXElement e) {
            ID = e.GetString("id");
            nameID = e.GetString("name");
            quality = e.GetInt("quality");
        }
#else
        public AsteroidData(XElement e) {
            ID = e.GetString("id");
            nameID = e.GetString("name");
            quality = e.GetInt("quality");
        }
#endif
    }
}
