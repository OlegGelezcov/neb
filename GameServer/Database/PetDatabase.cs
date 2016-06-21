using ExitGames.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Nebula.Game;
using Nebula.Game.Utils;
using Nebula.Pets;
using Space.Game;
using System.Collections.Generic;

namespace Nebula.Database {
    public class PetDatabase {
        private static ILogger s_Log = LogManager.GetCurrentClassLogger();

        private MongoCollection<PetDocument> m_PetDocuments;
        private static readonly object sync = new object();

        private static PetDatabase s_Instance = null;

        private GameApplication m_App;

        public static PetDatabase instance(GameApplication app) {
            if (s_Instance == null) {
                s_Instance = new PetDatabase(app);
            }
            return s_Instance;
        }

        private PetDatabase(GameApplication app) {
            m_App = app;
            m_PetDocuments = m_App.defaultDatabase.GetCollection<PetDocument>(GameServerSettings.Default.PetCollectionName);
        }

        public void SavePets(string characterID, PetCollection pets) {
            lock (sync) {
                s_Log.InfoFormat("save pets for character = {0}".Color(LogColor.red), characterID);
                var document = m_PetDocuments.FindOne(Query<PetDocument>.EQ(doc => doc.characterId, characterID));
                if (document == null) {
                    document = new PetDocument { characterId = characterID };
                }
                document.isNewDocument = false;
                document.Set(pets.GetSave());
                m_PetDocuments.Save(document);
            }
        }

        public PetCollection LoadPets(string characterID, Res resource, out bool isNew ) {
            lock (sync) {
                s_Log.InfoFormat("load pets for character = {0}".Color(LogColor.red), characterID);
                var document = m_PetDocuments.FindOne(Query<PetDocument>.EQ(doc => doc.characterId, characterID));
                if (document != null) {
                    isNew = false;
                    return document.SourceObject(resource);
                } else {
                    isNew = true;
                    document = new PetDocument {
                        characterId = characterID,
                        isNewDocument = isNew,
                        pets = new List<PetSave>()
                    };
                    m_PetDocuments.Save(document);
                    return document.SourceObject(resource);
                }
            }
        }
    }
}
