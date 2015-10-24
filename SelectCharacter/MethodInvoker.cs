using System;
using System.Collections;
using ServerClientCommon;
using ExitGames.Logging;
using System.Collections.Generic;
using SelectCharacter.Chat;
using Common;
using SelectCharacter.Group;
using NebulaCommon.ServerToServer.Events;

namespace SelectCharacter {
    public class MethodInvoker {

        private SelectCharacterClientPeer peer { get; set; }


        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public SelectCharacterApplication application { get; private set; }

        public MethodInvoker(SelectCharacterApplication inApplication, SelectCharacterClientPeer inPeer) {
            application = inApplication;
            peer = inPeer;
        }


        public object SetYesNoNotification() {

            var notification = peer.application.Notifications.Create( Guid.NewGuid().ToString(),
                string.Format("test notification sended at {0}", DateTime.UtcNow.ToString()),
                new Hashtable(), 
                NotficationRespondAction.YesDelete, 
                NotificationSourceServiceType.Server, NotificationSubType.Unknown);

            log.Info("set notification called");

            peer.application.Notifications.SetNotificationToCharacter(peer.characterId, notification);
            return 0;
        }

        public object InviteToGroup(string sourceGameRefID, string sourceCharacterID, string sourceLogin, string targetCharacterID ) {

            bool isGroupMember = application.Groups.IsSomeGroupMember(sourceCharacterID);

            Hashtable data = new Hashtable {
                { (int)SPC.Action, (isGroupMember) ? (int)GroupAction.AddToGroup : (int)GroupAction.CreateGroup },
                { (int)SPC.GameRefId, sourceGameRefID },
                { (int)SPC.CharacterId, sourceCharacterID },
                { (int)SPC.Login, sourceLogin }
            };
            
            if(!application.Groups.AllowInviteToGroup(sourceCharacterID)) {
                return (int)ReturnCode.Fatal;
            }


            string uniqueID = sourceGameRefID + sourceCharacterID + sourceLogin + targetCharacterID + NotificationSourceServiceType.Group.ToString() + NotificationSubType.InviteToGroup.ToString();
            var notification = application.Notifications.Create(uniqueID, "invite_notification",
                data, NotficationRespondAction.YesDelete, NotificationSourceServiceType.Group, NotificationSubType.InviteToGroup);
            application.Notifications.SetNotificationToCharacter(targetCharacterID, notification);
            return (int)ReturnCode.Ok;
        }

        public object RequestToGroup(string groupID, string sourceGameRefID, string sourceCharacterID, string sourceLogin  ) {
            Hashtable data = new Hashtable {
                { (int)SPC.Action, (int)GroupAction.RequestToGroup },
                { (int)SPC.GameRefId, sourceGameRefID },
                { (int)SPC.CharacterId, sourceCharacterID },
                { (int)SPC.Login, sourceLogin }
            };

            string uniqueID = groupID + sourceCharacterID + sourceGameRefID + sourceLogin + NotificationSourceServiceType.Group.ToString() + NotificationSubType.Unknown.ToString();
            var notification = application.Notifications.Create(uniqueID, "s_note_request_group", data, NotficationRespondAction.YesDelete, 
                NotificationSourceServiceType.Group, NotificationSubType.RequestToGroup);

            NebulaCommon.Group.GroupMember leader;
            if(!application.Groups.TryGetLeader(groupID, out leader)) {
                return (int)ReturnCode.Fatal;
            }

            application.Notifications.SetNotificationToCharacter(leader.characterID, notification);
            return (int)ReturnCode.Ok;
        }

        /*RPC find opened groups for race*/
        public Hashtable FindGroups(byte race) {
            return application.Groups.FindGroups(race);
        }

        public object RequestToFoundedGroup(string groupID, string sourceGameRefID, string sourceCharacterID, string sourceLogin) {
            return RequestToGroup(groupID, sourceGameRefID, sourceCharacterID, sourceLogin);
        }

        public object SetGroupOpened(string whoCharacterID, string groupID, bool opened) {
            if (application.Groups.SetGroupOpened(whoCharacterID, groupID, opened)) {
                return (int)ReturnCode.Ok;
            }
            return (int)ReturnCode.Fatal;
        }

