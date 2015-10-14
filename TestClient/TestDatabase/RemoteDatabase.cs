using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestClient.TestDatabase {
    public class RemoteDatabase {
        private const string CONNECTION_STRING = "mongodb://104.207.135.55";
        private MongoClient mClient;
        private MongoServer mServer;
        private MongoDatabase mDatabase;

        public void Connect() {
            mClient = new MongoClient(CONNECTION_STRING);
            mServer = mClient.GetServer();
            foreach(string databaseName in mServer.GetDatabaseNames()) {
                Console.WriteLine("Found database = {0}", databaseName);
            }
        }
    }
}
