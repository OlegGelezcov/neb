using Common;
using ExitGames.Logging;
using Nebula.Drop;
using Nebula.Engine;
using Nebula.Game.Components;
using Nebula.Game.Components.Quests;
using Nebula.Game.Components.Quests.Dialogs;
using Nebula.Inventory.Objects;
using Nebula.Quests;
using Nebula.Server.Operations;
using Photon.SocketServer;
using ServerClientCommon;
using Space.Game;
using Space.Game.Inventory;
using System.Collections;

namespace Nebula.Game.OperationHandlers {
    public class RPCInvokeOperationHandler : BasePlayerOperationHandler {

        private static readonly ILogger s_Log = LogManager.GetCurrentClassLogger();

        private const float LORE_BOX_ACTION_DISTANCE = 70;

        public override OperationResponse Handle(MmoActor actor, OperationRequest request, SendParameters sendParameters) {
            var operation = new RPCInvokeOperation(actor.Peer.Protocol, request);
            if(!operation.IsValid) {
                return new OperationResponse(request.OperationCode) {
                    ReturnCode = (int)ReturnCode.InvalidOperationParameter,
                    DebugMessage = operation.GetErrorMessage()
                };
            }

            switch(operation.rpcId) {
                case RPCID.rpc_ProposeContract:
                    return CallProposeContract(actor, request, operation);
                case RPCID.rpc_RestartLoop:
                    return CallRestartLoop(actor, request, operation);
                case RPCID.rpc_AcceptContract:
                    return CallAcceptContract(actor, request, operation);
                case RPCID.rpc_DeclineContract:
                    return CallDeclineContract(actor, request, operation);
                case RPCID.rpc_CompleteContract:
                    return CallCompleteContract(actor, request, operation);
                case RPCID.rpc_TestAddContractItems:
                    return CallTestAddContractItems(actor, request, operation);
                case RPCID.rpc_TestRemoveContractItems:
                    return CallTestRemoveContractItems(actor, request, operation);
                case RPCID.rpc_GetAchievments:
                    return CallGetAchievments(actor, request, operation);
                case RPCID.rpc_GetParamDetail:
                    return CallGetParamDetail(actor, request, operation);
                case RPCID.rpc_SetPlayerMark:
                    return CallSetPlayerMark(actor, request, operation);
                case RPCID.rpc_ResetNew:
                    return CallResetNew(actor, request, operation);
                case RPCID.rpc_TestKill:
                    return CallTestKill(actor, request, operation);
                case RPCID.rpc_UnlockLore:
                    return CallUnlockLore(actor, request, operation);
                case RPCID.rpc_TestUnlockFullLore:
                    return CallUnlockAllLore(actor, request, operation);
                case RPCID.rpc_StartAsteroidCollecting:
                    return CallStartAsteroidCollecting(actor, request, operation);
                case RPCID.rpc_ForceDispose:
                    return CallForceDispose(actor, request, operation);
                case RPCID.rpc_UseCreditsBag:
                    return CallUseCreditsBag(actor, request, operation);
                case RPCID.rpc_CreateCommandCenter:
                    return CallCreateCommandCenter(actor, request, operation);
                case RPCID.rpc_CreatePlanetObjectTurret:
                    return CallCreatePlanetTurret(actor, request, operation);
                case RPCID.rpc_CreatePlanetObjectResourceHangar:
                    return CallCreatePlanetResourceHangar(actor, request, operation);
                case RPCID.rpc_CreatePlanetObjectResourceAccelerator:
                    return CallCreatePlanetResourceAccelerator(actor, request, operation);
                case RPCID.rpc_CreatePlanetObjectMiningStation:
                    return CallCreatePlanetMiningStation(actor, request, operation);
                case RPCID.rpc_GetCells:
                    return CallGetCells(actor, request, operation);
                case RPCID.rpc_resetSystemToNeutral:
                    return CallResetSystemToNeutral(actor, request, operation);
                case RPCID.rpc_CollectOreFromPlanetMiningStation:
                    return CallCollectOreFromPlanetMiningStation(actor, request, operation);
                case RPCID.rpc_CreateTestSharedChest:
                    return CallCreateTestSharedChest(actor, request, operation);
                case RPCID.rpc_MoveAllFromInventoryToStationWithExclude:
                    return CallMoveAllFromInventoryToStationWithExclude(actor, request, operation);
                case RPCID.rpc_TestStun:
                    return CallTestStun(actor, request, operation);
                case RPCID.rpc_TestAreaInvisibility:
                    return CallTestAreaInvisibilty(actor, request, operation);
                case RPCID.rpc_GetQuests:
                    return CallGetQuests(actor, request, operation);
                case RPCID.rpc_CompleteQuest:
                    return CallCompleteQuest(actor, request, operation);
                case RPCID.rpc_GetDialogs:
                    return CallGetDialogs(actor, request, operation);
                case RPCID.rpc_UserEvent:
                    return CallUserEvent(actor, request, operation);
                case RPCID.rpc_ResetQuests:
                    return CallResetQuests(actor, request, operation);
                default:
                    return new OperationResponse(request.OperationCode) {
                        ReturnCode = (int)ReturnCode.InvalidRPCID,
                        DebugMessage = string.Format("not found rpc with id = {0}", operation.rpcId)
                    };
            }
        }

