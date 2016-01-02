using Photon.SocketServer.ServerToServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;
using ExitGames.Logging;
using NebulaCommon.ServerToServer.Operations;
using NebulaCommon;
using System.Net;
using Common;
using NebulaCommon.ServerToServer.Events;
using System.Collections;

namespace SelectCharacter {
    public class OutgoingMasterServerPeer : ServerPeerBase {

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        private readonly SelectCharacterApplication application;
        private readonly S2SMethodInvoker mInvoker;
        private bool redirected = false;
        private IDisposable updateLoop;

        public OutgoingMasterServerPeer(IRpcProtocol protocol, IPhotonPeer nativePeer, SelectCharacterApplication application)
            : base(protocol, nativePeer) {
            this.application = application;
            mInvoker = new S2SMethodInvoker(application);

            log.InfoFormat("connection to master at {0}:{1} established (id={2}), serverId={3}", this.RemoteIP, this.RemotePort, this.ConnectionId, SelectCharacterApplication.ServerId);
            this.RequestFiber.Enqueue(this.Register);
        }

        private void Register() {
            var contract = new RegisterGameServer {
                GameServerAddress = SelectCharacterApplication.Instance.PublicIpAddress.ToString(),
                UdpPort = this.application.GamingUdpPort,
                TcpPort = this.application.GamingTcpPort,
                WebSocketPort = this.application.GamingWebSocketPort,
                ServerId = SelectCharacterApplication.ServerId.ToString(),
                ServerState = (int)ServerState.Normal,
                ServerType = (byte)ServerType.SelectCharacter
            };
            if(log.IsInfoEnabled) {
                log.InfoFormat(
                    "Registering game server with address {0}, TCP {1}, UDP {2}, WebSocket {3}, ServerID {4}",
                    contract.GameServerAddress,
                    contract.TcpPort,
                    contract.UdpPort,
                    contract.WebSocketPort,
                    contract.ServerId
                    );
            }
            var request = new OperationRequest((byte)ServerToServerOperationCode.RegisterGameServer, contract);
            this.SendOperationRequest(request, new SendParameters());
        }

        protected bool IsRegistered { get; set; }


        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail) {
            this.IsRegistered = false;
            this.StopUpdateLoop();

            if(!this.redirected) {
                log.InfoFormat("connection to master closed (id={0}, reason={1}, detail={2}), serverId={3}",
                    this.ConnectionId, reasonCode, reasonDetail, SelectCharacterApplication.ServerId);
                this.application.ReconnectToMaster();
            } else {
                if(log.IsDebugEnabled) {
                    log.DebugFormat("{0} disconnected from master server: reason={1}, detail={2}, serverId={3}",
                        this.ConnectionId, reasonCode, reasonDetail, SelectCharacterApplication.ServerId);
                }
            }
        }



