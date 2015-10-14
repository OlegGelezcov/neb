using Master.News;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System.Collections.Generic;
using System.Linq;

namespace Master {
    public class DatabaseManager {
        public MongoClient client { get; private set; }
        public MongoServer server { get; private set; }
        public MongoDatabase database { get; private set; }
        public MongoCollection<PostEntry> news { get; private set; }

        public void Setup(string connectionString, string databaseName, string collectionName) {
            client = new MongoClient(connectionString);
            server = client.GetServer();
            database = server.GetDatabase(databaseName);
            news = database.GetCollection<PostEntry>(collectionName);
        }

        public List<PostEntry> GetAllPosts(string lang) {
            return news.Find(Query<PostEntry>.EQ(post => post.lang, lang)).OrderByDescending(post => post.time).ToList();
        }

        public List<PostEntry> GetAllPosts() {
            return news.FindAll().OrderByDescending(post => post.time).ToList();
        }
    }
}
