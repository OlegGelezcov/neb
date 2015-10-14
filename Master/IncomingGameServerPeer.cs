// IncomingGameServerPeer.cs
// Nebula
//
// Created by Oleg Zheleztsov on Sunday, September 13, 2015 5:41:00 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
namespace Master {
    using Common;
    using ExitGames.Logging;
    using NebulaCommon;
    using NebulaCommon.ServerToServer.Events;
    using NebulaCommon.ServerToServer.Operations;
    using Photon.SocketServer;
    using Photon.SocketServer.ServerToServer;
    using PhotonHostRuntimeInterfaces;
    using System;
    using System.Collections;
    using System.Net;

    public class IncomingGameServerPeer : ServerPeerBase {

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        private readonly MasterApplication application;

        public IncomingGameServerPeer(InitRequest initRequest, MasterApplication application)
            : base(initRequest.Protocol, initRequest.PhotonPeer) {
            this.application = application;
            log.InfoFormat("game server connection from {0}:{1} established (id={2})", this.RemoteIP, this.RemotePort, this.ConnectionId);
        }

        public string Key { get; protected set; }
        public Guid? ServerId { get; protected set; }
        public string TcpAddress { get; protected set; }
        public string UdpAddress { get; protected set; }
        public string WebSocketAddress { get; protected set; }
        public ServerState State { get; private set; }
        public int PeerCount { get; private set; }

        public ServerType ServerType { get; private set; }

        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail) {
            if(log.IsInfoEnabled) {
                string serverId = this.ServerId.HasValue ? this.ServerId.ToString() : "{null}";
                log.InfoFormat("OnDiconnect: game server connection closed (connectionId={0}, serverId={1}, reason={2})", this.ConnectionId, serverId, reasonCode);
            }
            this.RemoveGameServerPeerOnMaster();
        }



        protected override void OnEvent(IEventData eventData, SendParameters sendParameters) {
            try {
                if (!this.ServerId.HasValue) {
                    log.Warn("received game server event but server is not registered");
                    return;
                }
                log.InfoFormat("Game server send event {0}", (S2SEventCode)eventData.Code);
                switch((S2SEventCode)eventData.Code) {
                    case S2SEventCode.GroupUpdate:
                        {
                            HandleGroupUpdateEvent(eventData, sendParameters);
                            break;
                        }
                    case S2SEventCode.GroupRemoved:
                        {
                            HandleGroupRemovedEvent(eventData, sendParameters);
                            break;
                        }
                    case S2SEventCode.GETInventoryItemStart:
                        {
                            Transport(eventData, sendParameters, ServerType.Game);
                            break;
                        }
                    case S2SEventCode.GETInventoryItemEnd:
                        {
                            Transport(eventData, sendParameters, ServerType.SelectCharacter);
                            break;
                        }
                    case S2SEventCode.GETInventoryItemsStart:
                        {
                            Transport(eventData, sendParameters, ServerType.Game);
                            break;
                        }
                    case S2SEventCode.GETInventoryItemsEnd:
                        {
                            Transport(eventData, sendParameters, ServerType.SelectCharacter);
                            break;
                        }
                    case S2SEventCode.PUTInventoryItemStart:
                        {
                            Transport(eventData, sendParameters, ServerType.Game);
                            break;
                        }
                    case S2SEventCode.PUTInventoryItemEnd:
                        {
                            Transport(eventData, sendParameters, ServerType.SelectCharacter);
                            break;
                        }
                    case S2SEventCode.InvokeMethodStart:
                        {
                            ServerType servType = (ServerType)(byte)eventData.Parameters[(byte)ServerToServerParameterCode.TargetServer];
                            if(servType != ServerType.Master) {
                                Transport(eventData, sendParameters, servType);
                            }
                            break;
                        }
                    case S2SEventCode.InvokeMethodEnd:
                        {
                            string servID = (string)eventData.Parameters[(byte)ServerToServerParameterCode.SourceServer];
                            application.GameServers.SendEvent(eventData, sendParameters, servID);
                            break;
                        }
                    case S2SEventCode.RaceStatusChanged: {
                            Transport(eventData, sendParameters, ServerType.Game);
                            break;
                        }
                }
            }catch(Exception ex) {
                log.Error(ex);
            }
        }

        private void Transport(IEventData evt, SendParameters sendParameters, ServerType server) {
            application.GameServers.SendEvent(evt, sendParameters, server);
        }

        private void HandleGroupUpdateEvent(IEventData eventData, SendParameters sendParameters) {
            log.Info("IncomingGameServerPeer.HandleGroupUpdateEvent");
            Hashtable groupHash = (Hashtable)eventData.Parameters[(byte)ServerToServerParameterCode.Group];
            S2SGroupUpdateEvent evt = new S2SGroupUpdateEvent { group = groupHash };
            EventData eData = new EventData((byte)S2SEventCode.GroupUpdate, evt);
            application.GameServers.SendEvent(eData, sendParameters, ServerType.Game);
        }

