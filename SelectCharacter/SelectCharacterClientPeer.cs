using Common;
using ExitGames.Logging;
using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;
using SelectCharacter.Characters;
using SelectCharacter.Mail;
using SelectCharacter.Operations;
using SelectCharacter.Guilds;
using ServerClientCommon;
using NebulaCommon.SelectCharacter;
using SelectCharacter.Chat;
using SelectCharacter.Events;
using System.Collections;
using System.Collections.Generic;
using SelectCharacter.OperationHandlers;

namespace SelectCharacter {
    public class SelectCharacterClientPeer : PeerBase {

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        public SelectCharacterApplication application;
        public MethodInvoker invoker { get; private set; }

        public string id { get; private set; }
        public string characterId { get; private set; }
        public string groupID { get; private set; } = string.Empty;
        private DbPlayerCharacter mCachedCharacter;

        private readonly Dictionary<SelectCharacterOperationCode, BaseOperationHandler> mHandlers;

        public DbPlayerCharacter selectedCharacter {
            get {
                return mCachedCharacter;
            }
        }


        public SelectCharacterClientPeer(InitRequest initRequest, SelectCharacterApplication application)
            : base(initRequest.Protocol, initRequest.PhotonPeer) {
            this.application = application;
            invoker = new MethodInvoker(application, this);
            mHandlers = new Dictionary<SelectCharacterOperationCode, BaseOperationHandler>();

            mHandlers.Add(SelectCharacterOperationCode.RegisterClient,  new RegisterClientOperationHandler(application, this));
            mHandlers.Add(SelectCharacterOperationCode.GetCharacters,   new GetCharactersOperationHandler(application, this));
            mHandlers.Add(SelectCharacterOperationCode.DeleteCharacter, new DeleteCharacterOperationHandler(application, this));
            mHandlers.Add(SelectCharacterOperationCode.CreateCharacter, new CreateCharacterOperationHandler(application, this));
            mHandlers.Add(SelectCharacterOperationCode.SelectCharacter, new SelectCharacterOperationHandler(application, this));
            mHandlers.Add(SelectCharacterOperationCode.GetMails, new GetMailBoxOperationHandler(application, this));
            mHandlers.Add(SelectCharacterOperationCode.WriteMailMessage, new WriteMessageOperationHandler(application, this));
            mHandlers.Add(SelectCharacterOperationCode.DeleteMailMessage, new DeleteMessageOperationHandler(application, this));
            mHandlers.Add(SelectCharacterOperationCode.MoveAttachmentToStation, new MoveAttachmentToStationOperationHandler(application, this));
            mHandlers.Add(SelectCharacterOperationCode.GetNotifications, new GetNotificationsOperationHandler(application, this));
            mHandlers.Add(SelectCharacterOperationCode.HandleNotification, new HandleNotificationOperationHandler(application, this));
            mHandlers.Add(SelectCharacterOperationCode.InvokeMethod, new InvokeMethodOperationHandler(application, this));
            mHandlers.Add(SelectCharacterOperationCode.CreateGuild, new CreateGuildOperationHandler(application, this));
            mHandlers.Add(SelectCharacterOperationCode.GetGuild, new GetGuildOperationHandler(application, this));
            mHandlers.Add(SelectCharacterOperationCode.InviteToGuild, new InviteToGuildOperationHandler(application, this));
            mHandlers.Add(SelectCharacterOperationCode.ExitGuild, new ExitGuildOperationHandler(application, this));
            mHandlers.Add(SelectCharacterOperationCode.DeleteGuild, new DeleteGuildOperationHandler(application, this));
            mHandlers.Add(SelectCharacterOperationCode.SetGuildDescription, new SetGuildDescriptionOperationHandler(application, this));
            mHandlers.Add(SelectCharacterOperationCode.ChangeGuildMemberStatus, new ChangeGuildMemberStatusOperationHandler(application, this));
            mHandlers.Add(SelectCharacterOperationCode.GetPlayerStore, new GetPlayerStoreOperationHandler(application, this));
            mHandlers.Add(SelectCharacterOperationCode.BuyAuctionItem, new BuyAuctionItemOperationHandler(application, this));
            mHandlers.Add(SelectCharacterOperationCode.DeleteAuctionItem, new DeleteAuctionItemOperationHandler(application, this));
            mHandlers.Add(SelectCharacterOperationCode.SetNewPrice, new SetNewPriceOperationHandler(application, this));
        }

        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail) {
            if (log.IsDebugEnabled) {
                log.DebugFormat("LoginClientPeer Disconnect: pid={0}: reason={1}, detail={2}", this.ConnectionId, reasonCode, reasonDetail);
            }

            application.Auction.OnDisconnect(characterId);

            application.Groups.OnClientDisconnect(characterId);

            //save and remove player from players collection
            application.Players.SaveAndRemovePlayerFromCollection(id);

            //save notification and remove from notification cache
            application.Notifications.OnDisconnect(characterId);

            //remove and invalidate peer
            RemoveClientPeerFromApplication();
        }

        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters) {

            OperationResponse response = null;

            var code = (SelectCharacterOperationCode)operationRequest.OperationCode;
            if(mHandlers.ContainsKey(code)) {
                response = mHandlers[code].Handle(operationRequest, sendParameters);
            }

            if(response != null ) {
                this.SendOperationResponse(response, sendParameters);
            }

        }

        public void SetGroup(string groupID) {
            log.InfoFormat("set group = {0} for character = {1}", groupID, characterId);
            this.groupID = groupID;
        }



        public void SetCharacterId(string cID) {
            characterId = cID;
            
            if(!string.IsNullOrEmpty(id)) {
                var player = application.Players.GetExistingPlayer(id);
                mCachedCharacter = player.Data.GetCharacter(characterId);
            }
        }


        public void SetId(string inId) {
            id = inId;
        }

        public void RemoveClientPeerFromApplication() {
            if(!string.IsNullOrEmpty(id)) {
                //remove peer from peers collection
                application.Clients.OnDisconnect(this);

                //clear id for invalidate peer
                SetId(string.Empty);
            }
        }

        

        public string characterWorldID {
            get {
                var wrapper = application.Players.GetExistingPlayer(id);
                if(wrapper == null ) { return string.Empty; }
                var selectedCharacter = wrapper.Data.GetCharacter(characterId);
                if(selectedCharacter == null) { return string.Empty; }
                return selectedCharacter.WorldId;
            }
        }

        public void SendChatMessage(ChatMessage message) {
            int count = (message.links == null) ? 0 : message.links.Count;
            object[] links = null;
            if(count == 0 ) {
                links = new object[] { };
            } else {
                links = new object[count];
                for(int i = 0; i < count; i++) {
                    links[i] = message.links[i].GetInfo();
                }
            }

            ChatMessageEvent chatEvent = new ChatMessageEvent {
                CharacterID = message.targetCharacterID,
                ChatGroup = message.chatGroup,
                Links = links,
                Message = message.message,
                MessageID = message.messageID,
                SourceCharacterID = message.sourceCharacterID,
                SourceLogin = message.sourceLogin,
                TargetLogin = message.targetLogin
            };

            EventData data = new EventData((byte)SelectCharacterEventCode.ChatMessageEvent, chatEvent);
            SendEvent(data, new SendParameters());
        }

        /// <summary>
        /// Send generic event to client
        /// </summary>
        /// <param name="evt"></param>
        public void SendGenericEvent(GenericEvent evt) {
            EventData data = new EventData((byte)SelectCharacterEventCode.GenericEvent, evt);
            SendEvent(data, new SendParameters());
        }

        public void SendCreditsReceived(int credits) {
            GenericEvent data = new GenericEvent {
                subCode = (int)SelectCharacterGenericEventSubCode.ReceiveCredits,
                data = new Hashtable { { (int)SPC.Credits, credits } }
            };
            SendGenericEvent(data);
        }

        public void SendNewCommaderElected(int race, string login) {
            if(mCachedCharacter != null ) {
                if(mCachedCharacter.Race == race ) {
                    GenericEvent data = new GenericEvent {
                        subCode = (int)SelectCharacterGenericEventSubCode.CommanderElected,
                        data = new Hashtable { { (int)SPC.Login, login } }
                    };
                    SendGenericEvent(data);
                }
            }
        }
    }
}
