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
        //private MongoClient DbClient { get; set; }
        //private MongoServer DbServer { get; set; }
        //private MongoDatabase Database { get; set; }

        public MongoCollection<WorkshopDocument> StationDocuments { get; private set; }

        private GameApplication m_App;
        private static StationDatabase s_Instance = null;

        public static StationDatabase instance(GameApplication app) {
            if (s_Instance == null) {
                s_Instance = new StationDatabase(app);
            }
            return s_Instance;
        }

        private StationDatabase(GameApplication app) {
            //DbClient = new MongoClient(connectionString);
            //DbServer = DbClient.GetServer();
            //Database = DbServer.GetDatabase(GameServerSettings.Default.DatabaseName);
            m_App = app;
            StationDocuments = m_App.defaultDatabase.GetCollection<WorkshopDocument>(GameServerSettings.Default.DatabaseWorkshopCollectionName);
        }

        public void SaveStation(string characterID, WorkhouseStation station) {
            var document = StationDocuments.FindOne(Query<WorkshopDocument>.EQ(d => d.CharacterId, characterID));
            if(document == null ) {
                document = new WorkshopDocument { CharacterId = characterID, petSchemeAdded = false };
                
            }
            document.Set(station);
            StationDocuments.Save(document);
        }

        public WorkhouseStation LoadStation(string characterID, Res resource, out bool isNew) {
            var document = StationDocuments.FindOne(Query<WorkshopDocument>.EQ(d => d.CharacterId, characterID));
            if(document != null ) {
                isNew = false;
                return document.SourceObject(resource);
            } else {
                isNew = true;
                document = new WorkshopDocument {
                    CharacterId = characterID,
                    StationInventoryItems = new List<InventoryItemDocumentElement>(),
                    StationInventoryMaxSlots = 100,
                    petSchemeAdded = false };
                StationDocuments.Save(document);
                return document.SourceObject(resource);
            }
        }
    }
}
