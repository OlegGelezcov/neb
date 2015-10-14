using MongoDB.Bson;
using NebulaCommon;

namespace Nebula.Database {
    public class WorldDocument {
        public ObjectId Id { get; set; }
        public WorldInfo info { get; set; }
    }
}
