// MasterApplication.cs
// Nebula
//
// Created by Oleg Zheleztsov on Sunday, September 13, 2015 5:41:46 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
namespace Master {
    using Common;
    using ExitGames.Logging;
    using ExitGames.Logging.Log4Net;
    using log4net;
    using log4net.Config;
    using News;
    using Nebula;
    using Photon.SocketServer;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Xml.Linq;
    using LogManager = ExitGames.Logging.LogManager;

    public class MasterApplication : ApplicationBase {

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        public GameServerCollection GameServers { get; protected set; }
        protected byte MasterNodeId { get; set; }
        public AppVersion appVersion { get; private set; }

        public DatabaseManager database { get; private set; }

        public NewsService news { get; private set; }

        public string databaseConnectionString { get; private set; }

        protected readonly byte LocalNodeId = 1;

        public bool IsMaster {
            get {
                return this.LocalNodeId == this.MasterNodeId;
            }
        }

        public ServerInfoCollection ServerInfoCollection { get; set; }


        protected override PeerBase CreatePeer(InitRequest initRequest) {
            if (this.IsGameServerPeer(initRequest)) {
                if (log.IsDebugEnabled) {
                    log.DebugFormat("Received init request from game server");
                }
                return new IncomingGameServerPeer(initRequest, this);
            }
            if (IsMaster) {
                if(log.IsDebugEnabled) {
                    log.DebugFormat("Received init request from game client on master node");
                }
                return new MasterClientPeer(initRequest, this);
            }

            if(log.IsDebugEnabled) {
                log.DebugFormat("Received request from client, but this is not master node");
            }
            return null;
        }

        protected override void Setup() {
            try {
                LogManager.SetLoggerFactory(Log4NetLoggerFactory.Instance);
                GlobalContext.Properties["Photon:ApplicationLogPath"] = Path.Combine(this.ApplicationRootPath, "log");
                GlobalContext.Properties["LogFileName"] = "MS" + this.ApplicationName;
                XmlConfigurator.ConfigureAndWatch(new FileInfo(Path.Combine(this.BinaryPath, "log4net.config")));

                Protocol.AllowRawCustomValues = true;
                this.MasterNodeId = (byte)MasterServerSettings.Default.MasterNodeId;

                this.Initialize();
            } catch(Exception ex) {
                log.Error(ex);
                log.Error(ex.StackTrace);
            }
        }

        protected override void TearDown() {
            
        }

        protected override void OnStopRequested() {
            if(log.IsDebugEnabled) {
                log.DebugFormat("OnStopRequested... going to disconnect {0} GS peers", this.GameServers.Count);
            }
            if(this.GameServers != null ) {
                var gameServers = new Dictionary<string, IncomingGameServerPeer>(this.GameServers);
                foreach(var gameServer in gameServers) {
                    var peer = gameServer.Value;
                    if(log.IsDebugEnabled) {
                        log.DebugFormat("Disconnecting GS peer {0}:{1}", peer.RemoteIP, peer.RemotePort);
                    }
                    peer.Disconnect();
                }
            }
        }

        protected virtual void Initialize() {
            this.GameServers = new GameServerCollection();
            this.ServerInfoCollection = new ServerInfoCollection();

#if LOCAL
            string serverInfoFile = Path.Combine(this.BinaryPath, "assets/servers_local.xml");
            string databaseConnectionFile = Path.Combine(BinaryPath, "assets/database_connection_local.txt");
#else
            string serverInfoFile = Path.Combine(this.BinaryPath, "assets/servers.xml");
            string databaseConnectionFile = Path.Combine(BinaryPath, "assets/database_connection.txt");
#endif

            this.ServerInfoCollection.LoadFrom(serverInfoFile);
            ReadDatabaseConnectionString(databaseConnectionFile);

            appVersion = GetAppVersion();

            database = new DatabaseManager();
            database.Setup(databaseConnectionString, MasterServerSettings.Default.DatabaseName, MasterServerSettings.Default.NewsCollectionName);

            news = new NewsService(this);
        }

        private void ReadDatabaseConnectionString(string path) {
            databaseConnectionString = File.ReadAllText(path).Trim();
        }

        public IPAddress GetInternalMasterNodeIpAddress() {

            return IPAddress.Parse(MasterServerSettings.Default.S2SMasterAddress);
        }

        protected virtual bool IsGameServerPeer(InitRequest initRequest) {
            return initRequest.LocalPort == MasterServerSettings.Default.IncomingGameServerPeerPort;
        }

        private AppVersion GetAppVersion() {
            string settingsFilePath = Path.Combine(BinaryPath, "Data/server_inputs.xml");
            XDocument document = XDocument.Load(settingsFilePath);
            List<string> versions = document.Element("inputs").Elements("input").Where(e => {
                if (e.HasAttribute("key")) {
                    if (e.GetString("key") == "server_version") {
                        return true;
                    }
                }
                return false;
            }).Select(e => e.GetString("value")).ToList();
            if(versions.Count > 0 ) {
                log.InfoFormat("server version founded: {0} yellow", versions[0]);
                return AppVersion.FromString(versions[0]);
            } else {
                return new AppVersion(0, 0, 0, 0);
            }
        }
    }
}
