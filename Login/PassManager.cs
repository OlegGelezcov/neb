//Pass manager was actual in OLD version of game
/*
using Common;
using ExitGames.Logging;
using NebulaCommon;
using NebulaCommon.ServerToServer.Events;
using Photon.SocketServer;
using ServerClientCommon;
using System;
using System.Collections;

namespace Login {
    public class PassManager :
        IServer2ServerTransactionHandler<GETInventoryItemTransactionStart, GETInventoryItemTransactionEnd>,
        IServer2ServerTransactionHandler<PUTInventoryItemTransactionStart, PUTInventoryItemTransactionEnd> {

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        private readonly Server2ServerTransactionPool<GETInventoryItemTransactionStart, GETInventoryItemTransactionEnd> mGetTransactionPool;
        private readonly Server2ServerTransactionPool<PUTInventoryItemTransactionStart, PUTInventoryItemTransactionEnd> mPutTransactionPool;
        public LoginApplication application { get; private set; }

        public Server2ServerTransactionPool<GETInventoryItemTransactionStart, GETInventoryItemTransactionEnd> getTransactionPool {
            get {
                return mGetTransactionPool;
            }
        }

        public Server2ServerTransactionPool<PUTInventoryItemTransactionStart, PUTInventoryItemTransactionEnd> putTransactionPool {
            get {
                return mPutTransactionPool;
            }
        }

        public PassManager(LoginApplication app) {
            application = app;
            mGetTransactionPool = new Server2ServerTransactionPool<GETInventoryItemTransactionStart, GETInventoryItemTransactionEnd>(this);
            mPutTransactionPool = new Server2ServerTransactionPool<PUTInventoryItemTransactionStart, PUTInventoryItemTransactionEnd>(this);

        }

        public bool HandleTransaction(GETInventoryItemTransactionStart transactionStart, GETInventoryItemTransactionEnd transactionEnd) {
            if(transactionEnd.success) {
                if(((PostTransactionAction)transactionStart.postTransactionAction) == PostTransactionAction.IncreasePasses) {
                    var dbUser = application.DbUserLogins.GetExistingUserForGameRefOnly(transactionStart.gameRefID);
                    if(dbUser != null ) {
                        dbUser.IncrementPasses();
                        application.DbUserLogins.SaveUser(dbUser);
                        application.LogedInUsers.SendPassesUpdateEvent(dbUser);
                        log.InfoFormat("pass mgr: get inv transaction success");
                        return true;
                    } else {
                        log.InfoFormat("pass mgr: get inv transaction failed user in db not founded [red]");
                    }
                } else {
                    log.InfoFormat("pass mgr: get inv transaction invalid post transaction action [red]");
                }
            } else {
                log.InfoFormat("pass mgr: get inv transaction fail [red]");
            }
            return false;
        }

        public bool HandleTransaction(PUTInventoryItemTransactionStart transactionStart, PUTInventoryItemTransactionEnd transactionEnd) {
            if(transactionEnd.success ) {
                if(((PostTransactionAction)transactionStart.postTransactionAction) == PostTransactionAction.DecreasePasses) {
                    var dbUser = application.DbUserLogins.GetExistingUserForGameRefOnly(transactionStart.gameRefID);
                    if(dbUser != null ) {
                        if(dbUser.passes > 0 ) {
                            dbUser.DecrementPasses();
                            application.DbUserLogins.SaveUser(dbUser);
                            application.LogedInUsers.SendPassesUpdateEvent(dbUser);
                            return true;
                        } else {
                            log.InfoFormat("pass mgr: put inv transaction db user don't has passes [red]...");
                        }
                    } else {
                        log.InfoFormat("pass  mgr: put inv transaction db user not found [red]...");
                    }
                } else {
                    log.InfoFormat("pass mgr: put inv transaction invalid post action [red]");
                }
            } else {
                log.InfoFormat("pass mgr: put inv transaction fail [red]...");
            }
            return false;
        }

        public bool MovePassToInventory(string login, string gameRef, string character, string targetServer) {
            var dbUser = application.DbUserLogins.GetExistingUserForGameRef(login, gameRef);
            if(dbUser != null ) {
                if(dbUser.passes > 0 ) {
                    var objectInfo = CreatePassObjectInfo();
                    PUTInventoryItemTransactionStart start = new PUTInventoryItemTransactionStart {
                        characterID = character,
                        count = 1,
                        gameRefID = gameRef,
                        itemID = objectInfo.GetValue<string>((int)SPC.Id, string.Empty),
                        postTransactionAction = (byte)PostTransactionAction.DecreasePasses,
                        inventoryType = (byte)InventoryType.ship,
                        tag = 0,
                        targetObject = objectInfo,
                        transactionID = Guid.NewGuid().ToString(),
                        transactionSource = (byte)TransactionSource.PassManager,
                         transactionEndServer = targetServer,
                          transactionStartServer = LoginApplication.ServerId.ToString()
                    };
                    EventData eventData = new EventData((byte)S2SEventCode.PUTInventoryItemStart, start);
                    mPutTransactionPool.StartTransaction(start);
                    application.MasterPeer.SendEvent(eventData, new SendParameters());
                    log.InfoFormat("pass put transaction started [red]...");
                    return true;

                } else {
                    log.InfoFormat("user passes is zero [red]");
                    return false;
                }
            } else {
                log.InfoFormat("user not found {0}:{1}:{2} [red]", log, gameRef, character);
                return false;
            }
            
        }

        public bool MoveInventoryItemToPass(string login, string gameRef, string character, string item, string targetServer) {
            var dbUser = application.DbUserLogins.GetExistingUserForGameRef(login, gameRef);
            if(dbUser != null ) {
                GETInventoryItemTransactionStart start = new GETInventoryItemTransactionStart {
                    characterID = character,
                    count = 1,
                    gameRefID = gameRef,
                    inventoryType = (byte)InventoryType.ship,
                    itemID = item,
                    transactionID = Guid.NewGuid().ToString(),
                    transactionSource = (byte)TransactionSource.PassManager,
                    postTransactionAction = (byte)PostTransactionAction.IncreasePasses,
                    tag = 0,
                    transactionEndServer = targetServer,
                    transactionStartServer = LoginApplication.ServerId.ToString()
                };
                EventData eventData = new EventData((byte)S2SEventCode.GETInventoryItemStart, start);
                mGetTransactionPool.StartTransaction(start);
                application.MasterPeer.SendEvent(eventData, new SendParameters());
                log.InfoFormat("move inventory item to pass started");
                return true;
            }
            return false;
        }

        private Hashtable CreatePassObjectInfo() {
            return new Hashtable {
                {(int)SPC.Id, Guid.NewGuid().ToString() },
                {(int)SPC.Name, string.Empty },
                {(int)SPC.Level, 1 },
                {(int)SPC.ItemType, (int)(byte)InventoryObjectType.pass },
                {(int)SPC.Color, (int)(byte)ObjectColor.white },
                {(int)SPC.PlacingType, (int)PlacingType.Inventory },
                {(int)SPC.Binded, false },
                {(int)SPC.Splittable, false }
            };
        }
    }
}
*/