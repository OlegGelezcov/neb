
using Common;
#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif

namespace Nebula.Server.Components {
    public class DatabaseObjectComponentData : ComponentData {

        public string databaseID { get; private set; }

#if UP
        public DatabaseObjectComponentData(UPXElement element) {
            databaseID = element.GetString("database_id");
        }
#else
        public DatabaseObjectComponentData(XElement element) {
            databaseID = element.GetString("database_id");
        }
#endif
        public DatabaseObjectComponentData(string inDatabaseID) {
            databaseID = inDatabaseID;
        }

        public override ComponentID componentID {
            get {
                return ComponentID.DatabaseObject;
            }
        }
    }
}
