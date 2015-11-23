using Common;
using ExitGames.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Nebula;
using Nebula.Database;
using NebulaCommon;
using Space.Game;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Space.Database {
    public class DatabaseManager {

        public MongoClient DbClient { get; private set; }
        public MongoServer DbServer { get; private set; }
        public MongoDatabase Database { get; private set; }
 
        public MongoCollection<WorldDocument> Worlds { get; private set; }

        //Full world states
        public MongoCollection<WorldState> worldStates { get; private set; }


        public void Setup(string connectionString) {
            DbClient = new MongoClient(connectionString);
            
            DbServer = DbClient.GetServer();

           
            foreach(var cred in DbClient.Settings.Credentials) {
                log.InfoFormat("credentials = {0}:{1} [red]", cred.Username, cred.Password);
            }

            Database = DbServer.GetDatabase(GameServerSettings.Default.DatabaseName);
            
            Worlds                  = Database.GetCollection<WorldDocument>(GameServerSettings.Default.WorldCollection);
            worldStates = Database.GetCollection<WorldState>(GameServerSettings.Default.WorldStateCollectionName);
            log.InfoFormat("world count = {0} [red]", Worlds.Count());
            //log.InfoFormat("world count: {0} [red]", )
        }

        public WorldState GetWorldState(string worldID) {
            return worldStates.FindOne(Query<WorldState>.EQ(ws => ws.worldID, worldID));
        }

        /// <summary>
        /// Find world by ID
        /// </summary>
        /// <param name="worldID">World ID to be found</param>
        /// <returns>World info or null if not found</returns>
        public WorldDocument GetWorld(string worldID) {
            //search world by id query
            var query = Query<WorldDocument>.EQ(world => world.info.worldID, worldID);
            var result = Worlds.FindOne(query);
            if(result == null) {
                return null;
            }
            return result;
        }

        /// <summary>
        /// Return all worlds state
        /// </summary>
        /// <returns></returns>
        public ConcurrentDictionary<string, WorldInfo> GetWorlds() {
            //get all worlds from database
            var result = Worlds.FindAll().ToList();

            //construct dictionary from query result
            ConcurrentDictionary<string, WorldInfo> worldInfoDict = new ConcurrentDictionary<string, WorldInfo>();
            foreach (var world in result) {
                if (world.info != null) {
                    worldInfoDict.TryAdd(world.info.worldID, world.info);
                }
            }
            return worldInfoDict;
        }

        /// <summary>
        /// Save world info, if world info don't exists create and save new world info
        /// </summary>
        /// <param name="world">Target world to be saved</param>
        public void SetWorld(MmoWorld world) {
            var document = GetWorld(world.Zone.Id);
            if(document == null ) {
                //world info not found in DB, than create new world info
                var worldInfo = new WorldInfo {
                    worldID = world.Zone.Id,
                    currentRace = (int)(byte)world.ownedRace,
                    startRace = (int)(byte)world.Zone.InitiallyOwnedRace,
                    underAttack = world.underAttack,
                    worldType = (int)world.Zone.worldType,
                    attackRace = (byte)world.attackedRace,
                     playerCount = world.playerCount
                };
                document = new WorldDocument { info = worldInfo };           
            } else {
                //check the document info not null
                if(document.info == null ) {
                    document.info = new WorldInfo { worldID = world.Zone.Id };
                }

                //world info found in db, get world info and update data from MmoWorld
                document.info.currentRace = (int)(byte)world.ownedRace;
                document.info.startRace = (int)(byte)world.Zone.InitiallyOwnedRace;
                document.info.underAttack = world.underAttack;
                document.info.worldType = (int)world.Zone.worldType;
                document.info.attackRace = (byte)world.attackedRace;
                document.info.playerCount = world.playerCount;
            }
            /*
            log.InfoFormat("save world state = [{0}, {1}, {2}, {3}, {4}, {5}] [red]", 
                document.info.worldID,
                (Race)(byte)document.info.currentRace, 
                (Race)(byte)document.info.startRace, 
                document.info.underAttack, 
                (WorldType)(int)document.info.worldType, 
                document.info.playerCount);*/
            //save updated world info
            Worlds.Save(document);
        }


        private static readonly ILogger log = LogManager.GetCurrentClassLogger();



    }
}
