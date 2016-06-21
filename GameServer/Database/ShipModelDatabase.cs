using ExitGames.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Space.Database;
using Space.Game;
using Space.Game.Ship;

namespace Nebula.Database {
    public class ShipModelDatabase {
        private static ILogger log = LogManager.GetCurrentClassLogger();
        //private MongoClient DbClient { get; set; }
        //private MongoServer DbServer { get; set; }
        //private MongoDatabase Database { get; set; }
        private GameApplication m_App;

        private MongoCollection<ShipModelDocument> ShipModelDocuments { get; set; }
        private static readonly object sync = new object();

        private static ShipModelDatabase s_Instance = null;
        public static ShipModelDatabase instance(GameApplication app) {
            if (s_Instance == null) {
                s_Instance = new ShipModelDatabase(app);
            }
            return s_Instance;
        }

        private ShipModelDatabase(GameApplication app) {
            m_App = app;
            //DbClient = new MongoClient(connectionString);
            //DbServer = DbClient.GetServer();
            //Database = DbServer.GetDatabase(GameServerSettings.Default.DatabaseName);
            ShipModelDocuments = m_App.defaultDatabase.GetCollection<ShipModelDocument>(GameServerSettings.Default.DatabaseShipModelCollectionName);
        }

        public void SaveShipModel(string characterID, ShipModel shipModel) {
            lock (sync) {
                log.InfoFormat("save ship model for character = {0} [red]", characterID);
                var document = ShipModelDocuments.FindOne(Query<ShipModelDocument>.EQ(d => d.CharacterId, characterID));
                if (document == null) {
                    document = new ShipModelDocument { CharacterId = characterID };
                }
                document.IsNewDocument = false;
                document.Set(shipModel);
                ShipModelDocuments.Save(document);
            }
        }

        public ShipModel LoadShipModel(string characterID, Res resource, out bool isNew) {
            lock (sync) {
                log.InfoFormat("load ship model for character = {0} [red]", characterID);
                var document = ShipModelDocuments.FindOne(Query<ShipModelDocument>.EQ(d => d.CharacterId, characterID));
                if (document != null) {
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
}
