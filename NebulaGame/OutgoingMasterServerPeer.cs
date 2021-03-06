﻿using Photon.SocketServer.ServerToServer;
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

namespace NebulaGame {
    public class OutgoingMasterServerPeer : ServerPeerBase {

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        private readonly NebulaGameApplication application;

        private bool redirected = false;
        private IDisposable updateLoop;

        public OutgoingMasterServerPeer(IRpcProtocol protocol, IPhotonPeer nativePeer, NebulaGameApplication application)
            : base(protocol, nativePeer) {
            this.application = application;
            log.InfoFormat("connection to master at {0}:{1} established (id={2}), serverId={3}", this.RemoteIP, this.RemotePort, this.ConnectionId, NebulaGameApplication.ServerId);
            this.RequestFiber.Enqueue(this.Register);
        }

        private void Register() {
            var contract = new RegisterGameServer {
                GameServerAddress = NebulaGameApplication.Instance.PublicIpAddress.ToString(),
                UdpPort = this.application.GamingUdpPort,
                TcpPort = this.application.GamingTcpPort,
                WebSocketPort = this.application.GamingWebSocketPort,
                ServerId = NebulaGameApplication.ServerId.ToString(),
                ServerState = (int)ServerState.Normal
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
                    this.ConnectionId, reasonCode, reasonDetail, NebulaGameApplication.ServerId);
                this.application.ReconnectToMaster();
            } else {
                if(log.IsDebugEnabled) {
                    log.DebugFormat("{0} disconnected from master server: reason={1}, detail={2}, serverId={3}",
                        this.ConnectionId, reasonCode, reasonDetail, NebulaGameApplication.ServerId);
                }
            }
        }



        protected override void OnEvent(IEventData eventData, SendParameters sendParameters) {
            
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
                        log.InfoFormat("Successfully registered at master server: serverId={0}", NebulaGameApplication.ServerId);
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
                        log.WarnFormat("Failed to register at master: err={0}, msg={1}, serverid={2}", operationResponse.ReturnCode, operationResponse.DebugMessage, NebulaGameApplication.ServerId);
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
            log.InfoFormat("Reconnecting to master: serverId={0}", NebulaGameApplication.ServerId);
            NebulaGameApplication.Instance.ConnectToMaster(new IPEndPoint(address, this.RemotePort));
            this.Disconnect();
            this.Dispose();
        }
    }
}