        private OperationResponse CallResetQuests(MmoActor player, OperationRequest request, RPCInvokeOperation op) {
            player.GetComponent<QuestManager>().Reset();
            player.GetComponent<DialogManager>().Reset();
            RPCInvokeResponse respInstance = new RPCInvokeResponse {
                rpcId = op.rpcId,
                result = (int)RPCErrorCode.Ok
            };
            return new OperationResponse(request.OperationCode, respInstance);
        }

        private OperationResponse CallUserEvent(MmoActor player, OperationRequest request, RPCInvokeOperation op) {
            if(op.parameters != null && op.parameters.Length > 0 ) {
                RPCErrorCode code = RPCErrorCode.Ok;
                UserEventName eventName = (UserEventName)(int)op.parameters[0];
                switch(eventName) {
                    case UserEventName.object_scanner_select_ship:
                    case UserEventName.start_moving:
                    case UserEventName.rotate_camera: {
                            if (player.GetComponent<QuestManager>().TryCheckActiveQuests(new UserEvent(eventName))) {
                                s_Log.InfoFormat("player complete some quest with event: {0}".Lime(), eventName);
                            } else {
                                s_Log.InfoFormat("no quest completed by event: {0}".Orange(), eventName);
                            }
                        }
                        break;
                    case UserEventName.dialog_completed: {
                            if(op.parameters.Length > 1) {
                                string dialogId = (string)op.parameters[1];
                                player.GetComponent<DialogManager>().CompleteDialog(dialogId);
                            } else {
                                code = RPCErrorCode.MissedParameter;
                            }
                        }
                        break;
                    default: {
                            s_Log.Info(string.Format("error: no support for event: {0}", eventName).Red());
                            code = RPCErrorCode.UnsupportedEvent;
                        }
                        break;
                }

                RPCInvokeResponse respInstance = new RPCInvokeResponse {
                    rpcId = op.rpcId,
                    result = (int)code
                };
                return new OperationResponse(request.OperationCode, respInstance);
            }
            return InvalidOperationParameter(request);
        }

        private OperationResponse CallGetDialogs(MmoActor player, OperationRequest request, RPCInvokeOperation op ) {
            RPCInvokeResponse respInstance = new RPCInvokeResponse {
                rpcId = op.rpcId,
                result = (int)ReturnCode.Ok
            };
            player.GetComponent<DialogManager>().SendInfo();
            return new OperationResponse(request.OperationCode, respInstance);
        }