        public object SetGroupLeader(string groupID, string currentLeaderCharacterID, string newLeaderCharacterID ) {
            if(application.Groups.IsLeader(groupID, currentLeaderCharacterID)) {
                return application.Groups.SetLeader(groupID, newLeaderCharacterID);
            }
            return false;
        }

        public object KickGroupMember(string groupID, string currentLeaderCharacterID, string kickedCharacterID ) {
            return application.Groups.RemoveMember(groupID, currentLeaderCharacterID, kickedCharacterID);
        }

        /// <summary>
        /// Exit character from group
        /// </summary>
        /// <param name="characterID">Character ID</param>
        /// <param name="groupID">Group from which character want exit</param>
        /// <returns></returns>
        public object ExitGroup( string groupID, string characterID ) {
            bool result = application.Groups.ExitGroup(groupID, characterID);
            return result;
        }

        public object FindGuilds(int race) {
            return application.Guilds.GetGuilds(race);
        }

        public object ChangeGuildStatus(string guildID, bool opened) {
            return application.Guilds.ChangeGuildStatus(guildID, opened);
        }

        public object KickGuildMember(string guildID, string moderCharacterID,  string memberLogin, string memberCharacterID) {
            return application.Guilds.KickMember(moderCharacterID, memberLogin, memberCharacterID, guildID); //RemoveMember(memberLogin, memberCharacterID, guildID);
        }

        public bool RequestToGuild(string login, string characterID, string guildID ) {
            return application.Guilds.RequestToGuild(login, characterID, guildID);
        }

        public object SendChatMessage( 
            string messageID, 
            string sourceLogin, 
            string sourceCharacterID,
            int chatGroup, 
            string message, 
            string targetLogin, 
            string targetCharacterID, 
            Hashtable linkHash ) {

            object[] links = (object[])linkHash[(int)SPC.Data];

            List<ChatLinkedObject> linkedObjects = new List<ChatLinkedObject>();
            if(links != null) {
                foreach(object link in links ) {
                    ChatLinkedObject linkedObject = new ChatLinkedObject();
                    linkedObject.ParseInfo(link as Hashtable);
                    linkedObjects.Add(linkedObject);
                }
            }

            log.InfoFormat("Called send chat message = {0}, {1}, {2}, {3}, {4}, {5}, {6}",
                (ChatGroup)chatGroup, message, messageID, sourceCharacterID, sourceLogin, targetCharacterID, targetLogin);

            ChatMessage m = new ChatMessage {
                chatGroup = chatGroup,
                links = linkedObjects,
                message = message,
                messageID = messageID,
                sourceCharacterID = sourceCharacterID,
                sourceLogin = sourceLogin,
                targetCharacterID = targetCharacterID,
                targetLogin = targetLogin
            };

            application.Chat.SendMessage(m);
            return (int)ReturnCode.Ok;
        }

        public object SellToNPC(string login,  string gameRefID, string characterID, int count, byte inventoryType, string itemID, string targetServer) {
            //application.Stores.RequestItemFromInventory(login, characterID, count, gameRefID, (InventoryType)inventoryType, itemID, PostTransactionAction.SellItemToNPC);
            application.Stores.SellItemToNPC(login, gameRefID, characterID, count, (InventoryType)inventoryType, itemID, targetServer);
            return 0;
        }

        public Hashtable GetConsumableItems() {
            return application.consumableItems.GetInfo();
        }

        public bool BuyConsumableItem(string login, string gameRefID, string characterID, int race, string consumableItemID, string targetServer) {
            return application.Stores.BuyConsumableItem(login, gameRefID, characterID, race, consumableItemID, targetServer);
        }

        public object PutToAuction(string login, string gameRefID, string characterID, int count, byte inventoryType, string itemID, int price, string targetServer) {
            bool success = application.Stores.PutItemToAuction(login, gameRefID, characterID, count, (InventoryType)inventoryType, itemID, price, targetServer);
            return success;
        }

        public Hashtable GetCurrentAuctionPage(string characterID, bool reset, Hashtable filters ) {
            return application.Auction.GetCurrentItems(characterID, reset, filters);
        }

