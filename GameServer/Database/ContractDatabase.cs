using ExitGames.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Nebula.Game;
using Nebula.Game.Contracts;
using Nebula.Game.Utils;
using Space.Game;

namespace Nebula.Database {
    public class ContractDatabase {
        private static ILogger s_Log = LogManager.GetCurrentClassLogger();
        private MongoCollection<ContractDocument> m_ContractDocuments;
        private GameApplication m_App;

        private static ContractDatabase s_Instance = null;
        public static ContractDatabase instance(GameApplication app) {
            if (s_Instance == null) {
                s_Instance = new ContractDatabase(app);
            }
            return s_Instance;
        }

        private ContractDatabase(GameApplication app) {
            m_App = app;
            m_ContractDocuments = m_App.defaultDatabase.GetCollection<ContractDocument>(GameServerSettings.Default.ContractCollectionName);
        }

        public void SaveContracts(string characterId, ContractSave save) {
            s_Log.InfoFormat("save contracts for character = {0}".Color(LogColor.red), characterId);
            var document = m_ContractDocuments.FindOne(Query<ContractDocument>.EQ(c => c.characterId, characterId));
            if(document == null ) {
                document = new ContractDocument {
                     characterId = characterId
                };
            }
            document.isNewDocument = false;
            document.Set(save);
            m_ContractDocuments.Save(document);
        }

        public ContractSave LoadContracts(string characterId, IRes resource, out bool isNew ) {
            s_Log.InfoFormat("load contracts for character = {0}", characterId);
            var document = m_ContractDocuments.FindOne(Query<ContractDocument>.EQ(c => c.characterId, characterId));
            if(document != null ) {
                isNew = false;
                return document.SourceObject(resource);
            } else {
                isNew = true;
                document = new ContractDocument {
                    characterId = characterId,
                    isNewDocument = isNew,
                    activeContracts = new System.Collections.Generic.List<System.Collections.Hashtable>(),
                    completedContracts = new System.Collections.Generic.List<System.Collections.Hashtable>()
                };
                m_ContractDocuments.Save(document);
                return document.SourceObject(resource);
            }
        }
    }
}