        private OperationResponse CallCompleteQuest(MmoActor player, OperationRequest request, RPCInvokeOperation op ) {
            if(op.parameters != null && op.parameters.Length > 0 ) {
                string questId = (string)op.parameters[0];
                bool status = player.GetComponent<QuestManager>().CompleteReadyQuest(questId);
                RPCInvokeResponse respInstance = new RPCInvokeResponse {
                    rpcId = op.rpcId,
                    result = status
                };
                return new OperationResponse(request.OperationCode, respInstance);
            }
            return InvalidOperationParameter(request);
        }

        private OperationResponse CallGetQuests(MmoActor player, OperationRequest request, RPCInvokeOperation op ) {
            RPCInvokeResponse respInstance = new RPCInvokeResponse {
                rpcId = op.rpcId,
                result = (int)ReturnCode.Ok
            };
            player.GetComponent<QuestManager>().SendInfo();
            return new OperationResponse(request.OperationCode, respInstance);
        }

        private OperationResponse CallTestAreaInvisibilty(MmoActor player, OperationRequest request, RPCInvokeOperation op) {
            if(op.parameters != null && op.parameters.Length > 0 ) {
                float radius = (float)op.parameters[0];
                RPCInvokeResponse respInstance = new RPCInvokeResponse {
                    rpcId = op.rpcId,
                    result = player.ActionExecutor.TestSetAreaInvisibility(radius)
                };
                return new OperationResponse(request.OperationCode, respInstance);
            }
            return InvalidOperationParameter(request);
        }

        private OperationResponse CallTestStun(MmoActor player, OperationRequest request, RPCInvokeOperation op ) {
            RPCInvokeResponse respInstance = new RPCInvokeResponse {
                rpcId = op.rpcId,
                result = player.ActionExecutor.TestStun()
            };
            return new OperationResponse(request.OperationCode, respInstance);
        }

        private OperationResponse CallMoveAllFromInventoryToStationWithExclude(MmoActor player, OperationRequest request, RPCInvokeOperation op ) {
            if(op.parameters != null && op.parameters.Length > 0 ) {
                string[] excludeItems = (string[])op.parameters[0];
                Hashtable hash = player.ActionExecutor.MoveAllFromInventoryToStationWithExclude(excludeItems);
                RPCInvokeResponse respInstance = new RPCInvokeResponse {
                    rpcId = op.rpcId,
                    result = hash
                };
                return new OperationResponse(request.OperationCode, respInstance);
            }
            return InvalidOperationParameter(request);
        }

        private OperationResponse CallCreateTestSharedChest(MmoActor player, OperationRequest request, RPCInvokeOperation op ) {
            RPCInvokeResponse responseInstance = new RPCInvokeResponse {
                rpcId = op.rpcId,
                result = player.ActionExecutor.CreateTestSharedChest()
            };
            return new OperationResponse(request.OperationCode, responseInstance);
        }

        private OperationResponse CallCollectOreFromPlanetMiningStation(MmoActor player, OperationRequest request, RPCInvokeOperation op ) {
            if(op.parameters != null && op.parameters.Length > 0 ) {
                string itemId = (string)op.parameters[0];
                Hashtable hash = player.ActionExecutor.CollectOreFromPlanetMiningStation(itemId);
                RPCInvokeResponse respInstance = new RPCInvokeResponse {
                    rpcId = op.rpcId,
                    result = hash
                };
                return new OperationResponse(request.OperationCode, respInstance);
            }
            return InvalidOperationParameter(request);
        }

        private OperationResponse CallResetSystemToNeutral(MmoActor player, OperationRequest request, RPCInvokeOperation op ) {

            var forts = (player.World as MmoWorld).FindObjectsOfType<Outpost>();
            foreach(var kf in forts ) {
                (kf.Value.nebulaObject as GameObject).Destroy();
            }
            s_Log.InfoFormat("removed: {0} fortifications", forts.Count);

            var outposts = (player.World as MmoWorld).FindObjectsOfType<MainOutpost>();
            foreach(var kf in outposts ) {
                (kf.Value.nebulaObject as GameObject).Destroy();
            }
            s_Log.InfoFormat("removed: {0} outposts", outposts.Count);

            forts.Clear();
            outposts.Clear();

            (player.World as MmoWorld).SetUnderAttack(false);
            (player.World as MmoWorld).SetCurrentRace(Race.None);
            RPCInvokeResponse respInstance = new RPCInvokeResponse {
                 rpcId = op.rpcId,
                 result = new Hashtable {
                     { (int)SPC.ReturnCode, (int)RPCErrorCode.Ok }
                 }
            };
            return new OperationResponse(request.OperationCode, respInstance);
        }

