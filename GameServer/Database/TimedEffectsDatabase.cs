using ExitGames.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Nebula.Game;
using Nebula.Game.Utils;
using Space.Game;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Database {
    public class TimedEffectsDatabase {
        private static readonly ILogger s_Log = LogManager.GetCurrentClassLogger();
        private MongoCollection<TimedEffectsDocument> m_Collection;

        private static TimedEffectsDatabase s_Instance = null;
        public static TimedEffectsDatabase instance {
            get {
                if(s_Instance == null ) {
                    s_Instance = new TimedEffectsDatabase();
                }
                return s_Instance;
            }
        }

        private TimedEffectsDatabase() {
            m_Collection = GameApplication.Instance.defaultDatabase.GetCollection<TimedEffectsDocument>(GameServerSettings.Default.TimedEffectsCollectionName);
        }

        public void SaveTimedEffects(string characterId, Hashtable save) {
            s_Log.InfoFormat("save timed effects for {0}".Color(LogColor.red), characterId);
            var document = m_Collection.FindOne(Query<TimedEffectsDocument>.EQ(d => d.characterId, characterId));
            if(document == null ) {
                document = new TimedEffectsDocument { characterId = characterId };
            }
            document.isNewDocument = false;
            document.Set(save);
            m_Collection.Save(document);
        }

        public Hashtable LoadTimedEffects(string characterId, Res resource, out bool isNew ) {
            var document = m_Collection.FindOne(Query<TimedEffectsDocument>.EQ(d => d.characterId, characterId));
            if(document != null ) {
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
