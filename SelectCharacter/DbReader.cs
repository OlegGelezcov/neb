using MongoDB.Driver;
using MongoDB.Driver.Builders;
using SelectCharacter.Auction;
using SelectCharacter.Characters;
using SelectCharacter.Chat;
using SelectCharacter.Commander;
using SelectCharacter.Friends;
using SelectCharacter.Guilds;
using SelectCharacter.Mail;
using SelectCharacter.Notifications;
using SelectCharacter.Races;
using SelectCharacter.Store;

namespace SelectCharacter {
    public class DbReader {

        public MongoClient DbClient { get; private set; }
        public MongoServer DbServer { get; private set; }

        public MongoDatabase Database { get; private set; }
        public MongoCollection<DbPlayerCharactersObject> PlayerCharacters { get; private set; }
        public MongoCollection<CharacterNotifications> Notifications { get; private set; }
        public MongoCollection<ChatMessage> Chat { get; private set; }
        public MongoCollection<Guild> Guilds { get; set; }
        public MongoCollection<PlayerStore> Stores { get; set; } 
        public MongoCollection<AuctionItem> Auction { get; set; }
        public MongoCollection<RaceCommand> RaceCommands { get; set; }
        public MongoCollection<CommanderCandidate> CommanderCandidates { get; set; }
        public MongoCollection<CommanderElector> CommanderElectors { get; set; }
        public MongoCollection<CommanderElectionInfo> ElectionInfo { get; set; }
        public MongoCollection<RaceStats> raceStats { get; set; }
        public MongoCollection<PlayerFriends> friends { get; set; }
        public MongoCollection<CharacterInfo> characters { get; private set; }



        //mails collections
        public MongoCollection<MailBox> Mails { get; private set; }

        public void Setup(string connectionString, string databaseName, string userLoginsCollectionName) {
            this.DbClient = new MongoClient(connectionString);
            this.DbServer = this.DbClient.GetServer();
            this.Database = this.DbServer.GetDatabase(databaseName);
            this.PlayerCharacters = this.Database.GetCollection<DbPlayerCharactersObject>(userLoginsCollectionName);
            this.Mails = Database.GetCollection<MailBox>("mails");
            this.Chat = Database.GetCollection<ChatMessage>("chat");

            Notifications = Database.GetCollection<CharacterNotifications>("notifications");
            Guilds = Database.GetCollection<Guild>("guilds");
            Stores = Database.GetCollection<PlayerStore>("stores");
            Auction = Database.GetCollection<AuctionItem>("auction");
            RaceCommands = Database.GetCollection<RaceCommand>("race_commands");
            CommanderCandidates = Database.GetCollection<CommanderCandidate>("commander_candidates");
            CommanderElectors = Database.GetCollection<CommanderElector>("commander_electors");
            ElectionInfo = Database.GetCollection<CommanderElectionInfo>("election_info");
            raceStats = Database.GetCollection<RaceStats>("race_stats");
            friends = Database.GetCollection<PlayerFriends>("friends");
            characters = Database.GetCollection<CharacterInfo>("character_info");
        }

        public bool ExistsPlayer(string gameRefId) {
            var query = Query<DbPlayerCharactersObject>.EQ(ch => ch.GameRefId, gameRefId);
            return this.PlayerCharacters.Count(query) > 0;
        }

        public DbPlayerCharactersObject Get(string gameRefId) {
            var query = Query<DbPlayerCharactersObject>.EQ(ch => ch.GameRefId, gameRefId);
            var result = this.PlayerCharacters.FindOne(query);
            return result;
        }

        public DbPlayerCharactersObject GetByLogin(string login) {
            var query = Query<DbPlayerCharactersObject>.EQ(player => player.Login, login);
            return PlayerCharacters.FindOne(query);
        }

        public void Save(DbPlayerCharactersObject document) {
            var result = this.PlayerCharacters.Save(document);
        }
    }
}
