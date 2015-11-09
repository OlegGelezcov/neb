using ExitGames.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Space.Database;
using Space.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Database {
    public class SkillDatabase {
        private static ILogger log = LogManager.GetCurrentClassLogger();
        private MongoClient DbClient { get; set; }
        private MongoServer DbServer { get; set; }
        private MongoDatabase Database { get; set; }

        private MongoCollection<SkillsDocument> SkillsDocuments { get; set; }

        private static SkillDatabase s_Instance;

        public static SkillDatabase instance {
            get {
                if(s_Instance == null ) {
                    s_Instance = new SkillDatabase(GameApplication.Instance.databaseConnectionString);
                }
                return s_Instance;
            }
        }

        private SkillDatabase(string connectionString) {
            DbClient = new MongoClient(connectionString);
            DbServer = DbClient.GetServer();
            Database = DbServer.GetDatabase(GameServerSettings.Default.DatabaseName);
            SkillsDocuments = Database.GetCollection<SkillsDocument>(GameServerSettings.Default.DatabaseSkillsCollectionName);
        }

        public void SaveSkills(string characterID, PlayerSkillsSave skillSave ) {
            log.InfoFormat("save skills for character = {0} [red]", characterID);
            var document = SkillsDocuments.FindOne(Query<SkillsDocument>.EQ(d => d.CharacterId, characterID));
            if(document == null ) {
                document = new SkillsDocument { CharacterId = characterID };
            }
            document.IsNewDocument = false;
            document.Set(skillSave);
            SkillsDocuments.Save(document);
        }

        public PlayerSkillsSave LoadSkills(string characterID, Res resource, out bool isNew ) {
            log.InfoFormat("load skills for character = {0} [red]", characterID);
            var document = SkillsDocuments.FindOne(Query<SkillsDocument>.EQ(d => d.CharacterId, characterID));
            if(document != null) {
                isNew = false;
                return document.SourceObject(resource);
            } else {
                isNew = true;
                document = new SkillsDocument {
                    CharacterId = characterID,
                    IsNewDocument = isNew,
                    Skills = new Dictionary<int, int> { { 0, -1 }, { 1, -1 }, { 2, -1 }, { 3, -1 }, { 4, -1 }, { 5, -1 } }
                };
                SkillsDocuments.Save(document);
                return document.SourceObject(resource);
            }
        }
    }
}
