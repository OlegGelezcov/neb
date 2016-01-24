using MongoDB.Driver;
using MongoDB.Driver.Builders;
using SelectCharacter.Auction;
using SelectCharacter.Bank;
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
        public MongoCollection<BankSave> banks { get; private set; }
        public MongoCollection<DatabaseCharacterName> characterNames { get; private set; }



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
            banks = Database.GetCollection<BankSave>("banks");
            characterNames = Database.GetCollection<DatabaseCharacterName>("character_names");
        }

        public bool ExistsCharacterName(string characterName ) {
            var query = Query<DatabaseCharacterName>.EQ(ch => ch.characterName, characterName);
            return characterNames.Count(query) > 0;
        }

        public DatabaseCharacterName GetCharacterName(string characterName ) {
            var query = Query<DatabaseCharacterName>.EQ(ch => ch.characterName, characterName);
            return characterNames.FindOne(query);
        }

        public DatabaseCharacterName GetCharacterNameByCharacterId(string characterId) {
            var query = Query<DatabaseCharacterName>.EQ(ch => ch.characterId, characterId);
            return characterNames.FindOne(query);
        }

        public void WriteCharacterName(string gameRef, string characterId, string characterName ) {
            var existing = GetCharacterNameByCharacterId(characterId);
            if(existing != null ) {
                existing.characterName = characterName;
                characterNames.Save(existing);
            } else {
                characterNames.Save(new DatabaseCharacterName { gameRef = gameRef, characterName = characterName, characterId = characterId });
            }
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

        public DbPlayerCharactersObject GetByLogin(string inlogin) {
            string login = inlogin.ToLower();

            var query = Query<DbPlayerCharactersObject>.EQ(player => player.Login, login);
            return PlayerCharacters.FindOne(query);
        }

        public void Save(DbPlayerCharactersObject document) {
            var result = this.PlayerCharacters.Save(document);
        }

        public BankSave LoadBank(string inlogin ) {
            string login = inlogin.ToLower();

            var query = Query<BankSave>.EQ(b => b.login, login);
            var bank = banks.FindOne(query);
            if(bank != null ) {
                return bank;
            } else {
                Bank.Bank bankObj = new SelectCharacter.Bank.Bank();
                BankSave save = new BankSave {
                    bankInfo = bankObj.GetInfo(),
                    login = login
                };
                banks.Save(save);
                return save;
            }
        }

        public void SaveBank(BankSave save) {
            banks.Save(save);
        }
    }
}
