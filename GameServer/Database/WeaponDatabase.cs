using ExitGames.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Space.Database;
using Space.Game;

namespace Nebula.Database {
    public class WeaponDatabase {
        private static ILogger log = LogManager.GetCurrentClassLogger();
        //private MongoClient DbClient { get; set; }
        //private MongoServer DbServer { get; set; }
        //private MongoDatabase Database { get; set; }
        private MongoCollection<WeaponDocument> WeaponDocuments { get; set; }

        private static WeaponDatabase s_Instance = null;
        private GameApplication m_App;


        public static WeaponDatabase instance(GameApplication app) {
            if (s_Instance == null) {
                s_Instance = new WeaponDatabase(app);
            }
            return s_Instance;
        }

        private WeaponDatabase(GameApplication app) {
            //DbClient = new MongoClient(connectionString);
            //DbServer = DbClient.GetServer();
            //Database = DbServer.GetDatabase(GameServerSettings.Default.DatabaseName);
            m_App = app;
            WeaponDocuments = m_App.defaultDatabase.GetCollection<WeaponDocument>(GameServerSettings.Default.DatabaseWeaponCollectionName);
        }

        public void SaveWeapon(string characterID, ShipWeaponSave weaponSave) {
            log.InfoFormat("save weapon for character = {0} [red]", characterID);
            var document = WeaponDocuments.FindOne(Query<WeaponDocument>.EQ(d => d.CharacterId, characterID));
            if(document == null ) {
                document = new WeaponDocument { CharacterId = characterID };
            }
            document.IsNewDocument = false;
            document.Set(weaponSave);
            WeaponDocuments.Save(document);
        }

        public ShipWeaponSave LoadWeapon(string characterID, Res resource, out bool isNew) {
            log.InfoFormat("load weapon for character = {0}", characterID);
            var document = WeaponDocuments.FindOne(Query<WeaponDocument>.EQ(d => d.CharacterId, characterID));
            if(document != null ) {
                isNew = false;
                return document.SourceObject(resource);
            } else {
                isNew = true;
                document = new WeaponDocument { CharacterId = characterID, IsNewDocument = isNew, WeaponObject = new System.Collections.Hashtable() };
                WeaponDocuments.Save(document);
                return document.SourceObject(resource);
            }
        }
    }
}
