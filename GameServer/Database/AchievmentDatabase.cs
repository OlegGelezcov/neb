using ExitGames.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Nebula.Game;
using Nebula.Game.Components;
using Nebula.Game.Utils;
using System.Collections;

namespace Nebula.Database {
    public class AchievmentDatabase {
        private static readonly ILogger s_Log = LogManager.GetCurrentClassLogger();

        private static readonly object sync = new object();

        private MongoCollection<AchievmentDocument> achievmentDocuments { get; set; }

        private static AchievmentDatabase s_Instance = null;
        private GameApplication m_App;

        public static AchievmentDatabase instance(GameApplication app) {
            if(s_Instance == null ) {
                s_Instance = new AchievmentDatabase(app);
            }
            return s_Instance;
        }

        private AchievmentDatabase(GameApplication app) {
            m_App = app;
            achievmentDocuments = m_App.defaultDatabase.GetCollection<AchievmentDocument>(GameServerSettings.Default.AchievmentCollectionName);
        }

        public void SaveAchievment(string characterId, AchievmentSave achievments ) {
            lock (sync) {
                s_Log.InfoFormat("save achievments for character: {0}".Color(LogColor.yellow), characterId);
                var document = achievmentDocuments.FindOne(Query<AchievmentDocument>.EQ(d => d.characterId, characterId));
                if (document == null) {
                    document = new AchievmentDocument {
                        characterId = characterId
                    };
                }
                document.isNewDocument = false;
                document.Set(achievments);
                achievmentDocuments.Save(document);
            }
        }

        public AchievmentSave LoadAchievments(string characterId, out bool isNew) {
            lock(sync) {
                s_Log.InfoFormat("load achievments for character: {0}".Color(LogColor.orange), characterId);
                var document = achievmentDocuments.FindOne(Query<AchievmentDocument>.EQ(d => d.characterId, characterId));
                if (document != null) {
                    isNew = false;
                    return document.SourceObject();
                } else {
                    isNew = true;
                    document = new AchievmentDocument {
                        characterId = characterId,
                        isNewDocument = isNew,
                        variables = new Hashtable(),
                        visitedZones = new System.Collections.Generic.List<string>(),
                        loreRecords = new System.Collections.Generic.List<string>(),
                        points = 0
                    };
                    achievmentDocuments.Save(document);
                    return document.SourceObject();
                }
            }
        }
    }
}