        protected override void OnEvent(IEventData eventData, SendParameters sendParameters) {
            try {
                switch((S2SEventCode)eventData.Code) {
                    default:
                        {
                            if(log.IsDebugEnabled) {
                                log.DebugFormat("Received unknown event code {0}", eventData.Code);
                            }
                            break;
                        }
                    case S2SEventCode.UpdateShipModel:
                        {
                            HandleUpdateShipModelEvent(eventData);
                            break;
                        }
                    case S2SEventCode.UpdateCharacter:
                        {
                            HandleUpdateCharacterEvent(eventData);
                            break;
                        }
                    case S2SEventCode.GETInventoryItemEnd:
                        {
                            GETInventoryItemTransactionEnd end = new GETInventoryItemTransactionEnd(eventData);
                            switch((TransactionSource)end.transactionSource) {
                                case TransactionSource.Store:
                                    {
                                        application.Stores.inventoryGETPool.HandleTransaction(end);
                                        log.Info("GET transaction handled");

                                        break;
                                    }
                                case TransactionSource.Bank:
                                    {

                                        application.Clients.HandleTransaction(end);
                                        log.Info("bank add transaction returned....");
                                        break;
                                    }
                            }
                            break;
                        }
                    case S2SEventCode.GETInventoryItemsEnd:
                        {
                            GETInventoryItemsTransactionEnd end = new GETInventoryItemsTransactionEnd(eventData);
                            switch((TransactionSource)end.transactionSource) {
                                case TransactionSource.Mail:
                                    {
                                        application.Mail.inventoryItemsGETPool.HandleTransaction(end);
                                        log.Info("get transaction handled");
                                        break;
                                    }
                            }
                            break;
                        }
                    case S2SEventCode.PUTInventoryItemEnd:
                        {
                            HandlePUTInventoryTransactionEnd(eventData, sendParameters);
                            break;
                        }
                        //called when from login inap store we put item to character mail
                    case S2SEventCode.PUTMailTransactionStart: {
                            HandlePutMailTransactionStart(eventData, sendParameters);
                        }
                        break;
                    case S2SEventCode.InvokeMethodStart:
                        {
                            string method = (string)eventData.Parameters[(byte)ServerToServerParameterCode.Method];
                            object[] arguments = eventData.Parameters[(byte)ServerToServerParameterCode.Arguments] as object[];
                            string sourceServerID = eventData.Parameters[(byte)ServerToServerParameterCode.SourceServer] as string;
                            byte targetServetType = (byte)eventData.Parameters[(byte)ServerToServerParameterCode.TargetServer];

                            var mtd = mInvoker.GetType().GetMethod(method);

                            S2SInvokeMethodEnd end = new S2SInvokeMethodEnd {
                                method = method,
                                sourceServerID = sourceServerID,
                                targetServerType = targetServetType,
                                
                            };
                            if (mtd != null ) {
                                object result = mtd.Invoke(mInvoker, arguments);
                                end.callSuccess = true;
                                end.result = result;
                            } else {
                                end.callSuccess = false;
                                end.result = null;
                            }

                            EventData retEvent = new EventData((byte)S2SEventCode.InvokeMethodEnd, end);
                            SendEvent(retEvent, new SendParameters());
                            break;
                        }
                }
            }catch(Exception ex) {
                log.Error(ex);
            }
        }

        /// <summary>
        /// Place item to player mail and send back transaction end
        /// </summary>
        /// <param name="eventData">Input event data</param>
        /// <param name="sendParameters">Input send parameters</param>
        private void HandlePutMailTransactionStart(IEventData eventData, SendParameters sendParameters ) {
            var startTransaction = new PUTInventoryItemTransactionStart(eventData);
            var endTransaction = application.Mail.HandlePutMailTransactionStart(startTransaction);
            EventData evt = new EventData((byte)S2SEventCode.PUTMaiTransactionEnd, endTransaction);
            SendEvent(evt, sendParameters);
            log.InfoFormat("Put Mail Transaction End Returned to Source :red");
        }

        private void HandlePUTInventoryTransactionEnd(IEventData eventData, SendParameters sendParameters) {
            PUTInventoryItemTransactionEnd end = new PUTInventoryItemTransactionEnd(eventData);
            switch((TransactionSource)end.transactionSource) {
                case TransactionSource.Store:
                    {
                        log.InfoFormat("Handle PUT Inventory transaction end with Store source");
                        application.Stores.inventoryPUTPool.HandleTransaction(end);
                        break;
                    }
                case TransactionSource.Mail:
                    {
                        log.InfoFormat("Handle PUT Inventory transaction end with MAIL source");
                        application.Mail.inventoryPUTPool.HandleTransaction(end);
                        break;
                    }
                case TransactionSource.Bank:
                    {
                        log.InfoFormat("handle put bank transaction returned");
                        application.Clients.HandleTransaction(end);
                        break;
                    }
                case TransactionSource.PvpStore:
                    {
                        application.pvpStore.putTransactionPool.HandleTransaction(end);
                        log.InfoFormat("transaction handled for buy pvp store item [red]");
                    }
                    break;
                default:
                    {
                        log.ErrorFormat("Unsopprted PUT transaction source = {0}", (TransactionSource)end.transactionSource);
                        break;
                    }
            }
        }

