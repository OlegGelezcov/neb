﻿using Common;
using Nebula.Drop;
using Nebula.Engine;
using Nebula.Game.Components;
using Nebula.Server.Operations;
using Photon.SocketServer;
using ServerClientCommon;
using Space.Game;
using System.Collections;

namespace Nebula.Game.OperationHandlers {
    public class RPCInvokeOperationHandler : BasePlayerOperationHandler {

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
                default:
                    return new OperationResponse(request.OperationCode) {
                        ReturnCode = (int)ReturnCode.InvalidRPCID,
                        DebugMessage = string.Format("not found rpc with id = {0}", operation.rpcId)
                    };
            }
        }

        /// <summary>
        /// Handle start collecting asteroid action. Receive request from client and send StartCollectAsteroid generic event to all subscribers
        /// </summary>
        private OperationResponse CallStartAsteroidCollecting(MmoActor player, OperationRequest request, RPCInvokeOperation op) {
            if(op.parameters != null || op.parameters.Length >= 2 ) {
                byte containerType = (byte)op.parameters[0];
                string containerId = (string)op.parameters[1];
                var mmoComponent = player.GetComponent<MmoMessageComponent>();
                if(mmoComponent ) {
                    mmoComponent.PublishStartAsteroidCollecting(containerType, containerId);
                }
                RPCInvokeResponse responseInstance = new RPCInvokeResponse {
                    rpcId = op.rpcId,
                    result = new Hashtable {
                        { (int)SPC.ReturnCode, RPCErrorCode.Ok }
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
    }
}
