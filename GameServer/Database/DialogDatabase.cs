using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Space.Game;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Database {
    public class DialogDatabase {
        private MongoCollection<DialogDocument> dialogDocuments { get; set; }
        private static readonly object sync = new object();

        private static DialogDatabase s_Instance = null;
        private GameApplication m_App;

        public static DialogDatabase instance(GameApplication app ) {
            if(s_Instance == null ) {
                s_Instance = new DialogDatabase(app);
            }
            return s_Instance;
        }

        private DialogDatabase(GameApplication app) {
            m_App = app;
            dialogDocuments = app.defaultDatabase.GetCollection<DialogDocument>(GameServerSettings.Default.DatabaseDialogCollectionName);
        }

        public void SaveDialogs(string characterId, Hashtable dialogHash ) {
            lock(sync) {
                var document = dialogDocuments.FindOne(Query<DialogDocument>.EQ(d => d.characterId, characterId));
                if(document == null ) {
                    document = new DialogDocument {
                        characterId = characterId
                    };
                }
                document.isNewDocument = false;
                document.Set(dialogHash);
                dialogDocuments.Save(document);
            }
        }

        public Hashtable LoadDialogs(string characterId, Res resource, out bool isNew) {
            lock(sync) {
                var document = dialogDocuments.FindOne(Query<DialogDocument>.EQ(d => d.characterId, characterId));
                if(document != null ) {
                    isNew = false;
                    return document.SourceObject(resource);
                } else {
                    isNew = true;
                    document = new DialogDocument {
                        characterId = characterId,
                        isNewDocument = isNew,
                        dialogHash = new Hashtable()
                    };
                    dialogDocuments.Save(document);
                    return document.SourceObject(resource);
                }
            }
        }
    }
}
