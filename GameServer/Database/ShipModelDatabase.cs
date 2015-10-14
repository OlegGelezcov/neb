using ExitGames.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Space.Database;
using Space.Game;
using Space.Game.Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Database {
    public class ShipModelDatabase {
        private static ILogger log = LogManager.GetCurrentClassLogger();
        private MongoClient DbClient { get; set; }
        private MongoServer DbServer { get; set; }
        private MongoDatabase Database { get; set; }

        private MongoCollection<ShipModelDocument> ShipModelDocuments { get; set; }

        private static ShipModelDatabase s_Instance = null;
        public static ShipModelDatabase instance {
            get {
                if(s_Instance == null ) {
                    s_Instance = new ShipModelDatabase();
                }
                return s_Instance;
            }
        }

        private ShipModelDatabase() {
            DbClient = new MongoClient(GameServerSettings.Default.DatabaseConnectionString);
            DbServer = DbClient.GetServer();
            Database = DbServer.GetDatabase(GameServerSettings.Default.DatabaseName);
            ShipModelDocuments = Database.GetCollection<ShipModelDocument>(GameServerSettings.Default.DatabaseShipModelCollectionName);
        }

        public void SaveShipModel(string characterID, ShipModel shipModel) {
            log.InfoFormat("save ship model for character = {0} [red]", characterID);
            var document = ShipModelDocuments.FindOne(Query<ShipModelDocument>.EQ(d => d.CharacterId, characterID));
            if(document == null ) {
                document = new ShipModelDocument { CharacterId = characterID };
            }
            document.IsNewDocument = false;
            document.Set(shipModel);
            ShipModelDocuments.Save(document);
        }

        public ShipModel LoadShipModel(string characterID, Res resource, out bool isNew) {
            log.InfoFormat("load ship model for character = {0} [red]", characterID);
            var document = ShipModelDocuments.FindOne(Query<ShipModelDocument>.EQ(d => d.CharacterId, characterID));
            if(document != null ) {
                isNew = false;
                return document.SourceObject(resource);
            } else {
                isNew = true;
                document = new ShipModelDocument {
                    CharacterId = characterID,
                    IsNewDocument = isNew
                };
                ShipModelDocuments.Save(document);
                return document.SourceObject(resource);
            }
        }
    }
}