        private OperationResponse CallGetCells(MmoActor player, OperationRequest request, RPCInvokeOperation op ) {
            RPCInvokeResponse respInstance = new RPCInvokeResponse {
                rpcId = op.rpcId,
                result = player.nebulaObject.mmoWorld().GetCellInfo()
            };
            return new OperationResponse(request.OperationCode, respInstance);
        }

        private OperationResponse CallCreatePlanetMiningStation(MmoActor player, OperationRequest request, RPCInvokeOperation op) {
            if(op.parameters != null && op.parameters.Length >= 3 ) {
                int row = (int)op.parameters[0];
                int column = (int)op.parameters[1];
                string itemId = (string)op.parameters[2];
                Hashtable resp = player.ActionExecutor.CreatePlanetObjectMiningStation(row, column, itemId);
                RPCInvokeResponse respInstance = new RPCInvokeResponse {
                    rpcId = op.rpcId,
                    result = resp
                };
                return new OperationResponse(request.OperationCode, respInstance);
            }
            return InvalidOperationParameter(request);
        }
        private OperationResponse CallCreatePlanetResourceAccelerator(MmoActor player, OperationRequest request, RPCInvokeOperation op) {
            if(op.parameters != null && op.parameters.Length >= 3 ) {
                int row = (int)op.parameters[0];
                int column = (int)op.parameters[1];
                string itemId = (string)op.parameters[2];
                Hashtable resp = player.ActionExecutor.CreatePlanetObjectResourceAccelerator(row, column, itemId);
                RPCInvokeResponse respInstance = new RPCInvokeResponse {
                    rpcId = op.rpcId,
                    result = resp
                };
                return new OperationResponse(request.OperationCode, respInstance);
            }
            return InvalidOperationParameter(request);
        }

        private OperationResponse CallCreatePlanetResourceHangar(MmoActor player, OperationRequest request, RPCInvokeOperation op) {
            if(op.parameters != null && op.parameters.Length >= 3 ) {
                int row = (int)op.parameters[0];
                int column = (int)op.parameters[1];
                string itemId = (string)op.parameters[2];
                Hashtable resp = player.ActionExecutor.CreatePlanetObjectResourceHangar(row, column, itemId);
                RPCInvokeResponse respInstance = new RPCInvokeResponse {
                    rpcId = op.rpcId,
                    result = resp
                };
                return new OperationResponse(request.OperationCode, respInstance);
            }
            return InvalidOperationParameter(request);
        }

        private OperationResponse CallCreatePlanetTurret(MmoActor player, OperationRequest request, RPCInvokeOperation op ) {
            if(op.parameters != null && op.parameters.Length >= 3 ) {
                int row = (int)op.parameters[0];
                int column = (int)op.parameters[1];
                string itemId = (string)op.parameters[2];
                Hashtable resp = player.ActionExecutor.CreatePlanetObjectTurret(row, column, itemId);
                RPCInvokeResponse respInstance = new RPCInvokeResponse {
                    rpcId = op.rpcId,
                    result = resp
                };
                return new OperationResponse(request.OperationCode, respInstance);
            }
            return InvalidOperationParameter(request);
        }

        private OperationResponse CallCreateCommandCenter(MmoActor player, OperationRequest request, RPCInvokeOperation op) {
            if(op.parameters != null && op.parameters.Length >= 3) {
                int row = (int)op.parameters[0];
                int column = (int)op.parameters[1];
                string itemId = (string)op.parameters[2];
                Hashtable resp = player.ActionExecutor.CreatePlanetObjectCommandCenter(row, column, itemId);
                RPCInvokeResponse respInstance = new RPCInvokeResponse {
                    rpcId = op.rpcId,
                    result = resp
                };
                return new OperationResponse(request.OperationCode, respInstance);
            }
            return InvalidOperationParameter(request);
        }