        private void HandleUpdateShipModelEvent(IEventData eventData) {
            var updateShipModel = new UpdateShipModelEvent(this.Protocol, eventData);
            if(false == updateShipModel.IsValid) {
                string msg = updateShipModel.GetErrorMessage();
                log.ErrorFormat("UpdateShipModel contract error: {0}", msg);
                return;
            }
            application.Players.UpdateShipModule(
                updateShipModel.GameRefId, 
                updateShipModel.CharacterId, 
                (ShipModelSlotType)updateShipModel.SlotType, 
                updateShipModel.TemplateId
                );
        }

        private void HandleUpdateCharacterEvent(IEventData eventData) {
            var updateCharacter = new UpdateCharacterEvent(this.Protocol, eventData);
            application.Players.UpdateCharacter(updateCharacter);
        }

        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters) {
            if(log.IsDebugEnabled) {
                log.DebugFormat("Received unknown operation code {0}", operationRequest.OperationCode);
            }
            var response = new OperationResponse { OperationCode = operationRequest.OperationCode, ReturnCode = -1, DebugMessage = "Unknown operation code" };
            this.SendOperationResponse(response, sendParameters);
        }

        protected override void OnOperationResponse(OperationResponse operationResponse, SendParameters sendParameters) {
            switch((ServerToServerOperationCode)operationResponse.OperationCode) {
                default:
                    {
                        if(log.IsDebugEnabled) {
                            log.DebugFormat("Received unknown operation code {0}", operationResponse.OperationCode);
                        }
                        break;
                    }
                case ServerToServerOperationCode.RegisterGameServer:
                    {
                        this.HandleRegisterGameServerResponse(operationResponse);
                        break;
                    }
            }
        }

        private void HandleRegisterGameServerResponse(OperationResponse operationResponse) {
            var contract = new RegisterGameServerResponse(this.Protocol, operationResponse);
            if(!contract.IsValid) {
                if(operationResponse.ReturnCode != (short)ReturnCode.Ok) {
                    log.ErrorFormat("RegisterGameServer returned with err {0}: {1}", operationResponse.ReturnCode, operationResponse.DebugMessage);
                }
                log.Error("RegisterGameServerResponse contract invalid: " + contract.GetErrorMessage());
                this.Disconnect();
                return;
            }

            switch(operationResponse.ReturnCode) {
                case (short)ReturnCode.Ok:
                    {
                        log.InfoFormat("Successfully registered at master server: serverId={0}", SelectCharacterApplication.ServerId);
                        this.IsRegistered = true;
                        this.StartUpdateLoop();
                        break;
                    }
                case (short)ReturnCode.RedirectRepeat:
                    {
                        var address = new IPAddress(contract.InternalAddress);
                        log.InfoFormat("Connected master server is not the leader; Reconnecting to master at IP {0}...", address);
                        this.Reconnect(address);
                        break;
                    }
                default:
                    {
                        log.WarnFormat("Failed to register at master: err={0}, msg={1}, serverid={2}", operationResponse.ReturnCode, operationResponse.DebugMessage, SelectCharacterApplication.ServerId);
                        this.Disconnect();
                        break;
                    }
            }
        }

        protected void StartUpdateLoop() {
            if(this.updateLoop != null ) {
                log.Error("Update loop already started! Duplicate RegisterGameServer response?");
                this.updateLoop.Dispose();
            }

            this.updateLoop = this.RequestFiber.ScheduleOnInterval(this.UpdateServerState, 1000, 1000);

        }

        protected void StopUpdateLoop() {
            if (this.updateLoop != null) {
                this.updateLoop.Dispose();
                this.updateLoop = null;
            }
        }

        public void UpdateServerState() {
            if(this.Connected == false) {
                return;
            }

            //here do something
        }

        protected void Reconnect(IPAddress address) {
            this.redirected = true;
            log.InfoFormat("Reconnecting to master: serverId={0}", SelectCharacterApplication.ServerId);
            SelectCharacterApplication.Instance.ConnectToMaster(new IPEndPoint(address, this.RemotePort));
            this.Disconnect();
            this.Dispose();
        }
    }
}
