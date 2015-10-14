// MasterClientPeer.cs
// Nebula
//
// Created by Oleg Zheleztsov on Sunday, September 13, 2015 5:42:23 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
namespace Master {
    using Common;
    using ExitGames.Logging;
    using Master.Operations;
    using Photon.SocketServer;
    using PhotonHostRuntimeInterfaces;
    using System.Collections;

    public class MasterClientPeer : PeerBase {

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        private MasterApplication application;
        public MasterClientPeer(InitRequest initRequest, MasterApplication application)
            : base(initRequest.Protocol, initRequest.PhotonPeer) {
            this.application = application;
        }

        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail) {
            if (log.IsDebugEnabled) {
                log.DebugFormat("Disconnect: pid={0}: reason={1}, detail={2}", this.ConnectionId, reasonCode, reasonDetail);
            }
        }

        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters) {
            OperationResponse response;

            switch((OperationCode)operationRequest.OperationCode) {
                default:
                    response = new OperationResponse(operationRequest.OperationCode) {
                        ReturnCode = (short)ReturnCode.OperationInvalid,
                        DebugMessage = "Unknown operation code"
                    };
                    break;
                case OperationCode.GetServerList:
                    {
                        var responseObject = new GetServerListResponse {
                            ServerList = this.application.ServerInfoCollection.GetInfo()
                        };
                        response = new OperationResponse(operationRequest.OperationCode, responseObject);
                        break;
                    }
                case OperationCode.GetServerVersion:
                    {
                        var responseObject = new GetServerVersionResponse {
                            serverVersion = application.appVersion.ToString()
                        };
                        response = new OperationResponse(operationRequest.OperationCode, responseObject);
                        break;
                    }
                case OperationCode.GetNews:
                    {
                        GetNewsRequest request = new GetNewsRequest(Protocol, operationRequest);
                        if(!request.IsValid) {
                            response = new OperationResponse(operationRequest.OperationCode) { ReturnCode = (short)ReturnCode.InvalidOperationParameter, DebugMessage = "lang" };
                        } else {
                            Hashtable newsHash = application.news.GetNews(request.lang);
                            GetNewsResponse responseObject = new GetNewsResponse { news = newsHash };
                            response = new OperationResponse(operationRequest.OperationCode, responseObject);
                        }
                        break;
                    }

            }

            if(response != null ) {
                this.SendOperationResponse(response, sendParameters);
            }
        }
    }
}
