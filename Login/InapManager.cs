using Common;
using ExitGames.Logging;
using Login.Events;
using Nebula.Inventory.Objects;
using Nebula.Resources.Inaps;
using Nebula.Server.Login;
using NebulaCommon;
using NebulaCommon.ServerToServer.Events;
using Photon.SocketServer;
using ServerClientCommon;
using System;
using System.Collections;

namespace Login {

    /// <summary>
    /// Handle inap transactions
    /// </summary>
    public class InapManager : IServer2ServerTransactionHandler<PUTInventoryItemTransactionStart, PUTInventoryItemTransactionEnd> {

        private static readonly ILogger s_Log = LogManager.GetCurrentClassLogger();

        private readonly Server2ServerTransactionPool<PUTInventoryItemTransactionStart, PUTInventoryItemTransactionEnd> m_PutTransactionPool;
        private readonly LoginApplication m_Application;

        public Server2ServerTransactionPool<PUTInventoryItemTransactionStart, PUTInventoryItemTransactionEnd> putTransactionPool {
            get {
                return m_PutTransactionPool;
            }
        }

        public InapManager(LoginApplication app) {
            m_Application = app;
            m_PutTransactionPool = new Server2ServerTransactionPool<PUTInventoryItemTransactionStart, PUTInventoryItemTransactionEnd>(this);
        }


        #region Interface IServer2ServerTransactionHandler<PUTInventoryItemTransactionStart, PUTInventoryItemTransactionEnd>
        public bool HandleTransaction(PUTInventoryItemTransactionStart transactionStart, PUTInventoryItemTransactionEnd transactionEnd) {

            Hashtable tagHash = transactionStart.tag as Hashtable;
            string inapId = tagHash.GetValue<string>((int)SPC.InapId, string.Empty);
            int price = tagHash.GetValue<int>((int)SPC.Price, 0);

            if (!transactionEnd.success) {
                SendInapStatusUpdate(transactionStart.gameRefID, inapId, price, false);
                return false;
            }

            switch((PostTransactionAction)transactionStart.postTransactionAction) {
                case PostTransactionAction.RemoveNebulaCredits: {
                        var databaseUser = m_Application.GetUser(new GameRefId(transactionStart.gameRefID));
                        if(databaseUser == null ) {
                            SendInapStatusUpdate(transactionStart.gameRefID, inapId, price, false);
                            return false;
                        }

                        databaseUser.RemoveNebulaCredits(price);
                        m_Application.SaveUser(databaseUser);
                        SendInapStatusUpdate(transactionStart.gameRefID, inapId, price, true);
                        SendNebulaCreditsUpdateEvent(transactionStart.gameRefID, databaseUser.nebulaCredits);
                        return true;
                    }
                    break;
            }
            return false;
        } 

        private void SendInapStatusUpdate(string gameRef, string inapId, int price, bool success ) {
            m_Application.LogedInUsers.SendEvent(new GameRefId(gameRef), new EventData(
                (byte)LoginEventCode.InapStatusUpdate,
                new InapStatusUpdateEvent {
                    inapId = inapId,
                    price = price,
                    success = success
                }));
        }

        private void SendNebulaCreditsUpdateEvent(string gameRef, int nebulaCredits) {
            m_Application.LogedInUsers.SendEvent(new GameRefId(gameRef), new EventData (
                (byte)LoginEventCode.NebulaCreditsUpdate,
                new NebulaCreditsUpdateEvent {
                     nebulaCredits = nebulaCredits 
                }
            ));
        }
        #endregion

        public bool RequestPurchaseInap(string gameRef, string character, string inapId, string targetServer, out ReturnCode code) {

            var databaseUser = m_Application.GetUser(new GameRefId(gameRef));

            if(databaseUser == null ) {
                code = ReturnCode.UserNotFound;
                return false;
            }

            InapItem inapItem = m_Application.inapResource.GetInap(inapId);
            if(inapItem == null ) {
                code = ReturnCode.ResourceNotFound;
                return false;
            }

            if(databaseUser.nebulaCredits < inapItem.price) {
                code = ReturnCode.NotEnoughCredits;
                return false;
            }

            s_Log.InfoFormat("request purchase inap = {0} with interval = {1}", inapId, inapItem.data.GetValue<int>("interval", 0));

            Hashtable objInfo = null;
            switch(inapItem.type) {
                case InapObjectType.exp_boost: {
                        objInfo = new ExpBoostObject(
                            inapId + inapItem.tag.ToString(),
                            inapItem.data.GetValue<float>("value", 0f),
                            inapItem.data.GetValue<int>("interval", 0),
                            inapItem.tag
                            ).GetInfo();
                    }
                    break;
                case InapObjectType.loot_box: {
                        objInfo = new LootBoxObject(
                            inapId, 
                            inapItem.data.GetValue<string>("drop_list", string.Empty)
                        ).GetInfo();
                    }
                    break;
                case InapObjectType.pet_skin: {
                        string skin = inapItem.data.GetValue<string>("model", string.Empty);
                        if(string.IsNullOrEmpty(skin)) {
                            code = ReturnCode.InvalidInapType;
                            return false;
                        }
                        objInfo = new PetSkinObject(skin, skin).GetInfo();
                    }
                    break;
                case InapObjectType.founder_cube: {
                        objInfo = new FounderCubeInventoryObject().GetInfo();
                    }
                    break;
                case InapObjectType.credits_bag: {
                        objInfo = new CreditsBagObject("creditsbag", inapItem.data.GetValue<int>("count", 10000), false).GetInfo();
                    }
                    break;

            }

            if(objInfo == null ) {
                code = ReturnCode.InvalidInapType;
                return false;
            }

            Hashtable tagHash = new Hashtable {
                { (int)SPC.InapId, inapId },
                { (int)SPC.Price, inapItem.price },
                { (int)SPC.Title, "ginp_title" },
                { (int)SPC.Body, "ginp_body" }
            };

            string itemID = string.Empty;
            if(inapItem.type == InapObjectType.pet_skin) {
                itemID = objInfo.GetValue<string>((int)SPC.Id, string.Empty);
            } else if(inapItem.type == InapObjectType.founder_cube) {
                itemID = objInfo.GetValue<string>((int)SPC.Id, string.Empty);
            } else if(inapItem.type == InapObjectType.credits_bag) {
                itemID = "creditsbag";
            }
            else {
                itemID = inapId + inapItem.tag.ToString();
            }

            PUTInventoryItemTransactionStart startTransaction = new PUTInventoryItemTransactionStart {
                characterID = character,
                gameRefID = gameRef,
                count = 1,
                inventoryType = (int)InventoryType.ship,
                itemID = itemID,
                postTransactionAction = (byte)PostTransactionAction.RemoveNebulaCredits,
                tag = tagHash,
                targetObject = objInfo,
                transactionEndServer = targetServer,
                transactionStartServer = LoginApplication.ServerId.ToString(),
                transactionID = Guid.NewGuid().ToString(),
                transactionSource = (byte)TransactionSource.Inaps
            };

            EventData eventData = new EventData((byte)S2SEventCode.PUTMailTransactionStart, startTransaction);
            m_PutTransactionPool.StartTransaction(startTransaction);
            m_Application.MasterPeer.SendEvent(eventData, new SendParameters());
            s_Log.InfoFormat("inap transaction successfully started...");

            code = ReturnCode.Ok;
            return true;
        }
    }
}
