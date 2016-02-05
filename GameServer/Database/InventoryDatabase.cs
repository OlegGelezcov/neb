using ExitGames.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Space.Database;
using Space.Game;
using Space.Game.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Database {
    public class InventoryDatabase {

        private static ILogger log = LogManager.GetCurrentClassLogger();

        //private MongoClient DbClient { get; set; }
        //private MongoServer DbServer { get; set; }
        //private MongoDatabase Database { get; set; }

        private GameApplication m_App;


        private MongoCollection<InventoryDocument> InventoryDocuments { get; set; }

        private static InventoryDatabase s_Instance = null;

        public static InventoryDatabase instance(GameApplication app) {
            if (s_Instance == null) {
                s_Instance = new InventoryDatabase(app);
            }
            return s_Instance;
        }

        private InventoryDatabase(GameApplication app) {
            m_App = app;
            //DbClient = new MongoClient(connectionString);
            //DbServer = DbClient.GetServer();
            //Database = DbServer.GetDatabase(GameServerSettings.Default.DatabaseName);
            InventoryDocuments = m_App.defaultDatabase.GetCollection<InventoryDocument>(GameServerSettings.Default.DatabaseInventoryCollectionName);
        }

        public void SaveInventory(string characterID, ServerInventory serverInventory) {
            log.InfoFormat("SAVE INVENTORY FOR CHAR = {0}  INV COUNT = {1} [red]", characterID, serverInventory.SlotsUsed);
            var document = InventoryDocuments.FindOne(Query<InventoryDocument>.EQ(d => d.CharacterId, characterID));
            if(document == null ) {
                document = new InventoryDocument { CharacterId = characterID };
                
            }
            document.Set(serverInventory);
            InventoryDocuments.Save(document);
        }

        public ServerInventory LoadInventory(string characterID, Res resource) {
            log.InfoFormat("LOAD INVENTORY FOR CHAR = {0} [red]", characterID);
            var document = InventoryDocuments.FindOne(Query<InventoryDocument>.EQ(d => d.CharacterId, characterID));
            if(document != null ) {
                return document.SourceObject(resource);
            } else {
                document = new InventoryDocument { CharacterId = characterID, Items = new List<InventoryItemDocumentElement>(), MaxSlots = 0};
                InventoryDocuments.Save(document);
                return document.SourceObject(resource);
            }
        }
    }
}
