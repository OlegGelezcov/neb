using Common;
using System.Collections;

namespace Space.Game.Resources {
    public class MiscInventoryItemData {
        private readonly string id;
        private readonly InventoryObjectType type;
        private readonly string name;
        private readonly Hashtable data;

        public MiscInventoryItemData(string id, InventoryObjectType type, string name, Hashtable data) {
            this.id = id;
            this.type = type;
            this.name = name;
            this.data = data;
        }

        public string Id() {
            return this.id;
        }
        public InventoryObjectType Type() {
            return this.type;
        }
        public string Name() {
            return this.name;
        }
        public Hashtable Data() {
            return this.data;
        }

        public T DataValue<T>(string key) {
            return this.data.GetValue<T>(key, default(T));
        }
    }
}
