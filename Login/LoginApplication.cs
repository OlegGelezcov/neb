﻿using ExitGames.Concurrency.Fibers;
using ExitGames.Configuration;
using ExitGames.Logging;
using ExitGames.Logging.Log4Net;
using log4net;
using log4net.Config;
using Nebula.Resources.Inaps;
using Nebula.Server.Login;
using NebulaCommon;
using Photon.SocketServer;
using Photon.SocketServer.ServerToServer;
using ServerClientCommon;
using Space.Game.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using LogManager = ExitGames.Logging.LogManager;

namespace Login {
    public class LoginApplication : ApplicationBase {

        public static readonly Guid ServerId = Guid.NewGuid();
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        private static LoginApplication instance;
        private static OutgoingMasterServerPeer masterPeer;
        private PoolFiber executionFiber;
        private byte isReconnecting;
        private Timer masterConnectRetryTimer;

        public int? GamingTcpPort { get; protected set; }
        public int? GamingUdpPort { get; protected set; }
        public int? GamingWebSocketPort { get; protected set; }
        public IPEndPoint MasterEndPoint { get; protected set; }
        public IPAddress PublicIpAddress { get; protected set; }
        protected int ConnectRetryIntervalSeconds { get; set; }

        //public PassManager passManager { get; private set; }

        /// <summary>
        /// Server settings file reader
        /// </summary>
        public ServerInputsRes serverSettings { get; private set; }

        public static new LoginApplication Instance {
            get {
                return instance;
            }
            set {
                Interlocked.Exchange(ref instance, value);
            }
        }

        public OutgoingMasterServerPeer MasterPeer {
            get {
                return masterPeer;
            }
            set {
                Interlocked.Exchange(ref masterPeer, value);
            }
        }

        public LoggedInUserCollection LogedInUsers { get; private set; }
        public DbReader DbUserLogins { get; private set; }
        public InapManager inaps { get; private set; }
        public InapCollection inapResource { get; private set; }
        public StatCollection stats { get; private set; }

        public LoginApplication() {
            UpdateMasterEndPoint();
            this.GamingTcpPort = GameServerSettings.Default.GamingTcpPort;
            this.GamingUdpPort = GameServerSettings.Default.GamingUdpPort;
            this.GamingWebSocketPort = GameServerSettings.Default.GamingWebSocketPort;
            this.ConnectRetryIntervalSeconds = GameServerSettings.Default.ConnectRetryInterval;

        }

        protected override PeerBase CreatePeer(InitRequest initRequest) {
            return new LoginClientPeer(initRequest, this);
        }

        protected override ServerPeerBase CreateServerPeer(InitResponse initResponse, object state) {
            Thread.VolatileWrite(ref this.isReconnecting, 0);
            this.MasterPeer = new OutgoingMasterServerPeer(initResponse.Protocol, initResponse.PhotonPeer, this);
            return this.MasterPeer;
        }

        protected override void Setup() {
            try {
                Instance = this;
                this.InitLogging();
                log.InfoFormat("Setup: serverId={0}", ServerId);
#if LOCAL
                string databaseConnectionFile = System.IO.Path.Combine(BinaryPath, "assets/database_connection_local.txt");
#else
                string databaseConnectionFile = System.IO.Path.Combine(BinaryPath, "assets/database_connection.txt");
#endif
                string databaseConnectionString = File.ReadAllText(databaseConnectionFile).Trim();

                this.DbUserLogins = new DbReader();
                this.DbUserLogins.Setup(databaseConnectionString, GameServerSettings.Default.DatabaseName, GameServerSettings.Default.DbLoginCollectionName);

                this.LogedInUsers = new LoggedInUserCollection(this);
                stats = new StatCollection(this);

                //passManager = new PassManager(this);

                serverSettings = new ServerInputsRes();
                serverSettings.Load(BinaryPath, GameServerSettings.Default.assets.SERVER_INPUTS_FILE);

                inapResource = new InapCollection();
                inapResource.Load(System.IO.Path.Combine(BinaryPath, "assets/game_inaps.xml"));
                log.InfoFormat("inaps loaded: {0}", inapResource.inaps.Count);

                inaps = new InapManager(this);

                Protocol.AllowRawCustomValues = true;
                this.PublicIpAddress = PublicIPAddressReader.ParsePublicIpAddress(GameServerSettings.Default.PublicIPAddress);
                this.ConnectToMaster();

                this.executionFiber = new PoolFiber();
                this.executionFiber.Start();
                this.executionFiber.ScheduleOnInterval(this.Update, 60000, 60000);
            } catch(Exception ex) {
                log.Error(ex);
                log.Error(ex.StackTrace);
            }
        }

        protected override void TearDown() {
            log.InfoFormat("TearDown: serverId={0}", ServerId);
            if (this.MasterPeer != null) {
                this.MasterPeer.Disconnect();
            }
        }

