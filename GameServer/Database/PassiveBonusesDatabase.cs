﻿using ExitGames.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Database {
    public class PassiveBonusesDatabase {
        private readonly static ILogger log = LogManager.GetCurrentClassLogger();

        //private MongoClient DbClient { get; set; }
        //private MongoServer DbServer { get; set; }
        //private MongoDatabase Database { get; set; }

        private GameApplication m_App;

        private MongoCollection<PassiveBonusesDocument> collection { get; set; }
        private static readonly object sync = new object();

        private static PassiveBonusesDatabase s_Instance = null;
        public static PassiveBonusesDatabase instance(GameApplication app) {
            if (s_Instance == null) {
                s_Instance = new PassiveBonusesDatabase(app);
            }
            return s_Instance;
        }

        public PassiveBonusesDatabase(GameApplication app) {
            m_App = app;
            //DbClient = new MongoClient(connectionString);
            //DbServer = DbClient.GetServer();
            //Database = DbServer.GetDatabase(GameServerSettings.Default.DatabaseName);
            collection = m_App.defaultDatabase.GetCollection<PassiveBonusesDocument>(GameServerSettings.Default.PassiveBonusesCollectionName);
        }


        public void SavePassiveBonuses(string characterID, Dictionary<int, PassiveBonusDbData> bonuses) {
            lock (sync) {
                log.InfoFormat("save passive bonuses {0} [red]", characterID);
                var document = collection.FindOne(Query<PassiveBonusesDocument>.EQ(d => d.characterID, characterID));
                if (document == null) {
                    document = new PassiveBonusesDocument { characterID = characterID };
                }
                document.Set(bonuses);
                collection.Save(document);
            }
        }

        public Dictionary<int, PassiveBonusDbData> LoadPassiveBonuses(string characterID) {
            lock (sync) {
                log.InfoFormat("load passive bonuses for character = {0} [red]", characterID);
                var document = collection.FindOne(Query<PassiveBonusesDocument>.EQ(d => d.characterID, characterID));
                if (document == null) {
                    document = new PassiveBonusesDocument { characterID = characterID, bonuses = new Dictionary<int, PassiveBonusDbData>() };
                    collection.Save(document);
                }
                return document.bonuses;
            }
        }
    }
}