        public Hashtable GetNextAuctionPage(string characterID ) {
            return application.Auction.GetNextItems(characterID);
        }

        public Hashtable GetPrevAuctionPage(string characterID) {
            return application.Auction.GetPrevItems(characterID);
        }

        public Hashtable GetPrices() {
            return application.itemPriceCollection.GetInfo();
        }

        public Hashtable GetRaceCommands() {
            return application.RaceCommands.GetInfo();
        }

        public bool MakeAdmiral(int race, string sourceLogin, string sourceGameRef, string sourceCharacter,
            string targetLogin, string targetGameRef, string targetCharacter) {
            return application.RaceCommands.MakeAdmiral(race, 
                sourceLogin, sourceGameRef, sourceCharacter,
                targetLogin, targetGameRef, targetCharacter);
        }

        public Hashtable GetRaceStats() {
            return application.raceStats.GetInfo();
        }


        //Election  related methods=================================================

            /// <summary>
            /// Need call periodically to check election status
            /// </summary>
            /// <returns></returns>
        public Hashtable GetElectionInfo() {
            return application.Election.GetElectionInfo();
        }

        /// <summary>
        /// Return candidates for specified race. Need only call if election info return registration started
        /// </summary>
        /// <returns></returns>
        public Hashtable GetCandidates(int race) {
            if(application.Election.registrationStarted || application.Election.votingStarted) {
                return application.Election.GetCandidates(race);
            }
            return new Hashtable();
        }

        /// <summary>
        /// Return voted or not specified character. If not voting not started return always false
        /// </summary>
        /// <param name="characterID"></param>
        /// <returns></returns>
        public bool CharacterAlreadyVoted(string characterID ) {
            if(application.Election.votingStarted) {
                return application.Election.ExistElector(characterID);
            }
            return false;
        }

        /// <summary>
        /// If registration started check the character is candidate, if no registration always return false
        /// </summary>
        /// <param name="characterID"></param>
        /// <returns></returns>
        public bool CharacterAlreadyCandidate(string characterID ) {
            if(application.Election.registrationStarted) {
                return application.Election.ExistCandidate(characterID);
            }
            return false;
        }

        /// <summary>
        /// Add character as candidate to voting
        /// </summary>
        /// <param name="login"></param>
        /// <param name="gameRefID"></param>
        /// <param name="characterID"></param>
        /// <returns></returns>
        public bool AddAsCandidate(string login, string gameRefID, string characterID ) {
            if(application.Election.registrationStarted) {
                return application.Election.AddCandidate(login, gameRefID, characterID);
            }
            return false;
        }

        /// <summary>
        /// If voting started try to vote for candidate character, in other cases return false
        /// </summary>
        public bool VoteForCandidate(string login, string gameRefID, string characterID, string candidateCharacterID ) {
            if(application.Election.votingStarted) {
                return application.Election.VoteForCandidate(login, gameRefID, characterID, candidateCharacterID);
            }
            return false;
        }

        //ELECTION RPCS end==================================


        //Friends RPC Methods-----------------------------
        public Hashtable GetFriends(string gameRefID, string login) {
            return application.friends.GetFriendsInfo(gameRefID, login);
        }

        public bool InviteFriend(string fromGameRefID, string fromLogin, string toGameRefID, string toLogin) {
            return application.friends.RequestFriend(fromGameRefID, fromLogin, toGameRefID, toLogin);
        }

        public bool RemoveFriend(string sourceGameRefID, string sourceLogin, string targetGameRefID, string targetLogin) {
            application.friends.RemoveFriend(sourceGameRefID, sourceLogin, targetGameRefID, targetLogin);
            return true;
        }
        //Friends RPC Methods End----------------------

        public bool ResetNewMailMessages(string gameRefID) {
            return application.Mail.ResetNewMessageCount(gameRefID);
        }

        public bool RemoveBankItem(string item, int count) {
            return peer.RemoveBankItem(item, count);
        }

        public bool BuyBankMaxSlots() {
            return peer.BuyMaxSlots();
        } 
    }
}