        protected override void OnStopRequested() {
            log.InfoFormat("OnStopRequested: serverId={0}", ServerId);
            if (this.masterConnectRetryTimer != null) {
                this.masterConnectRetryTimer.Dispose();
            }
            if (this.MasterPeer != null) {
                this.MasterPeer.Disconnect();
            }
            base.OnStopRequested();
        }

        protected override void OnServerConnectionFailed(int errorCode, string errorMessage, object state) {
            var ipEndPoint = state as IPEndPoint;
            if (ipEndPoint == null) {
                log.ErrorFormat("Unknown connection failed with err {0}: {1}", errorCode, errorMessage);
                return;
            }

            if (ipEndPoint.Equals(this.MasterEndPoint)) {
                if (this.isReconnecting == 0) {
                    log.ErrorFormat("Master connection failed with err {0}: {1}, serverId={2}", errorCode, errorMessage, ServerId);
                } else if (log.IsWarnEnabled) {
                    log.WarnFormat("Master connection failed with err {0}: {1}, serverId={2}", errorCode, errorMessage, ServerId);
                }
                this.ReconnectToMaster();
                return;
            }
        }

        private void UpdateMasterEndPoint() {
            IPAddress masterAddress;
            if (!IPAddress.TryParse(GameServerSettings.Default.MasterIPAddress, out masterAddress)) {
                var hostEntry = Dns.GetHostEntry(GameServerSettings.Default.MasterIPAddress);
                if (hostEntry.AddressList == null || hostEntry.AddressList.Length == 0) {
                    throw new ConfigurationException("MasterIPAddress setting is neither an IP nor an DNS entry: " +
                        GameServerSettings.Default.MasterIPAddress);
                }
                masterAddress = hostEntry.AddressList.First(address => address.AddressFamily == AddressFamily.InterNetwork);
                if (masterAddress == null) {
                    throw new ConfigurationException(
                        "MasterIPAddress does not resolve to an IPv4 address! Found: "
                        + string.Join(", ", hostEntry.AddressList.Select(a => a.ToString()).ToArray())
                        );
                }
            }

            int masterPort = GameServerSettings.Default.OutgoingMasterServerPeerPort;
            this.MasterEndPoint = new IPEndPoint(masterAddress, masterPort);
        }

        public void ConnectToMaster(IPEndPoint endPoint) {
            if (this.Running == false) {
                return;
            }
            if (this.ConnectToServerTcp(endPoint, "Master", endPoint)) {
                if (log.IsInfoEnabled) {
                    log.InfoFormat("Connecting to master at {0}, serverId={1}", endPoint, ServerId);
                }
            } else {
                log.WarnFormat("master connection refused - is the process shutting down? {0}", ServerId);
            }
        }

        public void ConnectToMaster() {
            if (this.Running == false) {
                return;
            }
            UpdateMasterEndPoint();
            ConnectToMaster(MasterEndPoint);
        }

        public void ReconnectToMaster() {
            if (this.Running == false) {
                return;
            }
            Thread.VolatileWrite(ref this.isReconnecting, 1);
            this.masterConnectRetryTimer = new Timer(o => this.ConnectToMaster(), null, this.ConnectRetryIntervalSeconds * 1000, 0);
        }

        protected virtual void InitLogging() {
            LogManager.SetLoggerFactory(Log4NetLoggerFactory.Instance);
            GlobalContext.Properties["Photon:ApplicationLogPath"] = Path.Combine(this.ApplicationRootPath, "log");
            GlobalContext.Properties["LogFileName"] = "GS" + this.ApplicationName;
            XmlConfigurator.ConfigureAndWatch(new FileInfo(Path.Combine(this.BinaryPath, "log4net.config")));
        }

        private void Update() { }

        public bool CheckAccessToken(string loginId, string accessToken ) {
            return true;
        }

        public DbUserLogin GetUser( LoginAuth auth ) {
            return this.DbUserLogins.GetUser(auth);
        }

        public DbUserLogin GetUser(FacebookId fbId ) {
            return DbUserLogins.GetUser(fbId);
        }

        public DbUserLogin GetUser(DeviceId deviceId ) {
            return DbUserLogins.GetUser(deviceId);
        }

        public DbUserLogin GetUser(SteamId sid) {
            return DbUserLogins.GetUser(sid);
        }

        public DbUserLogin GetUser(VkontakteId vkId ) {
            return DbUserLogins.GetUser(vkId);
        }

        public DbUserLogin GetUser(GameRefId gameRef  ) {
            return DbUserLogins.GetUser(gameRef);
        }

        public void SaveUser(DbUserLogin user) {
            DbUserLogins.SaveUser(user);
        }
    }
}