        private void HandleGroupRemovedEvent(IEventData eventData, SendParameters sendParameters ) {
            log.Info("IncomingGameServerPeer.HandleGroupRemovedEvent");
            string groupID = (string)eventData.Parameters[(byte)ServerToServerParameterCode.Group];
            S2SGroupRemovedEvent evt = new S2SGroupRemovedEvent { Group = groupID };
            EventData eData = new EventData((byte)S2SEventCode.GroupRemoved, evt);
            application.GameServers.SendEvent(eData, sendParameters, ServerType.Game);
        }

        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters) {
            try {
                if (log.IsDebugEnabled) {
                    log.DebugFormat("OnOperationRequest: pid={0}, op={1}", this.ConnectionId, operationRequest.OperationCode);
                }
                OperationResponse response = null;
                switch((ServerToServerOperationCode)operationRequest.OperationCode) {
                    default:
                        response = new OperationResponse(operationRequest.OperationCode) { ReturnCode = -1, DebugMessage = "Unknown operation code" };
                        break;
                    case ServerToServerOperationCode.RegisterGameServer: {
                            response = this.ServerId.HasValue
                                ? new OperationResponse(operationRequest.OperationCode) { ReturnCode = -1, DebugMessage = "Already registered" }
                                : this.HandleRegisterGameServerRequest(operationRequest);
                            break;
                        }
                    case ServerToServerOperationCode.UpdateShipModel:
                        {
                            UpdateShipModel operation = new UpdateShipModel(this.Protocol, operationRequest);
                            if(!operation.IsValid) {
                                response = new OperationResponse(operationRequest.OperationCode) { ReturnCode = (short)ReturnCode.InvalidOperationParameter, DebugMessage = "InvalidOperationParameter" };
                            } else {
                                UpdateShipModelEvent evtData = new UpdateShipModelEvent {
                                    CharacterId = operation.CharacterId,
                                    GameRefId = operation.GameRefId,
                                    SlotType = operation.SlotType,
                                    TemplateId = operation.TemplateId
                                };
                                EventData evt = new EventData((byte)S2SEventCode.UpdateShipModel, evtData);
                                application.GameServers.SendEvent(evt, new SendParameters(), ServerType.SelectCharacter);
                            }
                            break;
                        }
                    case ServerToServerOperationCode.UpdateCharacter:
                        {
                            UpdateCharacter operation = new UpdateCharacter(this.Protocol, operationRequest);
                            UpdateCharacterEvent evtData = new UpdateCharacterEvent {
                                CharacterId = operation.CharacterId,
                                Deleted = operation.Deleted,
                                GameRefId = operation.GameRefId,
                                Model = operation.Model,
                                Race = operation.Race,
                                Workshop = operation.Workshop,
                                 Exp = operation.Exp,
                                  WorldId = operation.WorldId
                            };
                            EventData evt = new EventData((byte)S2SEventCode.UpdateCharacter, evtData);
                            application.GameServers.SendEvent(evt, new SendParameters(), ServerType.SelectCharacter);
                            break;
                        }
                }
                if(response != null)
                    this.SendOperationResponse(response, sendParameters);
            } catch(Exception ex) {
                log.Error(ex);
            }
        }



        protected override void OnOperationResponse(OperationResponse operationResponse, SendParameters sendParameters) {
            throw new NotSupportedException();
        }

        public override string ToString() {
            if (this.ServerId.HasValue) {
                return string.Format("GameServer({2}) at {0}/{1}", this.TcpAddress, this.UdpAddress, this.ServerId);
            }
            return base.ToString();
        }

        protected virtual Hashtable GetAuthlist() { return null; }

        protected virtual byte[] SharedKey {
            get { return null; }
        }

        public void RemoveGameServerPeerOnMaster() {
            if(this.ServerId.HasValue) {
                this.application.GameServers.OnDisconnect(this);
            }
        }

        private OperationResponse HandleRegisterGameServerRequest(OperationRequest operationRequest) {
            var registerRequest = new RegisterGameServer(this.Protocol, operationRequest);
            if (!registerRequest.IsValid) {
                string msg = registerRequest.GetErrorMessage();
                log.ErrorFormat("RegisterGameServer contract error: {0}", msg);
                return new OperationResponse(operationRequest.OperationCode) { DebugMessage = msg, ReturnCode = (short)ReturnCode.OperationInvalid };
            }

            IPAddress masterAddress = this.application.GetInternalMasterNodeIpAddress();
            var contract = new RegisterGameServerResponse { InternalAddress = masterAddress.GetAddressBytes() };

            if(this.application.IsMaster) {
                if(log.IsErrorEnabled) {
                    log.DebugFormat(
                        "Received register request: Address={0}, UdpPort={1}, TcpPort={2}, WebSocketPort={3}, State={4}",
                        registerRequest.GameServerAddress,
                        registerRequest.UdpPort,
                        registerRequest.TcpPort,
                        registerRequest.WebSocketPort,
                        (ServerState)registerRequest.ServerState);
                }
            }

            if(registerRequest.UdpPort.HasValue) {
                this.UdpAddress = registerRequest.GameServerAddress + ":" + registerRequest.UdpPort;
            }
            if (registerRequest.TcpPort.HasValue) {
                this.TcpAddress = registerRequest.GameServerAddress + ":" + registerRequest.TcpPort;
            }
            if(registerRequest.WebSocketPort.HasValue && registerRequest.WebSocketPort != 0) {
                this.WebSocketAddress = registerRequest.GameServerAddress + ":" + registerRequest.WebSocketPort;
            }

            this.ServerId = new Guid(registerRequest.ServerId);
            this.State = (ServerState)registerRequest.ServerState;
            this.Key = string.Format("{0}-{1}-{2}", registerRequest.GameServerAddress, registerRequest.UdpPort, registerRequest.TcpPort);
            this.ServerType = (ServerType)registerRequest.ServerType;

            this.application.GameServers.OnConnect(this);
            //if(this.State == ServerState.Normal) {
            //    this.application.LoadBalancer.TryAddServer(this, 0);
            //}

            contract.AuthList = this.GetAuthlist();
            contract.SharedKey = this.SharedKey;
            return new OperationResponse(operationRequest.OperationCode, contract);
        }
    }
}
