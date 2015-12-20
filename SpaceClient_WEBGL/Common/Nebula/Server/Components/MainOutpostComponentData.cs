using Common;
using ExitGames.Client.Photon;
#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif

namespace Nebula.Server.Components {
    public class MainOutpostComponentData : ComponentData, IDatabaseComponentData{

#if UP
        public MainOutpostComponentData(UPXElement e) { }
#else
        public MainOutpostComponentData(XElement e) { }
#endif
        public MainOutpostComponentData() { }

        public MainOutpostComponentData(Hashtable hash) { }

        public override ComponentID componentID {
            get {
                return ComponentID.MainOutpost;
            }
        }

        public Hashtable AsHash() {
            return new Hashtable();
        }
    }
}
