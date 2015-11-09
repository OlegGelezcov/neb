using Common;
using MongoDB.Driver;
using Space.Database;
using System;

namespace TestClient {
    public class TestConnectingToMongoWithUserAndPassword {
        private MongoClient mClient;
        private MongoServer mServer;
        private MongoDatabase mDatabase;
        private MongoCollection<Master.News.PostEntry> mNews;

        public void Connect() {
            string connectionString = "mongodb://root:87e898AA@192.168.1.102";
            mClient = new MongoClient(connectionString);
            mServer = mClient.GetServer();
            mDatabase = mServer.GetDatabase("nebula");
            mNews = mDatabase.GetCollection<Master.News.PostEntry>("news");
        }

        public void Insert() {
            Master.News.PostEntry post = new Master.News.PostEntry {
                imageURL = "http://google.com",
                lang = "en",
                message = "test message for testing",
                postID = Guid.NewGuid().ToString(),
                postURL = "http://apple.com",
                time = CommonUtils.SecondsFrom1970()
            };
            mNews.Insert(post);

        }

        public void Read() {
            var posts = mNews.FindAll();
            foreach(var p in posts) {
                Console.WriteLine(p.postID);
            }
        }

        public void CheckUsers() {
            var users = mDatabase.FindAllUsers();
            foreach(var user in users ) {
                Console.WriteLine(user.Username + ": " + user.ToString());
            }
        }


        public void TestDatabaseManager() {
            DatabaseManager db = new DatabaseManager();
            db.Setup("mongodb://borg:87e898AA@localhost/nebula");
            Console.WriteLine(db.Worlds.Count());
        }
    }
}
