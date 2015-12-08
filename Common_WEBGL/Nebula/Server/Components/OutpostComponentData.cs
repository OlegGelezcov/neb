namespace Nebula.Server.Components {
    using Common;
    using System.Xml.Linq;
    using ExitGames.Client.Photon;

    public class OutpostComponentData : ComponentData, IDatabaseComponentData  {
        public OutpostComponentData(XElement e) {

        }

        public OutpostComponentData() { }
        public OutpostComponentData(Hashtable hash) { }
        public override ComponentID componentID {
            get {
                return ComponentID.Outpost;
            }
        }

        public Hashtable AsHash() {
            return new Hashtable();
        }
    }
}
