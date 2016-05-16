using Login;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using SelectCharacter;
using SelectCharacter.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dbtool {
    class Program {
        static void Main(string[] args) {

            
            var settings = GetSettings();
            var client = new MongoClient("mongodb://104.207.135.55");

             var server = client.GetServer();
            /*var characaterDatabase = server.GetDatabase(settings.DatabaseName);
            var characterCollection = characaterDatabase.GetCollection<DbPlayerCharactersObject>(settings.DatabaseCollectionName);
            //Console.WriteLine(characterCollection.Count());
            
            foreach(var player in characterCollection.FindAll()) {
                if(player.Characters != null ) {
                    foreach(var ch in player.Characters ) {
                        if(ch.Name == "f18k21" ) {
                            Console.WriteLine("founded!");
                            Console.WriteLine("login: " + player.Login);
                            Console.WriteLine("gameref: " + player.GameRefId);
                            Console.WriteLine("characterid: " + ch.CharacterId);
                        }
                    }
                }
            }*/

            var database = server.GetDatabase("user_logins");
            var logins = database.GetCollection<DbUserLogin>("user_login_collection");
            Console.WriteLine("logins: " + logins.Count());
            foreach(var login in logins.FindAll()) {
                if(login.login.ToLower() == "fvbhua" ) {
                    Console.WriteLine("found password: " + login.password);
                    break;
                }
            }

        }

        private static SelectCharacterSettings GetSettings() {
            return new SelectCharacterSettings {
                GamingTcpPort = 4563,
                GamingUdpPort = 5108,
                GamingWebSocketPort = 9093,
                ConnectRetryInterval = 15,
                PublicIPAddress = "45.63.0.198",
                MasterIPAddress = "45.63.0.198",
                OutgoingMasterServerPeerPort = 4520,
                DatabaseCollectionName = "character_collection",
                MaxPlayerCharactersCount = 5,
                DatabaseName = "characters",
                DatabaseSaveInterval = 6
            };
        }
    }
}
