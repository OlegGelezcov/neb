using Common;
using ExitGames.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using NebulaCommon.SelectCharacter;
using Space.Database;
using Space.Game;

namespace Nebula.Database {
    public class CharacterDatabase {
        private static ILogger log = LogManager.GetCurrentClassLogger();
        private static readonly object sync = new object();

        private GameApplication m_Application;
        //private MongoClient DbClient { get; set; }
        //private MongoServer DbServer { get; set; }
        //private MongoDatabase Database { get; set; }

        public MongoCollection<StatsDocument> CharacterDocuments { get; private set; }

        private static CharacterDatabase s_Instance = null;
        public static CharacterDatabase instance(GameApplication app) {
            if (s_Instance == null) {
                s_Instance = new CharacterDatabase(app);
            }
            return s_Instance;
        }

        public CharacterDatabase(GameApplication app) {
            m_Application = app;
            CharacterDocuments = m_Application.defaultDatabase.GetCollection<StatsDocument>(GameServerSettings.Default.DatabaseStatsCollectionName);
        }

        public void SaveCharacter(string characterID, PlayerCharacter character ) {
            lock(sync) {
                var document = CharacterDocuments.FindOne(Query<StatsDocument>.EQ(d => d.CharacterId, characterID));
                if (document == null) {
                    document = new StatsDocument { CharacterId = characterID };
                }
                document.Set(character);
                document.IsNewCharacter = false;
                CharacterDocuments.Save(document);
            }
        }

        public PlayerCharacter LoadCharacter(string characterID, Res resource, out bool isNew) {
            lock(sync) {
                var document = CharacterDocuments.FindOne(Query<StatsDocument>.EQ(d => d.CharacterId, characterID));
                if (document != null) {
                    isNew = false;
                    return document.SourceObject(resource);
                } else {
                    document = new StatsDocument { CharacterId = characterID, Exp = 0, Model = new System.Collections.Hashtable { }, Name = "", Race = (int)Race.None, Workshop = (int)Workshop.Arlen };
                    document.IsNewCharacter = true;
                    CharacterDocuments.Save(document);
                }
                isNew = document.IsNewCharacter;
                return document.SourceObject(resource);
            }
        }

    }
}
