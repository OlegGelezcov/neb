using Common;
using ExitGames.Logging;
using MongoDB.Driver.Builders;
using ServerClientCommon;
using Space.Game.Resources;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectCharacter.Commander {
    public class CommanderElection {

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        private const int LEVEL_FOR_CANDIDATE = 15;
        private const int CREDITS_FOR_CANDIDATE = 500;

        private CommanderElectionInfo electionInfo { get; set; }
        private SelectCharacterApplication application { get; set; }
        private readonly object syncRoot = new object();
        private readonly CandidateCache mCache = new CandidateCache();

        private const int TO_START_REGISTRATION_SECONDS = 950400; //this 11 days
        private const int TO_START_VOTING_SECONDS = 1209600;    //this 14 days
        private const int TO_END_VOTING_SECONDS = 1296000;      //this 15 days

        public CommanderElection(SelectCharacterApplication application) {
            this.application = application;
            //load election info at start
            LoadElectionInfo();
        }

        private void LoadElectionInfo() {
            //get election info from database
            electionInfo = application.DB.ElectionInfo.FindOne();

            //if election info not founded create new and save to DB
            if(electionInfo == null ) {
                electionInfo = new CommanderElectionInfo { startTime = 0, endTime = 0, started = false };
                application.DB.ElectionInfo.Save(electionInfo);
            }
        }



        public void Update() {
            bool electionStartedBefore = electionInfo.started;
            bool registrationStartedBefore = electionInfo.registrationStarted;

            electionInfo.UpdateState();

            if(electionStartedBefore && (!electionInfo.started)) {
                RestartVotingPertiod();
            }

            if(!electionStartedBefore && electionInfo.started) {
                log.InfoFormat("voting started! [green]");
            }

            if((!registrationStartedBefore) && electionInfo.registrationStarted) {
                ClearOldCommanders();
                log.InfoFormat("registering started! [green]");
            }

            if(!( electionInfo.started || electionInfo.registrationStarted) ) {
                if (CommonUtils.SecondsFrom1970() > electionInfo.endTime) {
                    RestartVotingPertiod();
                }
            }
        }

        private void RestartVotingPertiod() {
            log.InfoFormat("restart voting called");
            //send new commander and send notification and log message to users
            ComputeNewCommander();
            //setting new voting date
            SetNextVoteDate();

            //clear database from previous voting
            ClearOldVotingData();
        }

        private void ComputeNewCommander() {
            ComputeCommander((int)(byte)Race.Humans);
            ComputeCommander((int)(byte)Race.Borguzands);
            ComputeCommander((int)(byte)Race.Criptizoids);
        }

        private void ComputeCommander(int race) {
            var candidates = application.DB.CommanderCandidates.Find(Query<CommanderCandidate>.EQ(c => c.race, race)).ToArray();
            int maxVoices = -1;
            CommanderCandidate maxVoicesCandidate = null;
            for(int i = 0; i < candidates.Length; i++) {
                if(candidates[i].voices > maxVoices ) {
                    maxVoices = candidates[i].voices;
                    maxVoicesCandidate = candidates[i];
                }
            }

            if(maxVoicesCandidate != null ) {
                log.InfoFormat("set commander from voting results = {0}", maxVoicesCandidate.characterID);
                //change race status in race commands service
                application.RaceCommands.SetRaceStatus(race, RaceStatus.Commander, maxVoicesCandidate.login, maxVoicesCandidate.gameRefID, maxVoicesCandidate.characterID);

                //change race status at player character
                application.Players.SetRaceStatus(maxVoicesCandidate.gameRefID, maxVoicesCandidate.characterID, (int)RaceStatus.Commander);
                log.InfoFormat("set race = {0} commander: {1} [green]", (Race)(byte)race, maxVoicesCandidate.login);

                //change event to clients about new commander
                application.Clients.SendNewCommanderElected(race, maxVoicesCandidate.login);

                //send notification to commander with greatings

                Hashtable notificationData = new Hashtable {
                    { (int)SPC.Race, race }
                };
                var notification = application.Notifications.Create(Guid.NewGuid().ToString(),
                    "s_note_new_commander",
                    notificationData, ServerClientCommon.NotficationRespondAction.Delete, ServerClientCommon.NotificationSourceServiceType.Election, 
                    ServerClientCommon.NotificationSubType.Unknown);
                application.Notifications.SetNotificationToCharacter(maxVoicesCandidate.characterID, notification);
            }
        }

        private void ClearOldVotingData() {
            log.InfoFormat("clear old voting data [green]");
            application.DB.CommanderCandidates.Drop();
            application.DB.CommanderElectors.Drop();
            mCache.Clear();
        }

        private void ClearOldCommanders() {
            log.InfoFormat("clear old commanders [green]");
            application.raceStats.Clear();
            application.RaceCommands.Clear();
        }



        private void SetNextVoteDate() {
            var regDate = DateTime.UtcNow.AddSeconds(TO_START_REGISTRATION_SECONDS); //AddDays(11);
            var startDate = DateTime.UtcNow.AddSeconds(TO_START_VOTING_SECONDS); //AddDays(14);
            var endDate = DateTime.UtcNow.AddSeconds(TO_END_VOTING_SECONDS); //AddDays(15);

            electionInfo.SetElectionTime(CommonUtils.SecondsFrom1970(regDate), CommonUtils.SecondsFrom1970(startDate), CommonUtils.SecondsFrom1970(endDate));
            application.DB.ElectionInfo.Save(electionInfo);
            log.InfoFormat("new vote date [{0}-{1}], now = {2}", electionInfo.startTime, electionInfo.endTime, CommonUtils.SecondsFrom1970());
        }

        public bool AddCandidate(string login, string gameRefID, string characterID) {

            if (!electionInfo.registrationStarted) {
                log.Info("error registration not allowed");
                return false;
            }
            if(ExistCandidate(characterID)) {
                log.Info("error candidate already exists");
                return false;
            }

            var player = application.Players.GetExistingPlayer(gameRefID);
            if(player == null || player.Data == null) {
                log.Info("AddCandidate error: player not found");
                return false;
            }

            var character = player.Data.GetCharacter(characterID);
            if(character == null ) {
                log.Info("AddCandidate error: character not found");
                return false;
            }

            int level = application.leveling.LevelForExp(character.Exp);
            if(level < LEVEL_FOR_CANDIDATE) {
                log.InfoFormat("AddCandidate error - level is low = {0}", level);
                return false;
            }

            var guild = application.Guilds.GetGuild(characterID);
            if(guild == null ) {
                log.InfoFormat("AddCandidate error - guild for candidate not founded");
                return false;
            }

            var store = application.Stores.GetOrCreatePlayerStore(login, gameRefID, characterID);

            if(store == null ) {
                log.InfoFormat("AddCandidate error - store for candidate not founded");
                return false;
            }

            if(store.credits < CREDITS_FOR_CANDIDATE ) {
                log.InfoFormat("AddCandidate error - credits not enough");
                return false;
            }

            //remove 500 gold
            store.RemoveCredits(CREDITS_FOR_CANDIDATE);

            CommanderCandidate newCandidate = new CommanderCandidate { characterID = characterID, gameRefID = gameRefID, login = login, race = character.Race, voices = 0, guildName = guild.name };
            //application.DB.CommanderCandidates.Save(newCandidate);
            AddCandidate(newCandidate);

            log.InfoFormat("candidate registration successfull");
            return true;
        }

        private void AddCandidate(CommanderCandidate candidate) {           
            application.DB.CommanderCandidates.Save(candidate);
            mCache.TryAddCandidate(candidate);
        }

        public bool registrationStarted {
            get {
                return electionInfo.registrationStarted;
            }
        }

        public bool votingStarted {
            get {
                return electionInfo.started;
            }
        }

        public Hashtable GetCandidates(int race) {
            
            try {
                Hashtable candidatesHash = new Hashtable();

                foreach(var pCandidate in mCache.mCandidates) {
                    if (pCandidate.Value.race == race) {
                        candidatesHash.Add(pCandidate.Key, pCandidate.Value.GetInfo());
                    }
                }

                //var candidates = application.DB.CommanderCandidates.Find(Query<CommanderCandidate>.EQ(c => c.race, race)).ToArray();


                //for (int i = 0; i < candidates.Length; i++) {
                //    candidatesHash.Add(candidates[i].characterID, candidates[i].GetInfo());
                //}
                log.InfoFormat("return candidates count = {0} [green]", candidatesHash.Count);
                return candidatesHash;
            } catch (Exception ec) {
                log.Error(ec.Message);
                log.Error(ec.StackTrace);
            }
            return new Hashtable();
        }

        public Hashtable GetElectionInfo() {
            return electionInfo.GetInfo();
        }

        public bool VoteForCandidate(string login, string gameRefID, string characterID, string candidateCharacterID) {
            if (!electionInfo.started) {
                log.InfoFormat("VoteForCandidate error: voting not started");
                return false;
            }
            if(ExistElector(characterID)) {
                log.InfoFormat("VoteForCandidate error: you already make election");
                return false;
            }

            var player = application.Players.GetExistingPlayer(gameRefID);
            if (player == null || player.Data == null) {
                log.InfoFormat("VoteForCandidate error: player not found");
                return false;
            }

            var character = player.Data.GetCharacter(characterID);
            if (character == null) {
                log.InfoFormat("VoteForCandidate error: character not found");
                return false;
            }

            var candidate = GetCandidate(candidateCharacterID);
            if(candidate == null ) {
                log.InfoFormat("VoteForCandidate error: candidate not found");
                return false;
            }

            if(character.Race != candidate.race ) {
                log.InfoFormat("VoteForCandidate error: candidate invalid race");
                return false;
            }

            candidate.IncrementVoices();
            application.DB.CommanderCandidates.Save(candidate);
            CommanderElector elector = new CommanderElector {
                characterID = characterID,
                gameRefID = gameRefID,
                candidateCharacterID = candidateCharacterID,
                login = login,
                race = character.Race
            };
            application.DB.CommanderElectors.Save(elector);
            log.Info("you successfully vote");

            return true;
        }


        public bool ExistCandidate(string characterID) {
            CommanderCandidate result;
            if(mCache.TryGetCandidate(characterID, out result)) {
                return true;
            }
            result = application.DB.CommanderCandidates.FindOne(Query<CommanderCandidate>.EQ(cc => cc.characterID, characterID));
            if(result != null ) {
                mCache.TryAddCandidate(result);
                return true;
            }
            return false;
        }

        public bool ExistElector(string characterID ) {
            CommanderElector elector = application.DB.CommanderElectors.FindOne(Query<CommanderElector>.EQ(ce => ce.characterID, characterID));
            return (elector != null);
        }

        private CommanderCandidate GetCandidate(string characterID ) {
            CommanderCandidate result;
            if(mCache.TryGetCandidate(characterID, out result)) {
                return result;
            }
            result = application.DB.CommanderCandidates.FindOne(Query<CommanderCandidate>.EQ(cc => cc.characterID, characterID)); 
            if(result != null ) {
                mCache.TryAddCandidate(result);
            }
            return result;
        }
    }
}
