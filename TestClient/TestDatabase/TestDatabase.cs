using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Bson;
using System.Collections;

namespace TestClient.TestDatabase {

    public class DBObject {
        public ObjectId Id { get; set; }
        public int data { get; set; }
    }

    public class Filter {
        public bool Check(int code) {
            //Console.WriteLine("check call");
            return code % 2 == 0;
        }
    }



    public class TestDatabase {
        string connectionString = "mongodb://localhost";

        public MongoClient DbClient { get; private set; }
        public MongoServer DbServer { get; private set; }

        public MongoDatabase Database { get; private set; }

        public MongoCollection<DatabaseData> Collection { get; set; }
        public MongoCollection<DBObject> Collection2 { get; set; }


        public void CursorsPrepare() {
            this.DbClient = new MongoClient(connectionString);
            this.DbServer = this.DbClient.GetServer();
            this.Database = this.DbServer.GetDatabase("test_client");
            Collection2 = Database.GetCollection<DBObject>("cursors_sample");

            for (int i = 1; i < 10000; i++) {
                DBObject obj = new DBObject { data = i };
                Collection2.Insert(obj);
            }
            Console.WriteLine("data writtent");
        }

        public void TestCursors() {
            
            this.DbClient = new MongoClient(connectionString);
            this.DbServer = this.DbClient.GetServer();
            this.Database = this.DbServer.GetDatabase("test_client");
            Collection2 = Database.GetCollection<DBObject>("cursors_sample");

            MongoCursor<DBObject> cursor = Collection2.FindAll();
            //Console.WriteLine("cursor count = {0}", cursor.Count());

            cursor.SetSkip(0);
           
            var lst = cursor.Take(10).ToList();
            foreach(var l in lst) {
                Console.Write(l.data + ", " );
            }
            Console.WriteLine();

            cursor = Collection2.FindAll();
            cursor.SetSkip(10);
            lst = cursor.Take(10).ToList();
            foreach (var l in lst) {
                Console.Write(l.data + ", ");
            }
            Console.WriteLine();

            cursor = Collection2.FindAll();
            cursor.SetSkip(0);
            lst = cursor.Take(10).ToList();
            foreach (var l in lst) {
                Console.Write(l.data + ", ");
            }
            Console.WriteLine();

            Console.WriteLine("cursor count = {0}", cursor.Count());

            cursor = Collection2.FindAll();
            Filter filter = new Filter();
            var filtered = cursor.Where(obj => filter.Check(obj.data)).Skip(10).Take(10).ToList();
            Console.WriteLine("filtered count: " + filtered.Count);
            foreach(var o in filtered) {
                Console.Write(o.data + ", ");
            }
            Console.WriteLine();
        }



        public void Connect() {
            this.DbClient = new MongoClient(connectionString);
            this.DbServer = this.DbClient.GetServer();
            this.Database = this.DbServer.GetDatabase("test_client");
            Collection = Database.GetCollection<DatabaseData>("my_collection");
        }

        public void SaveData() {
            /*
            Dictionary<string, OtherData> dict = new Dictionary<string, OtherData> {
                { "dic1", new OtherData {  otherDataName = "name 1"} },
                { "dict2", new OtherData { otherDataName = "nemr 3"} }
            };*/
            Dictionary<string, OtherData> dict = new Dictionary<string, OtherData>();
            DatabaseData data = new DatabaseData {
                time = DateTime.Now,
                name = "Oleg Data",
                Data = new SubData {
                    number = 15,
                    someString = "Oleg test strog",
                    someHash = new Hashtable { { "first_key", "first k value"}, { "second key", 3.5f } },
                    Dict = dict
                },
                Id = new ObjectId("5575ac9e88dba11b68498645")
            };
            Collection.Save(data);
        }

        public void ReadData() {
            var query = Query<DatabaseData>.EQ(d => d.Id, new ObjectId("5575ac9e88dba11b68498645"));
            var result = Collection.FindOne(query);
            if(result != null ) {
                Console.WriteLine(result.ToString());
            } else {
                Console.WriteLine("result not found");
            }
        }

    }

    public class DatabaseData {
        public DateTime time { get; set; }
        public ObjectId Id { get; set; }
        public string name { get; set; }
        public SubData Data { get; set; }
        public override string ToString() {
            var builder = new StringBuilder();
            builder.AppendLine(string.Format("name = {0}", name));
            builder.AppendLine(Data.ToString());
            builder.AppendLine(string.Format("time = {0}", time));
            return builder.ToString();
        }
    }

    public class SubData {
        public int number { get; set; }
        public string someString { get; set; }
        public Hashtable someHash { get; set; }
        public Dictionary<string, OtherData> Dict { get; set; }

        public override string ToString() {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(string.Format("number = {0}, some string = {1}", number, someString));
            foreach(DictionaryEntry entry in someHash) {
                builder.AppendLine(string.Format("{0} = {1}", entry.Key, entry.Value));
            }
            builder.AppendLine("Dictionary-------------");
            foreach(var entry in Dict) {
                builder.AppendLine(string.Format("{0} = {1}", entry.Key, entry.Value.otherDataName));
            }

            return builder.ToString();
        }
    }

    public class OtherData {
        public string otherDataName { get; set; }
    }
}
