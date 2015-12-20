
namespace Nebula.Server.Components {
    using Common;
    using ExitGames.Client.Photon;
    using global::Common;
#if UP
    using Nebula.Client.UP;
#else
    using System.Xml.Linq;
#endif

    public class TargetComponentData : ComponentData, IDatabaseComponentData {
#if UP
        public TargetComponentData(UPXElement e) {

        }
#else
        public TargetComponentData(XElement e) {

        }
#endif
        public TargetComponentData(Hashtable hash) {

        }

        public TargetComponentData() { }
        public override ComponentID componentID {
            get {
                return ComponentID.Target;
            }
        }

        public Hashtable AsHash() {
            return new Hashtable();
        }
    }
}
