using ExitGames.Concurrency.Fibers;
using ExitGames.Configuration;
using ExitGames.Logging;
using ExitGames.Logging.Log4Net;
using log4net;
using log4net.Config;
using NebulaCommon;
using Photon.SocketServer;
using Photon.SocketServer.ServerToServer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using LogManager = ExitGames.Logging.LogManager;

namespace NebulaGame {

    public class NebulaGameApplication : ApplicationBase {

        public static readonly Guid ServerId = Guid.NewGuid();
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        private static NebulaGameApplication instance;
        private static OutgoingMasterServerPeer masterPeer;
        private PoolFiber executionFiber;
        private byte isReconnecting;
        private Timer masterConnectRetryTimer; 
        private byte currentNodeId = 2;

        public int? GamingTcpPort { get; protected set; }
        public int? GamingUdpPort { get; protected set; }
        public int? GamingWebSocketPort { get; protected set; }
        public IPEndPoint MasterEndPoint { get; protected set; }
        public IPAddress PublicIpAddress { get; protected set; }
        protected int ConnectRetryIntervalSeconds { get; set; }

        public static new NebulaGameApplication Instance {
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

        public NebulaGameApplication() {
            UpdateMasterEndPoint();
            this.GamingTcpPort = GameServerSettings.Default.GamingTcpPort;
            this.GamingUdpPort = GameServerSettings.Default.GamingUdpPort;
            this.GamingWebSocketPort = GameServerSettings.Default.GamingWebSocketPort;
            this.ConnectRetryIntervalSeconds = GameServerSettings.Default.ConnectRetryInterval;
        }

        protected override PeerBase CreatePeer(InitRequest initRequest) {
            return null;
        }

        protected override ServerPeerBase CreateServerPeer(InitResponse initResponse, object state) {
            Thread.VolatileWrite(ref this.isReconnecting, 0);
            this.MasterPeer = new OutgoingMasterServerPeer(initResponse.Protocol, initResponse.PhotonPeer, this);
            return this.MasterPeer;
        }

        protected override void Setup() {
            Instance = this;
            this.InitLogging();
            log.InfoFormat("Setup: serverId={0}", ServerId);
            Protocol.AllowRawCustomValues = true;
            this.PublicIpAddress = PublicIPAddressReader.ParsePublicIpAddress(GameServerSettings.Default.PublicIPAddress);
            this.ConnectToMaster();

            this.executionFiber = new PoolFiber();
            this.executionFiber.Start();
            this.executionFiber.ScheduleOnInterval(this.Update, 60000, 60000);
        }
        protected override void TearDown() {
            log.InfoFormat("TearDown: serverId={0}", ServerId);
            if(this.MasterPeer != null ) {
                this.MasterPeer.Disconnect();
            }
        }

        protected override void OnStopRequested() {
            log.InfoFormat("OnStopRequested: serverId={0}", ServerId);
            if(this.masterConnectRetryTimer != null ) {
                this.masterConnectRetryTimer.Dispose();
            }
            if(this.MasterPeer != null) {
                this.MasterPeer.Disconnect();
            }
            base.OnStopRequested();
        }

        protected override void OnServerConnectionFailed(int errorCode, string errorMessage, object state) {
            var ipEndPoint = state as IPEndPoint;
            if(ipEndPoint == null ) {
                log.ErrorFormat("Unknown connection failed with err {0}: {1}", errorCode, errorMessage);
                return;
            }

            if(ipEndPoint.Equals(this.MasterEndPoint)) {
                if(this.isReconnecting == 0) {
                    log.ErrorFormat("Master connection failed with err {0}: {1}, serverId={2}", errorCode, errorMessage, ServerId);
                }else if(log.IsWarnEnabled) {
                    log.WarnFormat("Master connection failed with err {0}: {1}, serverId={2}", errorCode, errorMessage, ServerId);
                }
                this.ReconnectToMaster();
                return;
            }
        }

        private void UpdateMasterEndPoint() {
            IPAddress masterAddress;
            if(!IPAddress.TryParse(GameServerSettings.Default.MasterIPAddress, out masterAddress)) {
                var hostEntry = Dns.GetHostEntry(GameServerSettings.Default.MasterIPAddress);
                if(hostEntry.AddressList == null || hostEntry.AddressList.Length == 0 ) {
                    throw new ConfigurationException("MasterIPAddress setting is neither an IP nor an DNS entry: " +
                        GameServerSettings.Default.MasterIPAddress);
                }
                masterAddress = hostEntry.AddressList.First(address => address.AddressFamily == AddressFamily.InterNetwork);
                if(masterAddress == null ) {
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
            if(this.Running == false) {
                return;
            }
            if(this.ConnectToServerTcp(endPoint, "Master", endPoint)) {
                if(log.IsInfoEnabled) {
                    log.InfoFormat("Connecting to master at {0}, serverId={1}", endPoint, ServerId);
                }
            } else {
                log.WarnFormat("master connection refused - is the process shutting down? {0}", ServerId);
            }
        }

        public void ConnectToMaster() {
            if(this.Running == false ) {
                return;
            }
            UpdateMasterEndPoint();
            ConnectToMaster(MasterEndPoint);
        }

        public void ReconnectToMaster() {
            if(this.Running == false ) {
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
    }
}
