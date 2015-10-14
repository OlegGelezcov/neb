using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestClient {
    public class MongoTester {

        public MongoClient MClient { get; private set; }
        public MongoServer MServer { get; private set; }
        public MongoDatabase MDatabase { get; private set; }

        public MongoCollection<Entity> Entities { get; private set; }

        public void Setup() {
            this.MClient = new MongoClient(ClientSettings.Default.MongoConnectionString);
            this.MServer = this.MClient.GetServer();
            this.MDatabase = this.MServer.GetDatabase("test_client");
            this.Entities = this.MDatabase.GetCollection<Entity>("entities");


            var e1 = new Entity { Name = "Oleg" };
            this.Entities.Insert(e1);
            
        }
    }

    public class Entity {
        public ObjectId Id { get; set; }
        public string Name { get; set; } 
    }
}
