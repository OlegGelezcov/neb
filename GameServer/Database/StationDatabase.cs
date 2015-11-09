using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Space.Database;
using Space.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Database {
    public class StationDatabase {
        private MongoClient DbClient { get; set; }
        private MongoServer DbServer { get; set; }
        private MongoDatabase Database { get; set; }

        public MongoCollection<WorkshopDocument> StationDocuments { get; private set; }

        private static StationDatabase s_Instance = null;

        public static StationDatabase instance {
            get {
                if(s_Instance == null ) {
                    s_Instance = new StationDatabase(GameApplication.Instance.databaseConnectionString);
                }
                return s_Instance;
            }
        }

        private StationDatabase(string connectionString) {
            DbClient = new MongoClient(connectionString);
            DbServer = DbClient.GetServer();
            Database = DbServer.GetDatabase(GameServerSettings.Default.DatabaseName);
            StationDocuments = Database.GetCollection<WorkshopDocument>(GameServerSettings.Default.DatabaseWorkshopCollectionName);
        }

        public void SaveStation(string characterID, WorkhouseStation station) {
            var document = StationDocuments.FindOne(Query<WorkshopDocument>.EQ(d => d.CharacterId, characterID));
            if(document == null ) {
                document = new WorkshopDocument { CharacterId = characterID };
                
            }
            document.Set(station);
            StationDocuments.Save(document);
        }

        public WorkhouseStation LoadStation(string characterID, Res resource) {
            var document = StationDocuments.FindOne(Query<WorkshopDocument>.EQ(d => d.CharacterId, characterID));
            if(document != null ) {
                return document.SourceObject(resource);
            } else {
                document = new WorkshopDocument { CharacterId = characterID, StationInventoryItems = new List<InventoryItemDocumentElement>(), StationInventoryMaxSlots = 100 };
                StationDocuments.Save(document);
                return document.SourceObject(resource);
            }
        }
    }
}
