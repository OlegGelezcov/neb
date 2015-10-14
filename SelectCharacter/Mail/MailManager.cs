using MongoDB.Driver.Builders;
using NebulaCommon;
using NebulaCommon.ServerToServer.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using Common;
using ServerClientCommon;
using Photon.SocketServer;
using SelectCharacter.Events;
using SelectCharacter.Auction;
using ExitGames.Logging;

namespace SelectCharacter.Mail {
    public class MailManager : IServer2ServerTransactionHandler<PUTInventoryItemTransactionStart, PUTInventoryItemTransactionEnd>,
        IServer2ServerTransactionHandler<GETInventoryItemTransactionStart, GETInventoryItemTransactionEnd>,
        IServer2ServerTransactionHandler<GETInventoryItemsTransactionStart, GETInventoryItemsTransactionEnd>{

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        protected SelectCharacterApplication application { get; private set; }
        private readonly Server2ServerTransactionPool<PUTInventoryItemTransactionStart, PUTInventoryItemTransactionEnd> mPutTransactionPool;
        private readonly Server2ServerTransactionPool<GETInventoryItemTransactionStart, GETInventoryItemTransactionEnd> mGetTransactionPool;
        private readonly Server2ServerTransactionPool<GETInventoryItemsTransactionStart, GETInventoryItemsTransactionEnd> mGetItemsTransactionPool;


        public Server2ServerTransactionPool<PUTInventoryItemTransactionStart, PUTInventoryItemTransactionEnd> inventoryPUTPool {
            get {
                return mPutTransactionPool;
            }
        }

        public Server2ServerTransactionPool<GETInventoryItemTransactionStart, GETInventoryItemTransactionEnd> inventoryGETPool {
            get {
                return mGetTransactionPool;
            }
        }

        public Server2ServerTransactionPool<GETInventoryItemsTransactionStart, GETInventoryItemsTransactionEnd> inventoryItemsGETPool {
            get {
                return mGetItemsTransactionPool;
            }
        }

        public MailManager(SelectCharacterApplication application ) {
            this.application = application;
            mPutTransactionPool = new Server2ServerTransactionPool<PUTInventoryItemTransactionStart, PUTInventoryItemTransactionEnd>(this);
            mGetTransactionPool = new Server2ServerTransactionPool<GETInventoryItemTransactionStart, GETInventoryItemTransactionEnd>(this);
            mGetItemsTransactionPool = new Server2ServerTransactionPool<GETInventoryItemsTransactionStart, GETInventoryItemsTransactionEnd>(this);
        }

        public Hashtable GetMails(string gameRefId ) {

            var query = Query<MailBox>.EQ(mb => mb.gameRefId, gameRefId);

            var result = application.DB.Mails.FindOne(query);

            if(result == null ) {
                return new Hashtable();
            } else {
                return result.GetInfo();
            }
        }

        public void SaveMails(MailBox mails) {
            mails.ClearExpiredMails();
            application.DB.Mails.Save(mails);
        }

        public MailBox GetMailBox(string gameRefId) {
            var query = Query<MailBox>.EQ(mb => mb.gameRefId, gameRefId);

            var result = application.DB.Mails.FindOne(query);
            if(result == null ) {
                result = new MailBox {
                    gameRefId = gameRefId,
                    messages = new Dictionary<string, MailMessage>()
                };
                application.DB.Mails.Save(result);
            }
            return result;
        }

        public bool HandleTransaction(GETInventoryItemTransactionStart transactionStart, GETInventoryItemTransactionEnd transactionEnd) {
            return false;
        }

        public bool HandleTransaction(GETInventoryItemsTransactionStart transactionStart, GETInventoryItemsTransactionEnd transactionEnd) {
            if (transactionStart.transactionSource != transactionEnd.transactionSource) {
                return false;
            }
            if (transactionEnd.returnCode != (short)ReturnCode.Ok) {
                return false;
            }
            if (!transactionEnd.success) {
                return false;
            }

            switch ((PostTransactionAction)transactionStart.postTransactionAction) {
                case PostTransactionAction.PutItemsToAttachment:
                    log.InfoFormat("handle mail message with attachments when end");
                    if(transactionStart.GetNotSended() == null ) {
                        log.InfoFormat("NotSendeddata is null red");
                        return false;
                    }
                    MailMessage message = transactionStart.GetNotSended() as MailMessage;
                    message.ClearAttachments();
                    Hashtable resultHash = transactionEnd.result as Hashtable;
                    foreach(DictionaryEntry entry in resultHash) {
                        Hashtable itemHash = entry.Value as Hashtable;
                        Hashtable itemInfo = itemHash[(int)SPC.Info] as Hashtable;
                        int count = (int)itemHash[(int)SPC.Count];
                        log.InfoFormat("add some attachment to mail yellow");
                        message.AddAttachment(itemInfo, count);
                    }
                    MailBox mailBox = GetMailBox(message.receiverGameRefId);

                    mailBox.AddNewMessage(message);

                    SaveMails(mailBox);

                    application.Clients.SendGenericEventToGameref(mailBox.gameRefId, 
                        new GenericEvent { subCode = (int)SelectCharacterGenericEventSubCode.NewMessageCountChanged,
                            data = new Hashtable { { (int)SPC.Count, mailBox.newMessagesCount } } });

                    log.InfoFormat("Message added to target mailbox yellow");

                    MailUpdatedEvent evt = new MailUpdatedEvent { mailBox = mailBox.GetInfo() };
                    EventData data = new EventData((byte)SelectCharacterEventCode.MailUpdateEvent, evt);
                    application.SendEventToClient(message.receiverGameRefId, data);
                    log.InfoFormat("message sended to target yellow");
                    return true;
            }
            return false;
        }



        public bool HandleTransaction(PUTInventoryItemTransactionStart transactionStart, PUTInventoryItemTransactionEnd transactionEnd) {
            
            if(transactionStart.transactionSource != transactionEnd.transactionSource) {
                return false;
            }
            if(transactionEnd.returnCode != (short)ReturnCode.Ok) {
                return false;
            }
            if(!transactionEnd.success) {
                return false;
            }

            switch((PostTransactionAction)transactionStart.postTransactionAction) {
                case PostTransactionAction.RemoveMailAttachment:
                    {
                        Hashtable tagHash = transactionStart.tag as Hashtable;

                        string attachmentID = tagHash.GetValue<string>((int)SPC.Id, string.Empty);
                        string messageID = tagHash.GetValue<string>((int)SPC.Message, string.Empty);

                        if(string.IsNullOrEmpty(attachmentID)) {
                            return false;
                        }

                        if(string.IsNullOrEmpty(messageID)) {
                            return false;
                        }

                        var mailBox = GetMailBox(transactionStart.gameRefID);
                        if(mailBox == null ) {
                            return false;
                        }
                        bool result = mailBox.RemoveAttachment(messageID, attachmentID);

                        if(!result) {
                            return false;
                        }

                        SaveMails(mailBox);

                        MailUpdatedEvent evt = new MailUpdatedEvent { mailBox = mailBox.GetInfo() };
                        EventData data = new EventData((byte)SelectCharacterEventCode.MailUpdateEvent, evt);
                        application.SendEventToClient(transactionStart.gameRefID, data);

                        return true;
                    }
                default:
                    return false;
            }
        }

        public bool StartPutAttachmentToStation(string login, string messageID, string attachmentID ) {

            log.InfoFormat("PUT ATTCHMENT to inventory started");

            var player = application.DB.GetByLogin(login);
            if(player == null ) {
                log.InfoFormat("player not found");
                return false;
            }

            var mailBox = GetMailBox(player.GameRefId);

            if(mailBox == null ) {
                log.InfoFormat("mail box not found");
                return false;
            }

            MailAttachment attachment;
            if(!mailBox.TryGetAttachment(messageID, attachmentID, out attachment)) {
                log.ErrorFormat("desired attachment not found");
                return false;
            }

            if(string.IsNullOrEmpty(player.SelectedCharacterId)) {
                log.ErrorFormat("player don't have selected character");
                return false;
            }

            string itemID = attachment.objectHash.GetValue<string>((int)SPC.Id, string.Empty);
            if(string.IsNullOrEmpty(itemID)) {
                log.ErrorFormat("attachment item id is invalid");
                return false;
            }

            PUTInventoryItemTransactionStart transaction = new PUTInventoryItemTransactionStart {
                characterID = player.SelectedCharacterId,
                count = attachment.count,
                gameRefID = player.GameRefId,
                inventoryType = (byte)InventoryType.station,
                itemID = itemID,
                postTransactionAction = (byte)PostTransactionAction.RemoveMailAttachment,
                tag = new Hashtable { { (int)SPC.Id, attachmentID }, { (int)SPC.Message, messageID } },
                targetObject = attachment.objectHash,
                transactionID = Guid.NewGuid().ToString(),
                transactionSource = (byte)TransactionSource.Mail
            };
            EventData evt = new EventData((byte)S2SEventCode.PUTInventoryItemStart, transaction);
            mPutTransactionPool.StartTransaction(transaction);
            application.MasterPeer.SendEvent(evt, new SendParameters());
            return true;
        }

        public bool ResetNewMessageCount(string gameRefID) {
            var mailBox = GetMailBox(gameRefID);
            if(mailBox != null ) {
                int oldCount = mailBox.newMessagesCount;
                mailBox.ResetNewMessages();
                if(oldCount != 0 ) {
                    application.Clients.SendGenericEventToGameref(mailBox.gameRefId,
                    new GenericEvent {
                        subCode = (int)SelectCharacterGenericEventSubCode.NewMessageCountChanged,
                        data = new Hashtable { { (int)SPC.Count, mailBox.newMessagesCount } }
                    });
                    SaveMails(mailBox);
                }
                

                return true;
            }
            return false;
        }

        public bool StartWriteMessageTransaction(string senderGameRefID, string senderDisplayName, byte inventoryType, 
            string receiverGameRefID, string title, string body, Hashtable attachments ) {
            log.InfoFormat("Started writing message");

            var mailBox = GetMailBox(receiverGameRefID);
            if(mailBox == null ) {
                log.ErrorFormat("mail box of receiver is null");
                return false;
            }
            if(senderGameRefID == mailBox.gameRefId) {
                log.ErrorFormat("sender and receiver are same");
                return false;
            }
            if(receiverGameRefID != mailBox.gameRefId) {
                log.ErrorFormat("invalid mail box");
                return false;
            }

            MailMessage message = new MailMessage {
                attachments = new Dictionary<string, MailAttachment>(),
                body = body,
                id = Guid.NewGuid().ToString(),
                receiverGameRefId = receiverGameRefID,
                sendefGameRefId = senderGameRefID,
                senderLogin = senderDisplayName,
                time = DateTime.UtcNow.ToString(System.Globalization.CultureInfo.InvariantCulture),
                title = title
            };

            var player = application.Players.GetExistingPlayer(senderGameRefID);
            if(player == null ) {
                log.ErrorFormat("sender player not found");
                return false;
            }

            if(string.IsNullOrEmpty(player.Data.SelectedCharacterId)) {
                log.ErrorFormat("sender does'nt have selected character id");
                return false;
            }


            if (attachments == null || attachments.Count == 0) {
                log.InfoFormat("Attachments don't exists simple write message in receiver mailbox");

                mailBox.AddNewMessage(message);
                SaveMails(mailBox);

                application.Clients.SendGenericEventToGameref(mailBox.gameRefId,
                    new GenericEvent {
                        subCode = (int)SelectCharacterGenericEventSubCode.NewMessageCountChanged,
                        data = new Hashtable { { (int)SPC.Count, mailBox.newMessagesCount } }
                    });

                MailUpdatedEvent evt = new MailUpdatedEvent { mailBox = mailBox.GetInfo() };
                EventData data = new EventData((byte)SelectCharacterEventCode.MailUpdateEvent, evt);
                application.SendEventToClient(message.receiverGameRefId, data);

            } else {
                log.InfoFormat("Exist attachment send S2SEventCode.GETInventoryItemsStart - transaction to inventory");

                GETInventoryItemsTransactionStart start = new GETInventoryItemsTransactionStart {
                    characterID = player.Data.SelectedCharacterId,
                    gameRefID = senderGameRefID,
                    inventoryType = inventoryType,
                    items = attachments,
                    postTransactionAction = (byte)PostTransactionAction.PutItemsToAttachment,
                    tag = new Hashtable(),
                    transactionID = Guid.NewGuid().ToString(),
                    transactionSource = (byte)TransactionSource.Mail
                };
                start.SetNotSended(message);

                EventData evt = new EventData((byte)S2SEventCode.GETInventoryItemsStart, start);
                mGetItemsTransactionPool.StartTransaction(start);
                application.MasterPeer.SendEvent(evt, new SendParameters());
            }
            return true;
        }

        /// <summary>
        /// Put purchased item to mail as attachment
        /// </summary>
        /// <param name="gameRefID">Game ref id of mailbox</param>
        /// <param name="item">Item which will be put to mail</param>
        /// <returns>True if all succeded</returns>
        public bool PutAuctionItemToMailFromPurchase(string gameRefID, AuctionItem item) {

            log.InfoFormat("PutAuctionItemToMailFromPurchase() put item from auction to mail player = {0}, item = {1}",
                gameRefID, item.storeItemID);

            //First create message
            MailMessage message = new MailMessage {
                //attachment empty
                attachments = new Dictionary<string, MailAttachment>(),
                //standard body for purchases
                body = "You have purchased item",
                //unique id of message
                id = Guid.NewGuid().ToString(),
                //game ref id of receiver
                receiverGameRefId = gameRefID,
                //sender is empty (auction)
                sendefGameRefId = string.Empty,
                //sender is empty (auction)
                senderLogin = string.Empty,
                //time of message
                time = DateTime.UtcNow.ToString(System.Globalization.CultureInfo.InvariantCulture),
                //standard title of message
                title = "Purchase on auction"
            };
            //add attachment to message
            message.AddAttachment(item.objectInfo, item.count);
            //get mailbox of player
            var mailBox = GetMailBox(gameRefID);
            //add message to mailbox
            mailBox.AddNewMessage(message);

            application.Clients.SendGenericEventToGameref(mailBox.gameRefId,
                new GenericEvent {
                    subCode = (int)SelectCharacterGenericEventSubCode.NewMessageCountChanged,
                    data = new Hashtable { { (int)SPC.Count, mailBox.newMessagesCount } }
                });

            //save mail box to DB
            SaveMails(mailBox);
            //send update event to player
            MailUpdatedEvent evt = new MailUpdatedEvent { mailBox = mailBox.GetInfo() };
            EventData data = new EventData((byte)SelectCharacterEventCode.MailUpdateEvent, evt);
            application.SendEventToClient(gameRefID, data);
            log.InfoFormat("Successfully pushed item to mail from auction");
            return true;
        }

        public bool PutAuctionItemBack(AuctionItem item ) {
            log.InfoFormat("Put back auction item via mail");
            MailMessage message = new MailMessage {
                attachments = new Dictionary<string, MailAttachment>(),
                body = "Return from auction",
                id = Guid.NewGuid().ToString(),
                receiverGameRefId = item.gameRefID,
                sendefGameRefId = string.Empty,
                senderLogin = string.Empty,
                time = DateTime.UtcNow.ToString(System.Globalization.CultureInfo.InvariantCulture),
                title = "Return"
            };
            message.AddAttachment(item.objectInfo, item.count);
            var mailBox = GetMailBox(item.gameRefID);
            mailBox.AddNewMessage(message);

            application.Clients.SendGenericEventToGameref(mailBox.gameRefId,
                new GenericEvent {
                    subCode = (int)SelectCharacterGenericEventSubCode.NewMessageCountChanged,
                    data = new Hashtable { { (int)SPC.Count, mailBox.newMessagesCount } }
                });

            SaveMails(mailBox);

            MailUpdatedEvent evt = new MailUpdatedEvent { mailBox = mailBox.GetInfo() };
            EventData data = new EventData((byte)SelectCharacterEventCode.MailUpdateEvent, evt);
            application.SendEventToClient(item.gameRefID, data);
            return true;
        }
    }
}
