using Common;
using ExitGames.Logging;
using GameMath;
using NebulaCommon;
using NebulaCommon.ServerToServer.Events;
using Photon.SocketServer;
using ServerClientCommon;
using Space.Game.Drop;
using Space.Game.Resources;
using Space.Game.Ship;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SelectCharacter.PvpStore {
    public class PvpStoreManager : IInfoSource,
        IServer2ServerTransactionHandler<PUTInventoryItemTransactionStart, PUTInventoryItemTransactionEnd>  {

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly Server2ServerTransactionPool<PUTInventoryItemTransactionStart, PUTInventoryItemTransactionEnd> mPutTransactionPool;

        public SelectCharacterApplication application { get; private set; }

        public Server2ServerTransactionPool<PUTInventoryItemTransactionStart, PUTInventoryItemTransactionEnd> putTransactionPool {
            get {
                return mPutTransactionPool;
            }
        }

        public PvpStoreManager(SelectCharacterApplication app) {
            application = app;
            mPutTransactionPool = new Server2ServerTransactionPool<PUTInventoryItemTransactionStart, PUTInventoryItemTransactionEnd>(this);
        }

        #region IInfoSource
        public Hashtable GetInfo() {
            return application.pvpStoreItems.GetInfo();
        } 
        #endregion

        #region IServer2ServerTransactionHandler<PUTInventoryItemTransactionStart, PUTInventoryItemTransactionEnd>
        public bool HandleTransaction(PUTInventoryItemTransactionStart transactionStart, PUTInventoryItemTransactionEnd transactionEnd) {
            if(transactionEnd.success ) {
                if(((PostTransactionAction)transactionStart.postTransactionAction) == PostTransactionAction.RemovePvpPoints ) {

                    int count = (int)transactionStart.tag;

                    if(application.Stores.RemovePvpPoints(transactionStart.characterID, count)) {
                        application.Clients.SendGenericEventToGameref(transactionStart.gameRefID, new Events.GenericEvent {
                            subCode = (int)SelectCharacterGenericEventSubCode.PvpStorePurchaseStatus,
                            data = new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.Ok } }
                        });
                        return true;
                    } else {
                        application.Clients.SendGenericEventToGameref(transactionStart.gameRefID, new Events.GenericEvent {
                            subCode = (int)SelectCharacterGenericEventSubCode.PvpStorePurchaseStatus,
                            data = new Hashtable { { (int)SPC.ReturnCode, (int)RPCErrorCode.UnableRemovePvpPointsFromBalance  } }
                        });
                    }
                    
                }
            }
            return false;
        } 
        #endregion

        public bool BuyStoreItem(string login, string gameRef, string character, int race, int inworkshop, int level, string productType, string server, out RPCErrorCode errorCode) {
            errorCode = RPCErrorCode.Ok;

            if(level < application.serverSettings.pvpStoreMinLevel ) {
                log.InfoFormat("player level very low for pvp store {0}:{1} [red]", level, application.serverSettings.pvpStoreMinLevel);
                errorCode = RPCErrorCode.LevelNotEnough;
                return false;
            }

            var storeItem = application.pvpStoreItems.GetItem(productType.ToLower());
            if(storeItem == null ) {
                log.InfoFormat("pvp store item {0} not founded [red]", productType.ToLower());
                errorCode = RPCErrorCode.ObjectNotFound;
                return false;
            }

            var store = application.Stores.GetOnlyPlayerStore(character);

            if(store == null ) {
                log.InfoFormat("player store not founded [red]");
                errorCode = RPCErrorCode.ObjectNotFound;
                return false;
            }

            if(store.pvpPoints < storeItem.price ) {
                log.InfoFormat("player don't enough pvp points for purchase {0}:{1}", store.pvpPoints, storeItem.price);
                errorCode = RPCErrorCode.DontEnoughPvpPoints;
                return false;
            }

            int workshop = inworkshop;
            if(Rand.Float01() >= 0.9f ) {
                workshop = (byte)CommonUtils.RandomWorkshop((Race)(byte)race);
            }

            IInfo result = null;
            if(storeItem.isWeapon) {
                WeaponDropper dropper = new WeaponDropper(new WeaponDropper.WeaponDropParams(application.resource, level, (Workshop)(byte)workshop, WeaponDamageType.damage, Difficulty.none), 1f);
                result = (dropper.DropWeapon() as IInfo);
            } else {
                switch(productType.ToLower()) {
                    case "es":
                        {
                            result = CreateModule(workshop, level, ShipModelSlotType.ES);
                        }
                        break;
                    case "cb":
                        {
                            result = CreateModule(workshop, level, ShipModelSlotType.CB);
                        }
                        break;
                    case "cm":
                        {
                            result = CreateModule(workshop, level, ShipModelSlotType.CM);
                        }
                        break;
                    case "dm":
                        {
                            result = CreateModule(workshop, level, ShipModelSlotType.DM);
                        }
                        break;
                    case "df":
                        {
                            result = CreateModule(workshop, level, ShipModelSlotType.DF);
                        }
                        break;
                }
            }

            if(result == null ) {
                log.InfoFormat("creating item error [red]");
                errorCode = RPCErrorCode.UnknownError;
                return false;
            }

            var itemHash = result.GetInfo();

            PUTInventoryItemTransactionStart start = new PUTInventoryItemTransactionStart {
                characterID = character,
                count = 1,
                gameRefID = gameRef,
                itemID = itemHash.GetValue<string>((int)SPC.Id, string.Empty),
                postTransactionAction = (byte)PostTransactionAction.RemovePvpPoints,
                inventoryType = (byte)InventoryType.ship,
                tag = storeItem.price,
                targetObject = itemHash,
                transactionID = Guid.NewGuid().ToString(),
                transactionSource = (byte)TransactionSource.PvpStore,
                transactionEndServer = server,
                transactionStartServer = SelectCharacterApplication.ServerId.ToString()
            };
            EventData eventData = new EventData((byte)S2SEventCode.PUTInventoryItemStart, start);
            mPutTransactionPool.StartTransaction(start);
            application.MasterPeer.SendEvent(eventData, new SendParameters());
            log.InfoFormat("pass put transaction started [red]...");
            return true;
        }


        private ShipModule CreateModule(int workshop, int level, ShipModelSlotType slotType) {
            ColorInfo color = application.resource.ColorRes.GenColor(Space.Game.Resources.ColoredObjectType.Module, 1);
            string setid = string.Empty;
            if (color.color == ObjectColor.green) {
                var allSets = application.resource.Sets.WorkshopSets((Workshop)(byte)workshop, level);
                if (allSets.Count > 0) {
                    setid = allSets[Rand.Int(0, allSets.Count - 1)].Id;
                }
            }
            ModuleDropper mdropper = new ModuleDropper(new ModuleDropper.ModuleDropParams(application.resource, (Workshop)(byte)workshop,
                slotType, level, Difficulty.none, new Dictionary<string, int>(), color.color,
                setid));
            return mdropper.DropModule();
        }
    }
}
