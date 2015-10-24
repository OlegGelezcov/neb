using Common;
using ExitGames.Logging;
using NebulaCommon;
using NebulaCommon.SelectCharacter;
using NebulaCommon.ServerToServer.Events;
using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;
using SelectCharacter.Bank;
using SelectCharacter.Chat;
using SelectCharacter.Events;
using SelectCharacter.OperationHandlers;
using ServerClientCommon;
using System.Collections;
using System.Collections.Generic;
using System;
using Nebula.Inventory;

namespace SelectCharacter {
    public class SelectCharacterClientPeer : PeerBase,
        IServer2ServerTransactionHandler<GETInventoryItemTransactionStart, GETInventoryItemTransactionEnd>,
        IServer2ServerTransactionHandler<PUTInventoryItemTransactionStart, PUTInventoryItemTransactionEnd>  {

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        public SelectCharacterApplication application;
        public MethodInvoker invoker { get; private set; }

        public string id { get; private set; }
        public string login { get; private set; }

        public string characterId { get; private set; }
        public string groupID { get; private set; } = string.Empty;
        private DbPlayerCharacter mCachedCharacter;

        private readonly Dictionary<SelectCharacterOperationCode, BaseOperationHandler> mHandlers;

        private BankSave mBankSave;

        public Bank.Bank bank { get; private set; }

        private readonly Server2ServerTransactionPool<GETInventoryItemTransactionStart, GETInventoryItemTransactionEnd> mGetPool;
        private readonly Server2ServerTransactionPool<PUTInventoryItemTransactionStart, PUTInventoryItemTransactionEnd> mPutPool;

        public DbPlayerCharacter selectedCharacter {
            get {
                return mCachedCharacter;
            }
        }

        public Server2ServerTransactionPool<GETInventoryItemTransactionStart, GETInventoryItemTransactionEnd> getPool {
            get {
                return mGetPool;
            }
        }

        public Server2ServerTransactionPool<PUTInventoryItemTransactionStart, PUTInventoryItemTransactionEnd> putPool {
            get {
                return mPutPool;
            }
        }


        public SelectCharacterClientPeer(InitRequest initRequest, SelectCharacterApplication application)
            : base(initRequest.Protocol, initRequest.PhotonPeer) {
            this.application = application;
            invoker = new MethodInvoker(application, this);
            mHandlers = new Dictionary<SelectCharacterOperationCode, BaseOperationHandler>();
            mGetPool = new Server2ServerTransactionPool<GETInventoryItemTransactionStart, GETInventoryItemTransactionEnd>(this);
            mPutPool = new Server2ServerTransactionPool<PUTInventoryItemTransactionStart, PUTInventoryItemTransactionEnd>(this);

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
            mHandlers.Add(SelectCharacterOperationCode.SendPushToPlayers, new SendPushToPlayersOperationHandler(application, this));
            mHandlers.Add(SelectCharacterOperationCode.MoveItemFromStationToBank, new MoveItemFromStationToBankOperationHandler(application, this));
            mHandlers.Add(SelectCharacterOperationCode.MoveItemFromBankToStation, new MoveItemFromBankToStationOperationHandler(application, this));
        }

        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail) {
            if (log.IsDebugEnabled) {
                log.DebugFormat("LoginClientPeer Disconnect: pid={0}: reason={1}, detail={2}", this.ConnectionId, reasonCode, reasonDetail);
            }

            UnloadBank();

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

        public void SetLogin(string inLogin) {
            login = inLogin;
        }

        public void SetBank(BankSave save) {
            mBankSave = save;
            if(mBankSave != null && bank == null ) {
                bank = new Bank.Bank();
                bank.ParseInfo(mBankSave.bankInfo);
                log.InfoFormat("bank loaded max count = {0} [red]", bank.maxSlots);
                SendBankUpdate();
            }
        }

        public void UnloadBank() {
            if(bank != null && mBankSave != null ) {
                mBankSave.bankInfo = bank.GetInfo();
                application.DB.SaveBank(mBankSave);
                bank = null;
                mBankSave = null;
            }
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


        public bool HandleTransaction(GETInventoryItemTransactionStart transactionStart, GETInventoryItemTransactionEnd transactionEnd) {
            if(transactionEnd.success) {
                //after getting item from inventory place it in bank and send update to client
                if(transactionStart.postTransactionAction == (byte)PostTransactionAction.AddToBank) {
                    if(bank != null ) {
                        if(transactionEnd.result != null ) {
                            Hashtable itemHash = transactionEnd.result as Hashtable;
                            if(itemHash != null ) {
                                int cnt = 0;
                                var itemObj = InventoryUtils.Create(itemHash, out cnt);
                                if(itemObj != null ) {
                                    bank.AddItem(itemObj, transactionEnd.count);
                                    SendBankUpdate();
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        public bool HandleTransaction(PUTInventoryItemTransactionStart transactionStart, PUTInventoryItemTransactionEnd transactionEnd) {
            if(transactionEnd.success) {
                //after putting item to station remove item from bank
                if (transactionStart.postTransactionAction == (byte)PostTransactionAction.WithdrawFromBank) {
                    if (bank != null) {
                        bank.RemoveItem(transactionStart.itemID, transactionStart.count);
                        SendBankUpdate();
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Start transaction of moving item from bank to player station
        /// itemid - id of inventory item in bank
        /// count - count of items to move
        /// server id - id of server where transaction handler placed
        /// </summary>
        public bool MoveItemToStation(string itemid, int count, string serverId) {
            if (isUserRegisterd) {
                if (bank != null) {
                    var item = bank.GetItem(itemid);
                    if (item.Count >= count) {
                        PUTInventoryItemTransactionStart start = new PUTInventoryItemTransactionStart {
                            characterID = characterId,
                            count = count,
                            gameRefID = id,
                            itemID = itemid,
                            postTransactionAction = (byte)PostTransactionAction.WithdrawFromBank,
                            inventoryType = (byte)InventoryType.station,
                            tag = 0,
                            targetObject = item.GetInfo(),
                            transactionID = Guid.NewGuid().ToString(),
                            transactionSource = (byte)TransactionSource.Bank,
                            transactionStartServer = SelectCharacterApplication.ServerId.ToString(),
                            transactionEndServer = serverId
                        };
                        EventData eventData = new EventData((byte)S2SEventCode.PUTInventoryItemStart, start);
                        mPutPool.StartTransaction(start);
                        application.MasterPeer.SendEvent(eventData, new SendParameters());
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Start S2S transaction of moving item from player station to bank. Return true if transaction successfully started
        /// </summary>
        public bool MoveItemFromStation(string itemid, int count, string serverId) {

            if(isUserRegisterd) {
                if (bank != null) {
                    if (bank.HasSpaceForItems(itemid)) {
                        GETInventoryItemTransactionStart start = new GETInventoryItemTransactionStart {
                            characterID = characterId,
                            count = count,
                            gameRefID = id,
                            inventoryType = (byte)InventoryType.station,
                            itemID = itemid,
                            transactionID = Guid.NewGuid().ToString(),
                            transactionSource = (byte)TransactionSource.Bank,
                            postTransactionAction = (byte)PostTransactionAction.AddToBank,
                            tag = 0,
                            transactionEndServer = serverId,
                            transactionStartServer = SelectCharacterApplication.ServerId.ToString()
                        };
                        EventData eventData = new EventData((byte)S2SEventCode.GETInventoryItemStart, start);
                        mGetPool.StartTransaction(start);
                        application.MasterPeer.SendEvent(eventData, new SendParameters());
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Send bank content update event to client
        /// </summary>
        private void SendBankUpdate() {
            if (bank != null) {
                log.InfoFormat("sending bank update to client [red]");
                GenericEvent evtObj = new GenericEvent {
                    subCode = (byte)SelectCharacterGenericEventSubCode.BankUpdate,
                    data = bank.GetInfo()
                };

                EventData eventData = new EventData((byte)SelectCharacterEventCode.GenericEvent, evtObj);
                SendEvent(eventData, new SendParameters());
            }
        }

        public bool RemoveBankItem(string item, int count) {
            if(bank != null ) {
                bool success = bank.RemoveItem(item, count);
                if(success ) {
                    SendBankUpdate();
                }
                return success;
            }
            return false;
        }

        public bool BuyMaxSlots() {
            if(bank == null ) {
                return false;
            }
            var price = application.bankSlotPrices.GetPriceForSlots(bank.maxSlots);
            if(price == null ) {
                return false;
            }

            if(application.Stores.HasCredits(login, id, characterId, price.price)) {
                if(application.Stores.RemoveCredits(login, id, characterId, price.price)) {
                    return AddMaxSlots(10);
                }
            }
            return false;
        }

        private bool AddMaxSlots(int count) {
            if(bank != null ) {
                bank.AddMaxSlot(count);
                SendBankUpdate();
                return true;
            }
            return false;
        }

        private bool isUserRegisterd {
            get {
                return (!string.IsNullOrEmpty(characterId)) && (!string.IsNullOrEmpty(id));
            }
        }
    }
}