        private OperationResponse CallUseCreditsBag(MmoActor player, OperationRequest request, RPCInvokeOperation op ) {
            if(op.parameters != null && op.parameters.Length >= 1) {
                string itemId = (string)op.parameters[0];
                ServerInventoryItem creditsBagItem;
                if(player.Station.StationInventory.TryGetItem(InventoryObjectType.credits_bag, itemId, out creditsBagItem ) ) {
                    if(creditsBagItem.Count > 0  ) {
                        int count = (creditsBagItem.Object as CreditsBagObject).count;
                        player.ActionExecutor.AddCredits(count);
                        player.Station.StationInventory.Remove(InventoryObjectType.credits_bag, itemId, 1);
                        player.EventOnStationHoldUpdated();
                        RPCInvokeResponse respInstance = new RPCInvokeResponse {
                            rpcId = op.rpcId,
                            result = new Hashtable {
                                  {(int)SPC.ReturnCode, (int)RPCErrorCode.Ok },
                                  {(int)SPC.Count, count }
                              }
                        };
                        return new OperationResponse(request.OperationCode, respInstance);
                    } else {
                        return ErrorResponse(request, op.rpcId, RPCErrorCode.CountIsZero);
                    }
                } else {
                    return ErrorResponse(request, op.rpcId, RPCErrorCode.ItemNotFound);
                }
            }
            return InvalidOperationParameter(request);
        }

        private OperationResponse CallForceDispose(MmoActor player, OperationRequest request, RPCInvokeOperation op ) {
            player.SetForceDispose();
            RPCInvokeResponse responseInstance = new RPCInvokeResponse {
                rpcId = op.rpcId,
                result = new Hashtable {
                    {(int)SPC.ReturnCode, (int)RPCErrorCode.Ok }
                }
            };
            return new OperationResponse(request.OperationCode, responseInstance);
        }

        /// <summary>
        /// Handle start collecting asteroid action. Receive request from client and send StartCollectAsteroid generic event to all subscribers
        /// </summary>
        private OperationResponse CallStartAsteroidCollecting(MmoActor player, OperationRequest request, RPCInvokeOperation op) {
            if(op.parameters != null && op.parameters.Length >= 2 ) {
                byte containerType = (byte)op.parameters[0];
                string containerId = (string)op.parameters[1];
                var mmoComponent = player.GetComponent<MmoMessageComponent>();
                if(mmoComponent ) {
                    mmoComponent.PublishStartAsteroidCollecting(containerType, containerId);
                }
                RPCInvokeResponse responseInstance = new RPCInvokeResponse {
                    rpcId = op.rpcId,
                    result = new Hashtable {
                        { (int)SPC.ReturnCode, (int)RPCErrorCode.Ok }
                    }
                };
                return new OperationResponse(request.OperationCode, responseInstance);
            } else {
                return InvalidOperationParameter(request);
            }
        }

