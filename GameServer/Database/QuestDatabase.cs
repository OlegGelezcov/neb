using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Space.Game;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Database {
    public class QuestDatabase {

        private MongoCollection<QuestDocument> questDocuments { get; set; }
        private static readonly object sync = new object();

        private static QuestDatabase s_Instance = null;
        private GameApplication m_App;

        public static QuestDatabase instance(GameApplication app) {
            if(s_Instance == null ) {
                s_Instance = new QuestDatabase(app);
            }
            return s_Instance;
        }

        private QuestDatabase(GameApplication app ) {
            m_App = app;
            questDocuments = app.defaultDatabase.GetCollection<QuestDocument>(GameServerSettings.Default.DatabaseQuestCollectionName);
        }

        public void SaveQuests(string characterId, Hashtable questHash ) {
            lock(sync) {
                var document = questDocuments.FindOne(Query<QuestDocument>.EQ(d => d.characterId, characterId));
                if(document == null ) {
                    document = new QuestDocument {
                        characterId = characterId
                    };
                }
                document.isNewDocument = false;
                document.Set(questHash);
                questDocuments.Save(document);
            }
        }

        public Hashtable LoadQuests(string characterId, Res resource, out bool isNew ) {
            lock(sync) {
                var document = questDocuments.FindOne(Query<QuestDocument>.EQ(d => d.characterId, characterId));
                if( document != null ) {
                    isNew = false;
                    return document.SourceObject(resource);
                } else {
                    isNew = true;
                    document = new QuestDocument {
                        characterId = characterId,
                        isNewDocument = isNew,
                        questHash = new Hashtable()
                    };
                    questDocuments.Save(document);
                    return document.SourceObject(resource);
                }
            }
        }
    }
}
