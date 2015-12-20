namespace Nebula.Server.Components {
    using Common;
    using System;
    using ExitGames.Client.Photon;
#if UP
    using Nebula.Client.UP;
#else
    using System.Xml.Linq;

#endif
    using global::Common;

    public class OutpostComponentData : ComponentData, IDatabaseComponentData  {

#if UP
        public OutpostComponentData(UPXElement e) {

        }
#else
        public OutpostComponentData(XElement e) {

        }
#endif
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