        /// <summary>
        /// Unlock all existing lore operation
        /// </summary>
        private OperationResponse CallUnlockAllLore(MmoActor player, OperationRequest request, RPCInvokeOperation op) {
            string[] humanStory = new string[] { "st_0_rec_0", "st_0_rec_1", "st_0_rec_2", "st_0_rec_3", "st_0_rec_4", "st_0_rec_5",
                "st_0_rec_6", "st_0_rec_7", "st_0_rec_8", "st_0_rec_9", "st_0_rec_10", "st_0_rec_11", "st_0_rec_12", "st_0_rec_13",
                "st_0_rec_14", "st_0_rec_15", "st_0_rec_16", "st_0_rec_17", "st_0_rec_18", "st_0_rec_19"};
            string[] criptizidStory = new string[] { "st_1_rec_0", "st_1_rec_1", "st_1_rec_2", "st_1_rec_3", "st_1_rec_4", "st_1_rec_5", "st_1_rec_6",
                "st_1_rec_7", "st_1_rec_8", "st_1_rec_9", "st_1_rec_10", "st_1_rec_11", "st_1_rec_12", "st_1_rec_13", "st_1_rec_14",
                "st_1_rec_15", "st_1_rec_16", "st_1_rec_17", "st_1_rec_18", "st_1_rec_19" };
            string[] borgStory = new string[] { "st_2_rec_0", "st_2_rec_1", "st_2_rec_2", "st_2_rec_3", "st_2_rec_4",
                "st_2_rec_5", "st_2_rec_6", "st_2_rec_7", "st_2_rec_8", "st_2_rec_9"};

            var achievmentComponent = player.GetComponent<AchievmentComponent>();
            foreach(string id in humanStory ) {
                achievmentComponent.FoundLoreRecord(id);
            }
            foreach(string id in criptizidStory ) {
                achievmentComponent.FoundLoreRecord(id);
            }
            foreach(string id in borgStory ) {
                achievmentComponent.FoundLoreRecord(id);
            }
            RPCInvokeResponse responseInstance = new RPCInvokeResponse {
                rpcId = op.rpcId
            };

            responseInstance.result = new Hashtable {
                                    { (int)SPC.ReturnCode, (int)RPCErrorCode.LoreRecordAlreadyUnlocked }
                                };
            return new OperationResponse(request.OperationCode, responseInstance);
        }

        private OperationResponse CallUnlockLore(MmoActor player, OperationRequest request, RPCInvokeOperation op ) {

            if(op.parameters != null && op.parameters.Length >= 1) {

                string loreBoxId = (string)op.parameters[0];
                NebulaObject loreBoxObject;
                RPCInvokeResponse responseInstance = new RPCInvokeResponse {
                    rpcId = op.rpcId
                };

                if(player.nebulaObject.mmoWorld().TryGetObject((byte)ItemType.Bot, loreBoxId, out loreBoxObject)) {

                    float distance = player.transform.DistanceTo(loreBoxObject.transform);
                    if(distance <= LORE_BOX_ACTION_DISTANCE ) {

                        var loreBoxComponent = loreBoxObject.GetComponent<LoreBoxComponent>();

                        if(loreBoxComponent != null ) {
                            if (player.GetComponent<AchievmentComponent>().FoundLoreRecord(loreBoxComponent.recordId)) {

                                (loreBoxObject as GameObject).Destroy();

                                responseInstance.result = new Hashtable {
                                    { (int)SPC.ReturnCode, (int)RPCErrorCode.Ok }
                                };

                            } else {
                                responseInstance.result = new Hashtable {
                                    { (int)SPC.ReturnCode, (int)RPCErrorCode.LoreRecordAlreadyUnlocked }
                                };
                            }
                        } else {
                            responseInstance.result = new Hashtable {
                                { (int)SPC.ReturnCode, (int)RPCErrorCode.ComponentNotFound }
                            };
                        }
                    } else {
                        responseInstance.result = new Hashtable {
                            { (int)SPC.ReturnCode, (int)RPCErrorCode.DistanceIsFar }
                        };
                    }
                } else {
                    responseInstance.result = new Hashtable {
                        { (int)SPC.ReturnCode, (int)RPCErrorCode.ItemNotFound },
                    };
                }

                return new OperationResponse(request.OperationCode, responseInstance);
            } else {
                return InvalidOperationParameter(request);
            }
        }

        private OperationResponse CallTestKill(MmoActor player, OperationRequest request, RPCInvokeOperation op) {

            bool success = false;
            var targetComponent = player.GetComponent<PlayerTarget>();
            if(targetComponent.targetObject != null ) {
                var targetDamagableComponent = targetComponent.targetObject.GetComponent<DamagableObject>();
                if(targetDamagableComponent != null ) {
                    WeaponDamage weaponDamage = new WeaponDamage(WeaponBaseType.Rocket, 100000, 0, 0);
                    DamageParams damageParams = new DamageParams();
                    damageParams.SetReflrected(false);
                    damageParams.SetIgnoreFixedDamage(true);
                    targetDamagableComponent.ReceiveDamage(new InputDamage(player.nebulaObject, weaponDamage, damageParams));
                    success = true;
                }
            }
            RPCInvokeResponse responseInstance = new RPCInvokeResponse {
                rpcId = op.rpcId,
                result = success
            };
            return new OperationResponse(request.OperationCode, responseInstance);
        }

