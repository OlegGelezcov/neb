using Common;
using ExitGames.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using NebulaCommon;
using Photon.SocketServer;
using SelectCharacter.Characters;
using SelectCharacter.Events;
using SelectCharacter.Notifications;
using ServerClientCommon;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectCharacter.Guilds {
    public class GuildService {

        //private const int MAX_FIND_GUILDS_COUNT = 30;
        private const int UPDATE_SEARCH_GUILD_CACHE_INTERVAL = 60;

        private static ILogger log = LogManager.GetCurrentClassLogger();

        private GuildCache mCache;
        private SelectCharacterApplication mApplication;
        private ConcurrentDictionary<int, Hashtable> mSearchCachedGuilds;

        private DateTime mCreatinDate = DateTime.UtcNow;
        private float mLastUpdateSearchResultTime;

        private float time {
            get {
                return (float)(DateTime.UtcNow - mCreatinDate).TotalSeconds;
            }
        }

        public GuildService(SelectCharacterApplication application) {
            mApplication = application;
            mCache = new GuildCache();
            mSearchCachedGuilds = new ConcurrentDictionary<int, Hashtable>();
            mCreatinDate = DateTime.UtcNow;
            mLastUpdateSearchResultTime = time;
        }

        public Guild GetGuild(string ownerCharacterId) {
            DbObjectWrapper<Guild> wrapperGuild = null;
            if(mCache.TryGetGuild(ownerCharacterId, out wrapperGuild)) {
                return wrapperGuild.Data;
            }

            var guild = mApplication.DB.Guilds.FindOne(Query<Guild>.EQ(g => g.ownerCharacterId, ownerCharacterId));
            if(guild != null ) {
                mCache.SetGuild(guild);
            }
            return guild;
        }

        private bool HasGuild(string guildID) {
            return (GetGuild(guildID) != null);
        }

        /// <summary>
        /// Mark guild from cache as modified for saving to database
        /// </summary>
        /// <param name="guildID"></param>
        public void MarkModified(string guildID) {
            mCache.MarkModified(guildID);
        }

        public Guild FindGuild(string guildName) {
            return mApplication.DB.Guilds.FindOne(Query<Guild>.EQ(g => g.name, guildName));
        }

        /// <summary>
        /// Create guild for character, if guild already exists return false and link to existing guild. If guild don't exists,
        /// create new guild, initialize it and return reference to newly created guild
        /// </summary>
        public bool CreateGuild(GuildMember owner, string name, string description, out Guild newGuild) {

            newGuild = GetGuild(owner.characterId);
            if(newGuild != null ) {
                log.Info("Such guild already exists");
                return false;
            }

            var player = mApplication.Players.GetExistingPlayer(owner.gameRefId);
            if(player == null ) {
                log.Info("error creating guild, player don't exists");
                return false;
            }

            var playerCharacter = player.Data.GetCharacter(owner.characterId);
            if(playerCharacter == null ) {
                log.Info("error creating guild, player character don't exists ");
                return false;
            }

            //if character guild setted and exist such guild than return error
            //else this is some error and reset character guild to empty
            if(!string.IsNullOrEmpty(playerCharacter.guildID)) {

                if (HasGuild(playerCharacter.guildID)) {
                    log.Info("player character already in guild");
                    return false;
                } else {
                    log.InfoFormat("reset character guild to (empty), reason: guild from character don't exists");
                    playerCharacter.SetGuild(string.Empty);
                }
            }

            int playerLevel = mApplication.leveling.LevelForExp(playerCharacter.Exp);
            if(playerLevel < 5) {
                log.Info("player level less than 5");
                return false;
            }

            newGuild = new Guild {
                ownerCharacterId = owner.characterId,
                rating = 0,
                members = new Dictionary<string, GuildMember> { { owner.characterId, owner } },
                description = description,
                guildRace = playerCharacter.Race,
                name = name,
                depositedPvpPoints = 0,
                depositedCredits = 0
            };
            mCache.SetGuild(newGuild);
            mApplication.Players.SetGuild(owner.gameRefId, owner.characterId, newGuild.ownerCharacterId);
            mApplication.DB.Guilds.Save(newGuild);

            mApplication.AddAchievmentVariable(owner.gameRefId, "coalition_member", 1);
            //send guild update
            SendGuildUpdateEvent(owner.characterId, newGuild.GetInfo(mApplication));

            return true;
        }

        

        private void SendGuildUpdateEvent(string characterID, Hashtable guildInfo ) {
            SelectCharacterClientPeer peer = null;
            if (mApplication.Clients.TryGetPeerForCharacterId(characterID, out peer)) {
                GuildUpdateEvent updateEvent = new GuildUpdateEvent { Guild = guildInfo };
                EventData eventData = new EventData((byte)SelectCharacterEventCode.GuildUpdate, updateEvent);
                if (false == peer.Disposed) {
                    peer.SendEvent(eventData, new SendParameters());
                }
            }
        }


        public bool AddMember(string guildID, GuildMember newMember) {
            DbObjectWrapper<Guild> guild;
            if(!mCache.TryGetGuild(guildID, out guild)) {
                return false;
            }

            bool success =  guild.Data.AddMember(newMember);
            guild.Changed = true;
            return success;
        }

        public bool KickMember(string moderCharacterID, string login, string characterID, string guildID) {
            var guild = GetGuild(guildID);
            if(guild == null ) {
                return false;
            }

            GuildMember moderMember = null;
            if(!guild.TryGetMember(moderCharacterID, out moderMember)) {
                return false;
            }

            if(!moderMember.IsAddMemberGranted()) {
                return false;
            }

            return RemoveMember(login, characterID, guildID);
        }


        public bool RemoveMember(string login, string characterID, string guildID) {
            if(guildID == characterID) {
                return false;
            }

            DbObjectWrapper<Guild> guild;
            if(!mCache.TryGetGuild(guildID, out guild)) {
                return false;
            }

            bool success = guild.Data.RemoveMember(characterID);
            guild.Changed = true;

            var player = mApplication.Players.GetExistingPlayerByLogin(login);
            if(player == null ) {
                return false;
            }

            player.Data.SetGuild(characterID, string.Empty);

            var character = player.Data.GetCharacter(characterID);

            if(character != null ) {
                guild.Data.AddTransaction(CoalitionTransaction.MakeTransaction(CoalitionTransactionType.member_removed, string.Empty, string.Empty, character.Name, characterID));
            }
            SendGuildUpdateEvent(characterID, new Hashtable());
            SendGuildUpdateEvent(guildID, guild.Data.GetInfo(mApplication));

            return success;
        }

        public bool DeleteGuild(string sourceCharacterID ) {
            DbObjectWrapper<Guild> guild;
            if (!mCache.TryGetGuild(sourceCharacterID, out guild)) {
                log.Info("DeleteGuild: guild not found");
                return false;
            }

            var members = guild.Data.memberList;

            GuildMember owner = null;
            if( !guild.Data.TryGetMember(sourceCharacterID, out owner) ) {
                log.Info("DeleteGuild: owner not found");
                return false;
            }

            foreach(var member in members ) {
                RemoveMember(member.login, member.characterId, guild.Data.ownerCharacterId);
            }

            var player = mApplication.Players.GetExistingPlayerByLogin(owner.login);
            if (player == null) {
                log.Info("DeleteGuild: owner player not found");
                return false;
            }


            //player.Data.SetGuild(sourceCharacterID, string.Empty);
            mApplication.Players.SetGuild(sourceCharacterID, string.Empty);
            SendGuildUpdateEvent(sourceCharacterID, new Hashtable());
           
            bool success =  mCache.TryRemoveGuild(guild.Data.ownerCharacterId, mApplication.DB.Guilds);
            return success;
        }

        public void SaveModified(MongoCollection<Guild> collection ) {
            //save modified guilds
            mCache.SaveModified(collection);

            //update serach cache for guilds
            UpdateSearchCache();
        }


        public bool HandleNotification(string characterID, Notification notification) {
            Hashtable notificationData = notification.data;
            GuildAction action = (GuildAction)(int)notificationData[(int)SPC.Action];

            switch(action) {
                case GuildAction.AddMember:
                    return HandleAddMemberNotification(characterID, notificationData);
                case GuildAction.RequestToGuild:
                    return HandleRequestToGuildNotification(characterID, notificationData);
                default:
                    log.InfoFormat("not handled notification {0}", action);
                    return false;
            }
        }

        /*
        Notification data must be { {SPC.Action, (GuildAction)(int) }, {SPC.Guild, (string)guildID } }
        */
        private bool HandleAddMemberNotification(string memberCharacterID, Hashtable notificationData ) {

            string guildID = (string)notificationData[(int)SPC.Guild];
            
            SelectCharacterClientPeer peer;
            if( !mApplication.Clients.TryGetPeerForCharacterId(memberCharacterID, out peer) ) {
                log.InfoFormat("character peer not found");
                return false;
            }

            var player = mApplication.Players.GetExistingPlayer(peer.id);

            if( player == null ) {
                log.Info("existing player not founded");
                return false;
            }

            var character = player.Data.GetCharacter(peer.characterId);
            if(character == null ) {
                log.Info("character not founded");
                return false;
            }

            if(!string.IsNullOrEmpty(character.guildID)) {
                log.Info("character already in guild");
                return false;
            }

            GuildMember member = new GuildMember {
                characterId = character.CharacterId,
                gameRefId = player.Data.GameRefId,
                guildStatus = (int)GuildMemberStatus.Member,
                exp = character.Exp,
                login = player.Data.Login.ToLower(),
                characterName = character.Name,
                characterIcon = character.characterIcon
            };

            if(!AddMember(guildID, member)) {
                log.Info("some error adding member to guild");
                return false;
            }


            mApplication.Players.SetGuild(player.Data.GameRefId, memberCharacterID, guildID);

            var guild = GetGuild(guildID);
            if (guild != null) {
                guild.AddTransaction(CoalitionTransaction.MakeTransaction(CoalitionTransactionType.member_added, string.Empty, string.Empty, member.characterName, member.characterId));
                SendGuildUpdateEvent(memberCharacterID, guild.GetInfo(mApplication));
            }

            
            mApplication.AddAchievmentVariable(member.gameRefId, "coalition_member", 1);
            return true;
        }

        private bool HandleRequestToGuildNotification(string handlerCharacterID, Hashtable notificationData ) {
            string requestedGuildID = notificationData.GetValue<string>((int)SPC.Guild, string.Empty);
            string targetCharacterID = notificationData.GetValue<string>((int)SPC.CharacterId, string.Empty);
            string targetLogin = notificationData.GetValue<string>((int)SPC.Login, string.Empty);
            if(string.IsNullOrEmpty(requestedGuildID) || string.IsNullOrEmpty(targetCharacterID) || string.IsNullOrEmpty(targetLogin)) {
                log.InfoFormat("HandleRequestToGuildNotification: notfication data invalid [green]");
                return false;
            }

            var targetGuild = GetGuild(requestedGuildID);
            if(targetGuild == null ) {
                log.InfoFormat("HandleRequestToGuildNotification: requested guild is null [green]");
                return false;
            }

            if(targetGuild.closed ) {
                log.InfoFormat("HandleRequestToGuildNotification: guild closed [green]");
                return false;
            }

            GuildMember handlerMember;
            if(!targetGuild.TryGetMember(handlerCharacterID, out handlerMember)) {
                log.InfoFormat("HandleRequestToGuildNotification: privileged user with character id = {0} not founded at guild [green]", handlerCharacterID);
                return false;
            }

            if(!handlerMember.IsAddMemberGranted()) {
                log.InfoFormat("HandleRequestToGuildNotification: adding users not granted [green]");
                return false;
            }

            var player = mApplication.Players.GetExistingPlayerByLogin(targetLogin);
            if(player == null ) {
                log.InfoFormat("HandleRequestToGuildNotification: player not founded [green]");
                return false;
            }

            var playerCharacter = player.Data.GetCharacter(targetCharacterID);
            if(playerCharacter == null ) {
                log.InfoFormat("HandleRequestToGuildNotification: target player character not founded [green]");
                return false;
            }

            if(playerCharacter.HasGuild()) {
                log.InfoFormat("HandleRequestToGuildNotification: target player character already at another guild [green]");
                return false;
            }

            GuildMember member = new GuildMember {
                characterId = targetCharacterID,
                gameRefId = player.Data.GameRefId,
                guildStatus = (int)GuildMemberStatus.Member,
                exp = playerCharacter.Exp,
                login = targetLogin,
                characterName = playerCharacter.Name,
                characterIcon = playerCharacter.characterIcon
            };

            if (!AddMember(requestedGuildID, member)) {
                log.Info("HandleRequestToGuildNotification: some error adding member to guild [green]");
                return false;
            }



            mApplication.Players.SetGuild(player.Data.GameRefId, targetCharacterID, requestedGuildID);

            SendGuildUpdateEvent(handlerCharacterID, targetGuild.GetInfo(mApplication));
            SendGuildUpdateEvent(targetCharacterID, targetGuild.GetInfo(mApplication));

            mApplication.AddAchievmentVariable(member.gameRefId, "coalition_member", 1);
            mApplication.AddAchievmentVariable(handlerMember.gameRefId, "coalition_member_accepted", 1);

            return true;
        }


        public bool SendInviteToGuildNotification(string sourceLogin, string sourceCharacterID, string targetLogin, string targetCharacterID, string guildID ) {
            var sourcePlayer = mApplication.Players.GetExistingPlayerByLogin(sourceLogin);
            var targetPlayer = mApplication.Players.GetExistingPlayerByLogin(targetLogin);

            if(sourcePlayer == null ) {
                log.Info("don't exists source player");
                return false;
            }

            if(targetPlayer == null ) {
                log.Info("don't exists target player");
                return false;
            }

            var sourceCharacter = sourcePlayer.Data.GetCharacter(sourceCharacterID);
            var targetCharacter = targetPlayer.Data.GetCharacter(targetCharacterID);
            if(sourceCharacter == null ) {
                log.Info("source character not found");
                return false;
            }
            if(targetCharacter == null ) {
                log.Info("target character not found");
                return false;
            }

            if(targetCharacter.HasGuild()) {
                log.Info("target character already in some guild");
                return false;
            }

            Guild targetGuild = GetGuild(guildID);
            if(targetGuild == null ) {
                log.Info("guild not found");
                return false;
            }
            GuildMember sourceMember = null;
            if(!targetGuild.TryGetMember(sourceCharacterID, out sourceMember)) {
                log.Info("source member not found");
                return false;
            }

            if(!sourceMember.IsAddMemberGranted()) {
                log.Info("not granted add members");
                return false;
            }

            //string text = string.Format("{0} invited you to group {1}. Do you want accept to group?", sourceLogin, targetGuild.name);
            Hashtable notificationData = new Hashtable {
                { (int)SPC.Action, (int)GuildAction.AddMember },
                { (int)SPC.Guild, guildID },
                { (int)SPC.SourceLogin, sourceLogin },
                { (int)SPC.Name, targetGuild.name }
            };

            string uniqueID = sourceCharacterID + sourceLogin + targetCharacterID + targetLogin + guildID + NotificationSourceServiceType.Guild.ToString() + NotificationSubType.InviteToGuild.ToString();
            var notification = mApplication.Notifications.Create(uniqueID, "s_note_invite_guild", notificationData, NotficationRespondAction.YesDelete, 
                NotificationSourceServiceType.Guild, NotificationSubType.InviteToGuild);
            mApplication.Notifications.SetNotificationToCharacter(targetCharacterID, notification);
            return true;
        }

        public bool RequestToGuild(string login, string characterID, string guildID, out RPCErrorCode code) {
            var player = mApplication.Players.GetExistingPlayerByLogin(login);
            if(player == null ) {
                log.InfoFormat("RequestToGuild: player with login = {0} not exists [green]", login);
                code = RPCErrorCode.PlayerNotFound;
                return false;
            }
            var character = player.Data.GetCharacter(characterID);
            if(character == null) {
                log.InfoFormat("RequestToGuild: player with character = {0} not exists [green]", characterID);
                code = RPCErrorCode.CharacterNotFound;
                return false;
            }

            if(character.HasGuild()) {
                log.InfoFormat("RequestToGuild: character already in guild [green]");
                code = RPCErrorCode.CharacterAlreadyInGuild;
                return false;
            }

            Guild guild = GetGuild(guildID);
            if(guild == null ) {
                log.InfoFormat("RequestToGuild: guild = {0} don't exists [green]");
                code = RPCErrorCode.CoalitionNotFound;
                return false;
            }

            if(guild.closed) {
                log.InfoFormat("RequestToGuild: guild is closed");
                code = RPCErrorCode.CoalitionInClosedState;
                return false;
            }

            //string text = string.Format("Player {0} want join to you guild. Allow this action?", login);
            Hashtable notificationData = new Hashtable {
                { (int)SPC.Action, (int)GuildAction.RequestToGuild },
                { (int)SPC.Guild, guildID },
                { (int)SPC.CharacterId, characterID },
                { (int)SPC.Login, login }
            };

            string uniqueID = login + characterID + guildID + NotificationSourceServiceType.Guild.ToString() + NotificationSubType.RequestToGuild.ToString();
            foreach (var member in guild.GetPrivilegedUsers()) {
                var notification = mApplication.Notifications.Create(uniqueID, "s_note_request_guild", notificationData, NotficationRespondAction.YesDelete, 
                    NotificationSourceServiceType.Guild, NotificationSubType.RequestToGuild);
                mApplication.Notifications.SetNotificationToCharacter(member.characterId, notification);
                log.InfoFormat("Guild request sended to = {0} [green]", member.login);
            }
            code = RPCErrorCode.Ok;
            return true;
        }

        public bool SetGuildDescription(string characterID, string guildID,  string description ) {

            DbObjectWrapper<Guild> guild = null;
            if(!mCache.TryGetGuild(guildID, out guild)) {
                log.Info("SetGuildDescription - guild not found");
                return false;
            }

            GuildMember member = null;
            if(!guild.Data.TryGetMember(characterID, out member)) {
                log.Info("SetGuildDescription - member not found");
                return false;
            }

            if(!member.IsSetGuildDescriptionGranted()) {
                log.Info("SetGuildDescription - set description not allowed");
                return false;
            }

            guild.Data.description = description;
            guild.Changed = true;
            return true;
        }

        public bool ChangeGuildStatus(string guildID, bool opened) {
            DbObjectWrapper<Guild> guild = null;
            if(!mCache.TryGetGuild(guildID, out guild)) {
                log.InfoFormat("ChangeGuildStatus: guild not found [green]");
                return false;
            }
            guild.Data.SetOpened(opened);
            guild.Changed = true;
            SendGuildUpdateEvent(guildID, guild.Data.GetInfo(mApplication));
            log.InfoFormat("ChangeGuildStatus: guils status changed to = {0} [green]", opened);
            return true;
        }

        /// <summary>
        /// Changing status of member by other member
        /// </summary>
        /// <param name="sourceCharacterID">Character ID of member who make changes</param>
        /// <param name="targetCharacterID">Character ID of membe who will be changed</param>
        /// <param name="guildID">Target guild id</param>
        /// <param name="status">New status of member</param>
        /// <returns></returns>
        public bool ChangeGuildMemberStatus(string sourceCharacterID, string targetCharacterID, string guildID, int status ) {

            if(sourceCharacterID == targetCharacterID ) {
                log.Info("ChangeGuildMemberStatus - don't allow change self status");
                return false;
            }

            DbObjectWrapper<Guild> guild = null;
            if(!mCache.TryGetGuild(guildID, out guild)) {
                log.Info("ChangeGuildMemberStatus - guild not found");
                return false;
            }

            GuildMember sourceMember = null;
            if(!guild.Data.TryGetMember(sourceCharacterID, out sourceMember)) {
                log.Info("ChangeGuildMemberStatus - source member not found");
                return false;
            }

            GuildMember targetMember = null;
            if(!guild.Data.TryGetMember(targetCharacterID, out targetMember)) {
                log.Info("ChangeGuildMemberStatus - target member not found");
                return false;
            }

            GuildMemberStatus newStatus = (GuildMemberStatus)status;

            if(newStatus == GuildMemberStatus.Moderator) {
                if(!guild.Data.addingModersAllowed) {
                    log.InfoFormat("ChangeGuildMemberStatus: guild already has maximum count of moderators [green]");
                    return false;
                }
            }

            GuildMemberStatus sourceStatus = (GuildMemberStatus)sourceMember.guildStatus;
            GuildMemberStatus targetStatus = (GuildMemberStatus)targetMember.guildStatus;


            if(sourceStatus == GuildMemberStatus.Moderator) {
                if(targetStatus != GuildMemberStatus.Member) {
                    log.Info("ChangeGuildMemberStatus - moderator allow change only members status");
                    return false;
                }
                if(newStatus != GuildMemberStatus.Moderator) {
                    log.Info("ChangeGuildMemberStatus - moderator allow raise status of member to moderator only");
                    return false;
                }
            } else if(sourceStatus == GuildMemberStatus.Owner) {
                if(targetStatus == GuildMemberStatus.Owner) {
                    log.Info("ChangeGuildMemberStatus  - Don't allow two owners of guild");
                    return false;
                }
                if(newStatus == targetStatus) {
                    log.Info("ChangeGuildMemberStatus - New status same as before status");
                    return false;
                }
            }

            targetMember.guildStatus = status;
            guild.Changed = true;

            if(status == (int)GuildMemberStatus.Moderator) {
                guild.Data.AddTransaction(CoalitionTransaction.MakeTransaction(CoalitionTransactionType.make_officier, string.Empty, string.Empty, targetMember.characterName, targetMember.characterId));
            }

            SendGuildUpdateEvent(sourceMember.characterId, guild.Data.GetInfo(mApplication));
            SendGuildUpdateEvent(targetMember.characterId, guild.Data.GetInfo(mApplication));

            if(newStatus == GuildMemberStatus.Moderator ) {
                mApplication.AddAchievmentVariable(targetMember.gameRefId, "coalition_officier", 1);
            }
            return true;
        }

        public void AddRating(string gID, int points) {
            var guild = GetGuild(gID);
            if(guild != null ) {
                guild.AddRating(points);
                MarkModified(gID);
            }
        }

        public Hashtable GetGuilds(int race) {

            if(mSearchCachedGuilds.ContainsKey(race)) {
                return mSearchCachedGuilds[race];
            } else {

                Hashtable result = ReadRaceGuildFromDB(race);
                mSearchCachedGuilds.TryAdd(race, result);
                return result;
            }
        }

        public void UpdateSearchCache() {


            if (time - mLastUpdateSearchResultTime > UPDATE_SEARCH_GUILD_CACHE_INTERVAL) {
                //log.Info("make update search result");
                mLastUpdateSearchResultTime = time;
                //log.InfoFormat("update serach guild result cache [yellow]");
                var keys = mSearchCachedGuilds.Keys;
                foreach (var race in keys) {
                    log.InfoFormat("update search guild cache for race = {0} [yellow]", race);
                    Hashtable result = ReadRaceGuildFromDB(race);
                    mSearchCachedGuilds[race] = result;
                }
            }
        }

        private Hashtable ReadRaceGuildFromDB(int race) {
            var query = Query<Guild>.EQ(g => g.guildRace, race);
            var cursor = mApplication.DB.Guilds.Find(query);
            var list = cursor.ToList();
            Hashtable result = new Hashtable();
            foreach (var guild in list) {
                result.Add(guild.ownerCharacterId, guild.GetShortInfo());
            }
            return result;
        }

        public ActionResult DepositCredits(string characterId, string guildId, int count) {
            var guildObject = GetGuild(guildId);
            if(guildObject == null ) {
                return new ActionResult(ReturnCode.GuildNotFound);
            }
            GuildMember member;
            if(false == guildObject.TryGetMember(characterId, out member)) {
                return new ActionResult(ReturnCode.GuildMemberNotFound);
            }

            var store = mApplication.Stores.GetOnlyPlayerStore(characterId);
            if(store == null ) {
                return new ActionResult(ReturnCode.PlayerStoreNotFounded);
            }

            if(store.credits < count ) {
                return new ActionResult(ReturnCode.NotEnoughCredits);
            }

            if(false == store.RemoveCredits(count)) {
                return new ActionResult(ReturnCode.CreditsRemoveFromStoreError);
            }

            guildObject.AddCredits(count);
            guildObject.AddTransaction(CoalitionTransaction.MakeTransaction(CoalitionTransactionType.deposit, member.characterName, characterId, count));
            MarkModified(guildId);

            SendGuildUpdateEvent(characterId, guildObject.GetInfo(mApplication));
            return new ActionResult(ReturnCode.Ok, new Hashtable { { (int)SPC.Count, count } });
        }

        public ActionResult WithdrawCredits(string characterId, string guildId, int count) {
            var guildObject = GetGuild(guildId);
            if (guildObject == null) {
                return new ActionResult(ReturnCode.GuildNotFound);
            }
            GuildMember member;
            if (false == guildObject.TryGetMember(characterId, out member)) {
                return new ActionResult(ReturnCode.GuildMemberNotFound);
            }

            if(false == member.IsAddMemberGranted()) {
                return new ActionResult(ReturnCode.GuildPrivilegeNotEnough);
            }

            if(guildObject.depositedCredits < count ) {
                return new ActionResult(ReturnCode.DepositedCreditsDontEnough);
            }

            var store = mApplication.Stores.GetOnlyPlayerStore(characterId);
            if (store == null) {
                return new ActionResult(ReturnCode.PlayerStoreNotFounded);
            }

            if(false == guildObject.RemoveCredits(count)) {
                return new ActionResult(ReturnCode.WithdrawCreditsError);
            }

            guildObject.AddTransaction(CoalitionTransaction.MakeTransaction(CoalitionTransactionType.withdraw, member.characterName, characterId, count));
            MarkModified(guildId);

            store.AddCredits(count);
            SendGuildUpdateEvent(characterId, guildObject.GetInfo(mApplication));
            return new ActionResult(ReturnCode.Ok, new Hashtable { { (int)SPC.Count, count } });
        }

        public ActionResult DepositPvpPoints(string characterId, string guildId, int count) {
            var guildObject = GetGuild(guildId);
            if (guildObject == null) {
                return new ActionResult(ReturnCode.GuildNotFound);
            }
            GuildMember member;
            if (false == guildObject.TryGetMember(characterId, out member)) {
                return new ActionResult(ReturnCode.GuildMemberNotFound);
            }

            var store = mApplication.Stores.GetOnlyPlayerStore(characterId);
            if (store == null) {
                return new ActionResult(ReturnCode.PlayerStoreNotFounded);
            }

            if (store.pvpPoints < count) {
                return new ActionResult(ReturnCode.NotEnoughPvpPoints);
            }

            if (false == store.RemovePvpPoints(count)) {
                return new ActionResult(ReturnCode.PvpPointsRemoveFromStoreError);
            }

            guildObject.AddPvpPoints(count);
            MarkModified(guildId);

            SendGuildUpdateEvent(characterId, guildObject.GetInfo(mApplication));
            return new ActionResult(ReturnCode.Ok, new Hashtable { { (int)SPC.Count, count } });
        }

        public ActionResult WithdrawPvpPoints(string characterId, string guildId, int count ) {
            var guildObject = GetGuild(guildId);
            if (guildObject == null) {
                return new ActionResult(ReturnCode.GuildNotFound);
            }
            GuildMember member;
            if (false == guildObject.TryGetMember(characterId, out member)) {
                return new ActionResult(ReturnCode.GuildMemberNotFound);
            }

            if (false == member.IsAddMemberGranted()) {
                return new ActionResult(ReturnCode.GuildPrivilegeNotEnough);
            }
            if (guildObject.depositedPvpPoints < count) {
                return new ActionResult(ReturnCode.DepositedPvpPointsDontEnough);
            }
            var store = mApplication.Stores.GetOnlyPlayerStore(characterId);
            if (store == null) {
                return new ActionResult(ReturnCode.PlayerStoreNotFounded);
            }
            if(false == guildObject.RemovePvpPoints(count)) {
                return new ActionResult(ReturnCode.WithdrawPvpPointsError);
            }
            MarkModified(guildId);
            store.AddPvpPoints(count);
            SendGuildUpdateEvent(characterId, guildObject.GetInfo(mApplication));
            return new ActionResult(ReturnCode.Ok, new Hashtable { { (int)SPC.Count, count } });
        }

        public ActionResult SetPoster(string characterId, string guildId, string message ) {
            //find guild
            var guildObject = GetGuild(guildId);
            if (guildObject == null) {
                return new ActionResult(ReturnCode.GuildNotFound);
            }

            //find member
            GuildMember member = null;
            if(!guildObject.TryGetMember(characterId, out member)) {
                return new ActionResult(ReturnCode.GuildMemberNotFound);
            }

            //check privilegies
            if(!member.IsAddMemberGranted()) {
                return new ActionResult(ReturnCode.GuildPrivilegeNotEnough);
            }

            guildObject.SetPoster(message);
            guildObject.AddTransaction(CoalitionTransaction.MakeTransaction(CoalitionTransactionType.set_poster, member.characterName, characterId, 0));
            MarkModified(guildId);
            SendGuildUpdateEvent(characterId, guildObject.GetInfo(mApplication));

            return new ActionResult(ReturnCode.Ok, new Hashtable {
                { (int)SPC.DayPoster, message }
            });
        }
    }
}
