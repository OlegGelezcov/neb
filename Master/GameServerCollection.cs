// GameServerCollection.cs
// Nebula
//
// Created by Oleg Zheleztsov on Sunday, September 13, 2015 5:40:15 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//


namespace Master {
    using ExitGames.Logging;
    using NebulaCommon;
    using Photon.SocketServer;
    using System;
    using System.Collections.Generic;

    public class GameServerCollection : Dictionary<string, IncomingGameServerPeer> {

        private readonly object syncRoot = new object();
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public void OnConnect(IncomingGameServerPeer gameServerPeer ) {
            if (!gameServerPeer.ServerId.HasValue) {
                throw new InvalidOperationException("server id cannot be null");
            }
            string key = gameServerPeer.Key;
            lock(this.syncRoot) {
                IncomingGameServerPeer peer;
                if(this.TryGetValue(key, out peer)) {
                    if(gameServerPeer != peer ) {
                        peer.Disconnect();
                        peer.RemoveGameServerPeerOnMaster();
                        this.Remove(key);
                    }
                }
                this.Add(key, gameServerPeer);
            }
        }

        public void OnDisconnect(IncomingGameServerPeer gameServerPeer) {
            if (!gameServerPeer.ServerId.HasValue) {
                throw new InvalidOperationException("server id cannot be null");
            }
            string key = gameServerPeer.Key;
            lock(this.syncRoot) {
                IncomingGameServerPeer peer;
                if(this.TryGetValue(key, out peer)) {
                    if(peer == gameServerPeer) {
                        this.Remove(key);
                    }
                }
            }
        }

        public void SendEvent(IEventData evt, SendParameters sendParameters, ServerType serverType) {
            lock(syncRoot) {
                foreach(var srv in this) {
                    if(srv.Value.ServerType == serverType) {
                        
                        srv.Value.SendEvent(evt, sendParameters);
                        log.InfoFormat("sended event {0} to game server {1}", (S2SEventCode)evt.Code, srv.Value.ServerType);
                    }
                }
            }
        }

        //public void SendEventTo(IEventData evt, SendParameters sendParameters, string serverID ) {
        //    lock(syncRoot) {
        //        foreach(var srv in this ) {
        //            if(srv.Value.ServerId.Value.ToString() == serverID ) {
        //                srv.Value.SendEvent(evt, sendParameters);
                        
        //            }
        //        }
        //    }
        //}

        public void SendEvent(IEventData evt, SendParameters sendParameters, string serverID ) {
            lock(syncRoot) {
                foreach(var srv in this ) {
                    if(srv.Value.ServerId.Value.ToString() == serverID ) {
                        srv.Value.SendEvent(evt, sendParameters);
                        log.InfoFormat("transport event to serv: {0}:{1}", srv.Value.ServerId.Value.ToString(), srv.Value.ServerType);
                        break;
                    }
                }
            }
        }

    }
}