        private OperationResponse CallResetNew(MmoActor player, OperationRequest request, RPCInvokeOperation op ) {
            if(op.parameters != null && op.parameters.Length >= 1) {
                InventoryType inventoryType = (InventoryType)(byte)op.parameters[0];
                bool success = true;
                if(inventoryType == InventoryType.ship) {
                    player.Inventory.ResetNew();
                } else if(inventoryType == InventoryType.station) {
                    player.Station.StationInventory.ResetNew();
                } else {
                    success = false;
                }
                RPCInvokeResponse responseInstance = new RPCInvokeResponse {
                    rpcId = op.rpcId,
                    result = success
                };
                return new OperationResponse(request.OperationCode, responseInstance);
            }
            return InvalidOperationParameter(request);
        }

        private OperationResponse CallSetPlayerMark(MmoActor player, OperationRequest request, RPCInvokeOperation op) {
            if(op.parameters != null && op.parameters.Length >= 2 ) {
                string id = (string)op.parameters[0];
                byte type = (byte)op.parameters[1];
                var targetComponent = player.GetComponent<PlayerTarget>();
                if(targetComponent != null ) {
                    if(string.IsNullOrEmpty(id)) {
                        targetComponent.ClearMarkedItem();
                    } else {
                        targetComponent.SetMarkedItem(id, type);
                    }
                    RPCInvokeResponse responseInstance = new RPCInvokeResponse {
                        rpcId = op.rpcId,
                        result = true
                    };
                    return new OperationResponse(request.OperationCode, responseInstance);
                }
            }
            return InvalidOperationParameter(request);
        }

        private OperationResponse CallGetParamDetail(MmoActor player, OperationRequest request, RPCInvokeOperation op) {
            if (op.parameters != null && op.parameters.Length >= 1) {
                int parameterName = (int)op.parameters[0];
                RPCInvokeResponse responseInstance = null;
                switch (parameterName) {
                    case ShipParamName.speed: {
                            var movable = player.GetComponent<PlayerShipMovable>();
                            Hashtable speedDetailHash = null;
                            if (movable != null) {
                                speedDetailHash = movable.GetSpeedDetail();
                            } else {
                                speedDetailHash = new Hashtable();
                            }
                            speedDetailHash.Add((int)SPC.ShipParamName, ShipParamName.speed);
                            responseInstance = new RPCInvokeResponse {
                                rpcId = op.rpcId,
                                result = speedDetailHash
                            };
                        }
                        break;
                    case ShipParamName.resist: {
                            var playerShip = player.GetComponent<PlayerShip>();
                            Hashtable resistHash = null;
                            if(playerShip != null ) {
                                resistHash = playerShip.GetResistanceDetail();
                            } else {
                                resistHash = new Hashtable();
                            }
                            resistHash.Add((int)SPC.ShipParamName, ShipParamName.resist);
                            responseInstance = new RPCInvokeResponse {
                                rpcId = op.rpcId,
                                result = resistHash
                            };
                        }
                        break;
                    default:
                        return InvalidOperationParameter(request);
                }

                return new OperationResponse(request.OperationCode, responseInstance);
            }
            return InvalidOperationParameter(request);
        }

        private OperationResponse CallTestRemoveContractItems(MmoActor player, OperationRequest request, RPCInvokeOperation op) {
            int result = player.ActionExecutor.TestRemoveContractItems();
            RPCInvokeResponse responseInstance = new RPCInvokeResponse {
                 rpcId = op.rpcId,
                 result = result
            };
            return new OperationResponse(request.OperationCode, responseInstance);
        }

