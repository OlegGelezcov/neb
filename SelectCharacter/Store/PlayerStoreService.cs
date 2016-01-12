using Common;
using ExitGames.Logging;
using MongoDB.Driver.Builders;
using NebulaCommon;
using NebulaCommon.ServerToServer.Events;
using Photon.SocketServer;
using SelectCharacter.Auction;
using SelectCharacter.Events;
using ServerClientCommon;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectCharacter.Store {
    public class PlayerStoreService : 
        IServer2ServerTransactionHandler<GETInventoryItemTransactionStart, GETInventoryItemTransactionEnd>,
        IServer2ServerTransactionHandler<PUTInventoryItemTransactionStart, PUTInventoryItemTransactionEnd> {

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly Server2ServerTransactionPool<GETInventoryItemTransactionStart, GETInventoryItemTransactionEnd> mTransactionPool;
        private readonly Server2ServerTransactionPool<PUTInventoryItemTransactionStart, PUTInventoryItemTransactionEnd> mPutTransactionPool;

        private readonly SelectCharacterApplication mApplication;
        private readonly ConcurrentDictionary<string, PlayerStore> mPlayerStoreCache;


        public Server2ServerTransactionPool<GETInventoryItemTransactionStart, GETInventoryItemTransactionEnd> inventoryGETPool {
            get {
                return mTransactionPool;
            }
        }

        public Server2ServerTransactionPool<PUTInventoryItemTransactionStart, PUTInventoryItemTransactionEnd> inventoryPUTPool {
            get {
                return mPutTransactionPool;
            }
        }

        public PlayerStoreService(SelectCharacterApplication application) {
            mApplication = application;
            mTransactionPool = new Server2ServerTransactionPool<GETInventoryItemTransactionStart, GETInventoryItemTransactionEnd>(this);
            mPutTransactionPool = new Server2ServerTransactionPool<PUTInventoryItemTransactionStart, PUTInventoryItemTransactionEnd>(this);
            mPlayerStoreCache = new ConcurrentDictionary<string, PlayerStore>();
        }        


        public bool HandleTransaction(GETInventoryItemTransactionStart transactionStart, GETInventoryItemTransactionEnd transactionEnd) {

            if(transactionEnd.transactionSource != (byte)TransactionSource.Store) {
                return false;
            }

            log.InfoFormat("transaction end receibed with success = {0}, return code = {1}", transactionEnd.success, (ReturnCode)transactionEnd.returnCode);

            if(transactionEnd.success) {
                PlayerStore store;
                if(mPlayerStoreCache.TryGetValue(transactionEnd.characterID, out store)) {
                    //store.PutAtStore(transactionEnd.result as Hashtable, transactionEnd.count);
                    switch((PostTransactionAction)transactionStart.postTransactionAction) {
                        case PostTransactionAction.SellItemToNPC:
                            {
                                int price = 0;
                                if( mApplication.itemPriceCollection.TryGetPrice(transactionEnd.result as Hashtable, out price) ) {
                                    log.InfoFormat("sold item for = {0} count = {1}", price, transactionEnd.count);
                                    AddCredits(store.login, transactionEnd.gameRefID, transactionEnd.characterID, price * transactionEnd.count);
                                    return true;
                                }
                                //log.Info()
                            }
                            break;
                        case PostTransactionAction.PutToAuction:
                            {
                                if(transactionStart.tag == null ) {
                                    log.InfoFormat("At PutToAction transaction tag must be price!");
                                    return false;
                                }
                                var storeItem = store.PutAtStore(transactionEnd.result as Hashtable, transactionEnd.count, (int)transactionStart.tag);
                                AuctionItem auctionItem = new AuctionItem {
                                    characterID = store.characterID,
                                    count = transactionEnd.count,
                                    gameRefID = store.gameRefID,
                                    login = store.login,
                                    objectInfo = transactionEnd.result as Hashtable,
                                    price = (int)transactionStart.tag,
                                    storeItemID = storeItem.storeItemID,
                                    time = storeItem.time
                                };
                                mApplication.Auction.Put(auctionItem);
                                break;
                            }
                    }
                    return true;
                }


            } else {
                log.Info("transaction end dont success");
            }

            return false; ;
        }



        public bool HandleTransaction(PUTInventoryItemTransactionStart transactionStart, PUTInventoryItemTransactionEnd transactionEnd) {
            if(transactionEnd.success ) {
                PlayerStore store;
                if(!mPlayerStoreCache.TryGetValue(transactionEnd.characterID, out store)) {
                    return false;
                }

                switch ((PostTransactionAction)transactionStart.postTransactionAction) {
                    case PostTransactionAction.BuyStoreItem:
                        {
                            int price = (int)transactionStart.tag;
                            if (store.RemoveCredits(price)) {
                                SendConsumablePurchaseStatus(transactionStart.gameRefID, true);
                                return true;
                            }
                            SendConsumablePurchaseStatus(transactionStart.gameRefID, false);
                            return false;
                        }
                    default:
                        return true;
                }
            } else {
                return false;
            }
        }

        private void SendConsumablePurchaseStatus(string gameRefID, bool status) {
            GenericEvent evt = new GenericEvent { subCode = (int)SelectCharacterGenericEventSubCode.ConsumablePurchaseStatus, data = new Hashtable { { (int)SPC.Status, status } } };
            mApplication.Clients.SendGenericEventToGameref(gameRefID, evt);
        }

        public bool BuyConsumableItem(string login, string gameRefID, string characterID, int race, string consumableItemID, string targetServer) {
            var store = GetOrCreatePlayerStore(login, gameRefID, characterID);

            ConsumableItem consumableItem;
            if(!mApplication.consumableItems.TryGetItem(consumableItemID, out consumableItem)) {
                return false;
            }

            if(store.credits < consumableItem.price) {
                return false;
            }

            var inventoryObject = mApplication.consumableItems.GetItemFromConsumable(consumableItem, race);
            if(inventoryObject == null ) {
                return false;
            }

            PutItemToInventory(login, 
                gameRefID, 
                characterID, 
                consumableItem.count, 
                InventoryType.station, 
                inventoryObject.Id, 
                PostTransactionAction.BuyStoreItem, 
                consumableItem.price, 
                inventoryObject.GetInfo(),
                targetServer
                );
            return true;
        }

        public bool PutItemToAuction(string login, string gameRefID, string characterID, int count, InventoryType inventoryType, string itemID, int price, string targetServer) {
            var store = GetOrCreatePlayerStore(login, gameRefID, characterID);

            if (!store.hasFreeSlots) {
                log.Info("You don't have free slots");
                return false;
            }

            RequestItemFromInventory(login, characterID, count, gameRefID, inventoryType, itemID, PostTransactionAction.PutToAuction, price, targetServer);
            return true;
        }


        public void SellItemToNPC(string login, string gameRefID, string characterID, int count, InventoryType inventoryType, string itemID, string targetServer) {
            RequestItemFromInventory(login, characterID, count, gameRefID, inventoryType, itemID, PostTransactionAction.SellItemToNPC, 1, targetServer);
        }

        /// <summary>
        /// Call this from peer to buy auction item
        /// </summary>
        /// <param name="login">Customer login</param>
        /// <param name="gameRefID">Customer game ref id</param>
        /// <param name="characterID">Customer character id</param>
        /// <param name="storeItemID">Auction item store ID</param>
        /// <returns></returns>
        public bool BuyAuctionItem(string login, string gameRefID, string characterID, string storeItemID ) {

            
            //Get item from auction
            AuctionItem auctionItem = mApplication.Auction.GetItem(storeItemID);
            if(auctionItem == null ) {
                log.InfoFormat("BuyAuctionItem: auction item not founded [red]");
                return false;
            }

            if(auctionItem.gameRefID == gameRefID ) {
                log.InfoFormat("BuyAuctionItem: don't allow buy items from self [red]");
                return false;
            }

            //Get store of customer
            var store = GetOrCreatePlayerStore(login, gameRefID, characterID);
            if(store == null) {
                log.InfoFormat("BuyAuctionItem: store of customer not founded [red]");
                return false;
            }

            //Check the customer have enough credits
            if(store.credits < auctionItem.price ) {
                log.InfoFormat("BuyAuctionItem: customer dont has enough credits {0} < {1} [red]", store.credits, auctionItem.price);
                return false;
            }

            //Delete item from acutino
            var deletedItem = mApplication.Auction.Remove(storeItemID);           
            if(deletedItem == null ) {
                log.InfoFormat("BuyAuctionItem: error of deleting auction item from auction [red]");
                return false;
            }

            //Delete item from seller store
            if(!RemoveFromStore(deletedItem.login, deletedItem.gameRefID, deletedItem.characterID, deletedItem.storeItemID)) {
                log.InfoFormat("BuyAuctionItem: error of removing auction item from seller store [red]");
                return false;
            }
            //Remove credist from customer store
            if( !RemoveCredits(login, gameRefID, characterID, deletedItem.price) ) {
                log.InfoFormat("BuyAuctionItem: error of removing credits from customer store [red]");
                return false;
            }

            if(!AddCredits(deletedItem.login, deletedItem.gameRefID, deletedItem.characterID, deletedItem.price)) {
                log.InfoFormat("BuyAuctionItem: error of adding credits to seller store [red]");
            }

            //Put purchased item to mail as attachment
            bool success =  mApplication.Mail.PutAuctionItemToMailFromPurchase(gameRefID, deletedItem);

            if(success ) {
                Hashtable data = new Hashtable {
                    { (int)SPC.Price, deletedItem.price }
                };
                var n = mApplication.Notifications.Create(deletedItem.storeItemID, "s_you_item_purchased", data, 
                    NotficationRespondAction.Delete, NotificationSourceServiceType.Server, NotificationSubType.AuctionPurchase);
                mApplication.Notifications.SetNotificationToCharacter(deletedItem.characterID, n);
            }
            return success;
        }

        /// <summary>
        /// Remove item from auction
        /// </summary>
        public bool RemoveAuctionItem(string login, string gameRefID, string characterID, string storeItemID ) {
            var store = GetOrCreatePlayerStore(login, gameRefID, characterID);
            if(!store.RemoveFromStore(storeItemID)) {
                return false;
            }
            AuctionItem testItem = mApplication.Auction.GetItem(storeItemID);
            if(testItem == null ) {
                return false;
            }

            if((login != testItem.login) || (gameRefID != testItem.gameRefID) || (characterID != testItem.characterID)) {
                return false;
            }
            AuctionItem item =  mApplication.Auction.Remove(storeItemID);
            
            if(item == null ) {
                return false;
            }

            return mApplication.Mail.PutAuctionItemBack(item);
        }

        /// <summary>
        /// Set new price on item
        /// </summary>
        public bool SetNewPrice(string login, string gameRefID, string characterID, string storeItemID, int newPrice) {
            var store = GetOrCreatePlayerStore(login, gameRefID, characterID);
            if(!store.ContainsStoreItem(storeItemID)) {
                return false;
            }
            var item = mApplication.Auction.GetItem(storeItemID);
            if(item == null ) {
                return false;
            }

            if((login != item.login) || (gameRefID != item.gameRefID) || (characterID != item.characterID)) {
                return false;
            }

            item.price = newPrice;
            mApplication.Auction.WriteItem(item);
            store.SetNewPrice(item.storeItemID, newPrice);

            SendStoreUpdate(store);
            log.InfoFormat("send store update after setting price = {0}", newPrice);

            return true;
        }

        private void RequestItemFromInventory(string login, string characterID, int count, string gameRefID, 
            InventoryType inventoryType, string itemID, PostTransactionAction action, object tag, string targetServer) {
            var playerStore = GetOrCreatePlayerStore(login, gameRefID, characterID);
            if (playerStore == null) {
                log.InfoFormat("player store is null");
                return;
            }

            GETInventoryItemTransactionStart start = new GETInventoryItemTransactionStart {
                characterID = characterID,
                count = count,
                gameRefID = gameRefID,
                inventoryType = (byte)inventoryType,
                itemID = itemID,
                transactionID = Guid.NewGuid().ToString(),
                transactionSource = (byte)TransactionSource.Store,
                postTransactionAction = (byte)action,
                tag = tag,
                transactionEndServer = targetServer,
                 transactionStartServer = SelectCharacterApplication.ServerId.ToString()
            };
            EventData evt = new EventData((byte)S2SEventCode.GETInventoryItemStart, start);
            mTransactionPool.StartTransaction(start);
            mApplication.MasterPeer.SendEvent(evt, new SendParameters());
            log.Info("store transaction started...");
        }

        /// <summary>
        /// Send event to put item to inventory
        /// </summary>
        /// <param name="login">Login os player</param>
        /// <param name="gameRefID">Game ref ID of player</param>
        /// <param name="characterID">Character of player</param>
        /// <param name="count">Count of item will be putted</param>
        /// <param name="inventoryType">Inventory type of where to put item</param>
        /// <param name="itemID">ID item of will be putted</param>
        /// <param name="action">Action after success complete transaction</param>
        /// <param name="tag">Some tag information with transaction</param>
        /// <param name="objectData">Object Data of item will be putted</param>
        private void PutItemToInventory(string login, 
            string gameRefID, 
            string characterID, 
            int count, 
            InventoryType inventoryType, 
            string itemID, 
            PostTransactionAction action, 
            object tag, 
            object objectData,
            string targetServer) {
            var playerStore = GetOrCreatePlayerStore(login, gameRefID, characterID);
            if(playerStore == null ) {
                log.Info("PutItemToInventory: player store is null");
                return;
            }

            PUTInventoryItemTransactionStart start = new PUTInventoryItemTransactionStart {
                characterID = characterID,
                count = count,
                gameRefID = gameRefID,
                inventoryType = (byte)inventoryType,
                itemID = itemID,
                postTransactionAction = (byte)action,
                tag = tag,
                targetObject = objectData,
                transactionSource = (byte)TransactionSource.Store,
                transactionID = Guid.NewGuid().ToString(),
                transactionEndServer = targetServer,
                 transactionStartServer = SelectCharacterApplication.ServerId.ToString()
            };
            EventData evt = new EventData((byte)S2SEventCode.PUTInventoryItemStart, start);
            mPutTransactionPool.StartTransaction(start);
            mApplication.MasterPeer.SendEvent(evt, new SendParameters());
            log.Info("store PUT transaction started...");
        }

        public void SaveChanges() {
            foreach(var store in mPlayerStoreCache) {
                if(store.Value.IsChanged()) {
                    mApplication.DB.Stores.Save(store.Value);
                    store.Value.ResetChanged();
                }
            }
        }

        public PlayerStore GetOrCreatePlayerStore(string login, string gameRefID, string characterID ) {

            if(mPlayerStoreCache.ContainsKey(characterID)) {
                PlayerStore store;
                if(mPlayerStoreCache.TryGetValue(characterID, out store)) {
                    return store;
                } else {
                    store = new PlayerStore {
                        characterID = characterID,
                        gameRefID = gameRefID,
                        login = login,
                        storeItems = new Dictionary<string, PlayerStoreItem>()
                    };
                    mApplication.DB.Stores.Save(store);
                    mPlayerStoreCache[store.characterID] = store;
                    return store;
                }

            } else {
                var store = mApplication.DB.Stores.FindOne(Query<PlayerStore>.EQ(s => s.characterID, characterID));
                if(store != null ) {
                    mPlayerStoreCache.TryAdd(store.characterID, store);
                    return store;
                } else {
                    store = new PlayerStore {
                        characterID = characterID,
                        gameRefID = gameRefID,
                        login = login,
                        storeItems = new Dictionary<string, PlayerStoreItem>()
                    };
                    mApplication.DB.Stores.Save(store);
                    mPlayerStoreCache.TryAdd(store.characterID, store);
                    return store;
                }
            }
        }

        public PlayerStore GetOnlyPlayerStore(string characterID) {

            if (mPlayerStoreCache.ContainsKey(characterID)) {
                PlayerStore store;
                if (mPlayerStoreCache.TryGetValue(characterID, out store)) {
                    return store;
                } 

            } else {
                var store = mApplication.DB.Stores.FindOne(Query<PlayerStore>.EQ(s => s.characterID, characterID));
                if (store != null) {
                    mPlayerStoreCache.TryAdd(store.characterID, store);
                    return store;
                } 
            }

            return null;
        }

        public bool SetCreditsBonus(string characterId,  float bonus ) {
            var store = GetOnlyPlayerStore(characterId);
            if(store == null ) {
                return false;
            }
            store.SetCreditsBonus(bonus);
            return true;
        }

        public bool HasCredits(string login, string gameRefID, string characterID, int credits) {
            var store = GetOrCreatePlayerStore(login, gameRefID, characterID);
            if (store == null) {
                log.InfoFormat("RemoveCredits(): store for character not found");
                return false;
            }
            return (store.credits >= credits);
        }


        public bool AddCredits(string login, string gameRefID, string characterID, int credits) {
            if(credits < 0 ) {
                log.Info("AddCredits(): credits must be >= 0");
                return false;
            }

            var store = GetOrCreatePlayerStore(login, gameRefID, characterID);
            if(store == null ) {
                log.InfoFormat("AddCredits(): store for character not found");
                return false;
            }

            credits = store.AddCredits(credits);

            SelectCharacterClientPeer peer;
            if(mApplication.Clients.TryGetPeerForGameRefId(gameRefID, out peer)) {
                peer.SendCreditsReceived(credits);
            }
            return true;
        }

        public bool AddPvpPoints(string login, string gameRef, string character, int pvpPoints) {
            if(pvpPoints < 0 ) {
                log.InfoFormat("AddPvpPoints say: pvp points count must be > 0");
                return false;
            }
            var store = GetOrCreatePlayerStore(login, gameRef, character);
            if(store == null ) {
                log.InfoFormat("AddPvpPoints say: store of character must be not (null)");
                return false;
            }

            store.AddPvpPoints(pvpPoints);
            return true;

        }

        public bool RemovePvpPoints(string character, int pvpPoints) {
            if(pvpPoints < 0 ) {
                log.InfoFormat("RemovePvpPoints say: pvp points count must be > 0 [red]");
                return false;
            }

            var store = GetOnlyPlayerStore(character);
            if(store == null ) {
                log.InfoFormat("RemovePvpPoints say: store of character must be not (null) [red]");
                return false;
            }

            return store.RemovePvpPoints(pvpPoints);
        }

        public bool RemoveCredits(string login, string gameRefID, string characterID, int credits) {
            if(credits < 0 ) {
                log.Info("RemoveCredits(): credits must be >= 0");
                return false;
            }

            var store = GetOrCreatePlayerStore(login, gameRefID, characterID);
            if(store == null ) {
                log.InfoFormat("RemoveCredits(): store for character not found");
                return false;
            }

            return store.RemoveCredits(credits);
        }

        public void SendStoreUpdate(PlayerStore store) {
            SelectCharacterClientPeer peer;
            if(mApplication.Clients.TryGetPeerForCharacterId(store.characterID, out peer)) {
                log.Info("send store update to client");
                PlayerStoreUpdateEvent data = new PlayerStoreUpdateEvent { storeInfo = store.GetInfo() };
                EventData evt = new EventData((byte)SelectCharacterEventCode.PlayerStoreUpdate, data);
                peer.SendEvent(evt, new SendParameters());
            }
        }

        /// <summary>
        /// Send pvp point update generic event to client
        /// </summary>
        /// <param name="store">Store of client</param>
        public void SendPvpPointsUpdate(PlayerStore store) {
            SelectCharacterClientPeer peer;
            if(mApplication.Clients.TryGetPeerForCharacterId(store.characterID, out peer)) {
                log.InfoFormat("sending PvP points updates to client peer [red]");
                GenericEvent eventObject = new GenericEvent {
                     data = new Hashtable { { (int)SPC.PvpPoints, store.pvpPoints } },
                     subCode = (int)SelectCharacterGenericEventSubCode.PvpPointUpdate
                };
                EventData eventData = new EventData((byte)SelectCharacterEventCode.GenericEvent, eventObject);
                peer.SendEvent(eventData, new SendParameters());
            } 
        }

        public bool RemoveFromStore(string login, string gameRefID, string characterID, string storeItemID ) {
            var store = GetOrCreatePlayerStore(login, gameRefID, characterID);
            if(store == null ) {
                return false;
            }
            return store.RemoveFromStore(storeItemID);
        }
    }
}
