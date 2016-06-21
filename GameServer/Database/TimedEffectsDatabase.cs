using ExitGames.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Nebula.Game;
using Nebula.Game.Utils;
using Space.Game;
using System.Collections;

namespace Nebula.Database {
    public class TimedEffectsDatabase {
        private static readonly ILogger s_Log = LogManager.GetCurrentClassLogger();
        private MongoCollection<TimedEffectsDocument> m_Collection;
        private static readonly object sync = new object();

        private static TimedEffectsDatabase s_Instance = null;
        private GameApplication m_App;

        public static TimedEffectsDatabase instance(GameApplication app) {
            if (s_Instance == null) {
                s_Instance = new TimedEffectsDatabase(app);
            }
            return s_Instance;
        }

        private TimedEffectsDatabase(GameApplication app) {
            m_App = app;
            m_Collection = m_App.defaultDatabase.GetCollection<TimedEffectsDocument>(GameServerSettings.Default.TimedEffectsCollectionName);
        }

        public void SaveTimedEffects(string characterId, Hashtable save) {
            lock(sync ) {
                s_Log.InfoFormat("save timed effects for {0}".Color(LogColor.red), characterId);
                var document = m_Collection.FindOne(Query<TimedEffectsDocument>.EQ(d => d.characterId, characterId));
                if (document == null) {
                    document = new TimedEffectsDocument { characterId = characterId };
                }
                document.isNewDocument = false;
                document.Set(save);
                m_Collection.Save(document);
            }
        }

        public Hashtable LoadTimedEffects(string characterId, Res resource, out bool isNew ) {
            lock (sync) {
                var document = m_Collection.FindOne(Query<TimedEffectsDocument>.EQ(d => d.characterId, characterId));
                if (document != null) {
                    isNew = false;
                    return document.SourceObject(resource);
                } else {
                    isNew = true;
                    document = new TimedEffectsDocument {
                        characterId = characterId,
                        isNewDocument = isNew,
                        effects = new Hashtable()
                    };
                    m_Collection.Save(document);
                    return document.SourceObject(resource);
                }
            }
        }
    }
}