        private OperationResponse CallTestAddContractItems(MmoActor player, OperationRequest request, RPCInvokeOperation op) {
            int result = player.ActionExecutor.TestAddContractItems();
            RPCInvokeResponse responseInstance = new RPCInvokeResponse {
                rpcId = op.rpcId,
                result = result
            };
            return new OperationResponse(request.OperationCode, responseInstance);
        }

        private OperationResponse CallGetAchievments(MmoActor player, OperationRequest request, RPCInvokeOperation op) {
            Hashtable hash = player.ActionExecutor.GetAchievmentInfo();
            RPCInvokeResponse responseInstance = new RPCInvokeResponse {
                rpcId = op.rpcId,
                result = hash
            };
            return new OperationResponse(request.OperationCode, responseInstance);
        }

        private OperationResponse CallCompleteContract(MmoActor player, OperationRequest request, RPCInvokeOperation op) {
            if(op.parameters != null && op.parameters.Length >= 1) {
                string contractId = op.parameters[0] as string;
                Hashtable ret = player.ActionExecutor.CompleteContract(contractId);
                RPCInvokeResponse responseInstance = new RPCInvokeResponse {
                    rpcId = op.rpcId,
                    result = ret
                };
                return new OperationResponse(request.OperationCode, responseInstance);
            }
            return InvalidOperationParameter(request);
        }

        private OperationResponse CallDeclineContract(MmoActor player, OperationRequest request, RPCInvokeOperation op) {
            if(op.parameters != null && op.parameters.Length >= 1) {
                string contractId = op.parameters[0] as string;
                Hashtable ret = player.ActionExecutor.DeclineContract(contractId);
                RPCInvokeResponse responseInstance = new RPCInvokeResponse {
                    rpcId = op.rpcId,
                    result = ret
                };
                return new OperationResponse(request.OperationCode, responseInstance);
            }
            return InvalidOperationParameter(request);
        }

        private OperationResponse CallAcceptContract(MmoActor player, OperationRequest request, RPCInvokeOperation op) {
            if(op.parameters != null && op.parameters.Length >= 1 ) {
                string contractId = op.parameters[0] as string;
                Hashtable ret = player.ActionExecutor.AcceptContract(contractId);
                RPCInvokeResponse responseInstance = new RPCInvokeResponse {
                    rpcId = op.rpcId,
                    result = ret
                };
                return new OperationResponse(request.OperationCode, responseInstance);
            }
            return InvalidOperationParameter(request);
        }

        private OperationResponse CallRestartLoop(MmoActor player, OperationRequest request, RPCInvokeOperation op) {
            int result = player.ActionExecutor.RestartLoop();
            RPCInvokeResponse r = new RPCInvokeResponse { rpcId = op.rpcId, result = result };
            return new OperationResponse(request.OperationCode, r);
        }

        private OperationResponse CallProposeContract(MmoActor player, OperationRequest request, RPCInvokeOperation op) {
            if(op.parameters != null && op.parameters.Length >= 1) {
                int category = (int)op.parameters[0];
                Hashtable result = player.ActionExecutor.ProposeContract(category);
                RPCInvokeResponse responseInstance = new RPCInvokeResponse {
                     rpcId = op.rpcId,
                     result = result
                };
                return new OperationResponse(request.OperationCode, responseInstance);
            }
            return InvalidOperationParameter(request);
        }

        private OperationResponse InvalidOperationParameter(OperationRequest request) {
            return new OperationResponse(request.OperationCode) {
                ReturnCode = (int)ReturnCode.InvalidOperationParameter,
                DebugMessage = "RPC parameters not valid"
            };
        }

        private OperationResponse ErrorResponse(OperationRequest request, int rpcId, RPCErrorCode code) {
            RPCInvokeResponse respInstance = new RPCInvokeResponse {
                rpcId = rpcId,
                result = new Hashtable {
                      {(int)SPC.ReturnCode, (int)code }
                  }
            };
            return new OperationResponse(request.OperationCode, respInstance);
        }
    }
}
