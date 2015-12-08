using Common;
using System.Xml.Linq;

namespace Nebula.Server.Components {
    public class DatabaseObjectComponentData : ComponentData {

        public string databaseID { get; private set; }

        public DatabaseObjectComponentData(XElement element) {
            databaseID = element.GetString("database_id");
        }

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
