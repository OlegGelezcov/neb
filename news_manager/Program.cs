using Common;
using Master;
using Master.News;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace news_manager {
    class Program {

        private static MongoClient client { get; set; }
        private static MongoServer server { get; set; }
        private static MongoDatabase database { get; set; }
        private static MongoCollection<PostEntry> news { get; set; }

        private static void InitConnection(MasterServerSettings settings) {
            client = new MongoClient("mongodb://localhost");
            server = client.GetServer();
            database = server.GetDatabase(settings.DatabaseName);
            news = database.GetCollection<PostEntry>(settings.NewsCollectionName);
        }

        private static void PrintHeader() {
            Console.WriteLine("Select action:");
            Console.WriteLine("1 - write post");
            Console.WriteLine("2 - get posts");
            Console.WriteLine("3 - delete post");
            Console.WriteLine("4 - write posts from file");
            Console.WriteLine("q - quit");
        }

        private static void WritePostsFromFile() {
            var posts = LoadFromFile();
            foreach(var post in posts) {
                news.Insert(post);
            }
            Console.WriteLine("OK");
        }

        private static void WritePost() {
            string lang = string.Empty;
            string message = string.Empty;
            string postURL = string.Empty;
            string imageURL = string.Empty;

            Console.WriteLine("Input language( en or ru): ");
            lang = Console.ReadLine();
            lang = lang.Trim();

            Console.WriteLine("Input message:");
            message = Console.ReadLine();

            Console.WriteLine("Input link to post:");
            postURL = Console.ReadLine();
            postURL = postURL.Trim();

            Console.WriteLine("Input image URL");
            imageURL = Console.ReadLine();
            imageURL = imageURL.Trim();

            PostEntry entry = new PostEntry {
                imageURL = imageURL,
                lang = lang,
                message = message,
                postID = Guid.NewGuid().ToString(),
                postURL = postURL,
                time = CommonUtils.SecondsFrom1970()
            };
            news.Insert(entry);
            Console.WriteLine("Post inserted");
        }

        private static void GetAllPosts() {
            var posts = news.FindAll().ToList();
            foreach(var post in posts) {
                Console.WriteLine(post.ToString());
                Console.WriteLine("--------------------------");
            }
            Console.WriteLine("===============================");
        }

        private static void DeletePost() {
            Console.WriteLine("Enter post ID:");
            string postID = Console.ReadLine();
            if (!string.IsNullOrEmpty(postID)) {
                news.Remove(Query<PostEntry>.EQ(post => post.postID, postID));
            }
            Console.WriteLine("OK");
        }

        private static List<PostEntry> LoadFromFile() {
            XDocument document = XDocument.Load("posts.xml");
            return document.Element("posts").Elements("post").Select(postElement => {
                string lang = postElement.Element("lang").GetString("value");
                string message = postElement.Element("message").GetString("value");
                string postURL = postElement.Element("url").GetString("value");
                string postImageURL = postElement.Element("image").GetString("value");
                return new PostEntry {
                    imageURL = postImageURL,
                    lang = lang,
                    message = message,
                    postID = Guid.NewGuid().ToString(),
                    postURL = postURL,
                    time = CommonUtils.SecondsFrom1970()
                };
            }).ToList();
        }

        static void Main(string[] args) {

            var settings = MasterServerSettings.Default;
            InitConnection(settings);

            PrintHeader();
            while(true) {
                string input = Console.ReadLine();
                input = input.Trim().ToLower();
                if(input == "q" ) {
                    break;
                } else if(input == "1") {
                    WritePost();
                } else if(input == "2") {
                    GetAllPosts();
                } else if(input == "3") {
                    DeletePost();
                } else if(input == "4") {
                    WritePostsFromFile();
                }
                PrintHeader();
            }

            Console.WriteLine("Bye.");
        }
    }
}
