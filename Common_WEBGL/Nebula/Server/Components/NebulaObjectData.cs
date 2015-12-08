using Common;
using GameMath;
using System.Collections.Generic;

namespace Nebula.Server.Components {
    public class NebulaObjectData {

        public string ID { get; set; } = string.Empty;
        public Vector3 position { get; set; } = new Vector3();
        public Vector3 rotation { get; set; } = new Vector3();
        //public string scriptFile { get; set; } = string.Empty;
        //public readonly Script script = new Script();

        public Dictionary<ComponentID, ComponentData> componentCollection { get; set; } = new Dictionary<ComponentID, ComponentData>();

        public bool hasDatabaseComponent {
            get {
                if(componentCollection == null ) {
                    return false;
                }
                return componentCollection.ContainsKey(ComponentID.DatabaseObject);
            }
        }

        public string databaseID {
            get {
                if(hasDatabaseComponent) {
                    DatabaseObjectComponentData data = componentCollection[ComponentID.DatabaseObject] as DatabaseObjectComponentData;
                    if(data != null ) {
                        return data.databaseID;
                    }
                }
                return string.Empty;
            }
        }
    }
}
