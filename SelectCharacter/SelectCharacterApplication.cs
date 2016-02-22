using Common;
using ExitGames.Concurrency.Fibers;
using ExitGames.Configuration;
using ExitGames.Logging;
using ExitGames.Logging.Log4Net;
using log4net;
using log4net.Config;
using NebulaCommon;
using NebulaCommon.ServerToServer.Events;
using Photon.SocketServer;
using Photon.SocketServer.ServerToServer;
using SelectCharacter.Auction;
using SelectCharacter.Characters;
using SelectCharacter.Chat;
using SelectCharacter.Commander;
using SelectCharacter.Friends;
using SelectCharacter.Group;
using SelectCharacter.Guilds;
using SelectCharacter.Mail;
using SelectCharacter.Notifications;
using SelectCharacter.PvpStore;
using SelectCharacter.Races;
using SelectCharacter.Resources;
using SelectCharacter.Store;
using ServerClientCommon;
using Space.Game;
using Space.Game.Resources;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using LogManager = ExitGames.Logging.LogManager;

namespace SelectCharacter {
    public class SelectCharacterApplication : ApplicationBase {
        public static readonly Guid ServerId = Guid.NewGuid();
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        private static SelectCharacterApplication instance;
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

        public DbReader DB { get; private set; }
        public MailManager Mail { get; private set; }

        public PlayerService Players { get; private set; }

        public StartPlayerModuleRes StartModules { get; private set; }
        public ItemPriceCollection itemPriceCollection { get; private set; }
        protected StartLocationCollection StartLocations { get; private set; }
        public ClientCollection Clients { get; private set; }
        public NotificationService Notifications { get; private set; }
        public GuildService Guilds { get; private set; }
        public ChatService Chat { get; private set; }
        public GroupService Groups { get; private set; }
        public PlayerStoreService Stores { get; private set; }
        public AuctionService Auction { get; private set; }
        public RaceCommandService RaceCommands { get; private set; }
        public Leveling leveling { get; private set; }
        public CommanderElection Election { get; private set; }
        public RaceStatsService raceStats { get; private set; }
        public FriendService friends { get; private set; }
        public ConsumableItemCollection consumableItems { get; private set; }

        public BankSlotPriceCollection bankSlotPrices { get; private set; }
        public PvpStoreItemCollection pvpStoreItems { get; private set; }

        public ServerInputsRes serverSettings { get; private set; }

        public Res resource { get; private set; }

        public PvpStoreManager pvpStore { get; private set; }

        public Achievments.AchievmentCache achievmentCache { get; private set; }

        public static new SelectCharacterApplication Instance {
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

        public SelectCharacterApplication() {
            UpdateMasterEndPoint();
            this.GamingTcpPort = SelectCharacterSettings.Default.GamingTcpPort;
            this.GamingUdpPort = SelectCharacterSettings.Default.GamingUdpPort;
            this.GamingWebSocketPort = SelectCharacterSettings.Default.GamingWebSocketPort;
            this.ConnectRetryIntervalSeconds = SelectCharacterSettings.Default.ConnectRetryInterval;
        }

        protected override PeerBase CreatePeer(InitRequest initRequest) {
            return new SelectCharacterClientPeer(initRequest, this);
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

                Clients = new ClientCollection();
                Notifications = new NotificationService(this);

#if LOCAL
                string databaseConnectionFile = System.IO.Path.Combine(BinaryPath, "assets/database_connection_local.txt");
#else
                string databaseConnectionFile = System.IO.Path.Combine(BinaryPath, "assets/database_connection.txt");
#endif

                string databaseConnectionString = File.ReadAllText(databaseConnectionFile).Trim();
                this.DB = new DbReader();
                this.DB.Setup(databaseConnectionString, SelectCharacterSettings.Default.DatabaseName, SelectCharacterSettings.Default.DatabaseCollectionName);

                Mail = new MailManager(this);
                Guilds = new GuildService(this);
                Chat = new ChatService(this);
                Groups = new GroupService(this);
                Stores = new PlayerStoreService(this);
                Auction = new AuctionService(this);
                RaceCommands = new RaceCommandService(this);

                this.Players = new PlayerService(this);

                this.StartModules = new StartPlayerModuleRes();
                this.StartModules.LoadFromFile(Path.Combine(this.BinaryPath, "assets/start_player_modules.xml"));

                bankSlotPrices = new BankSlotPriceCollection();
                bankSlotPrices.LoadFromFile(Path.Combine(this.BinaryPath, "assets/bank_slot_price.xml"));

                leveling = new Leveling();
                leveling.Load(Path.Combine(BinaryPath, "assets"));

                itemPriceCollection = new ItemPriceCollection();
                itemPriceCollection.Load(BinaryPath);

                consumableItems = new ConsumableItemCollection();
                consumableItems.Load(BinaryPath);

                StartLocations = new StartLocationCollection();
                StartLocations.LoadFromFile(Path.Combine(BinaryPath, "assets/start_locations.xml"));
                log.Info(StartLocations.GetWorld(Race.Humans, Workshop.DarthTribe));

                pvpStoreItems = new PvpStoreItemCollection();
                pvpStoreItems.Load(Path.Combine(BinaryPath, "assets/pvp_store.xml"));
                log.InfoFormat("store items loaded = {0} [red]", pvpStoreItems.count);

                serverSettings = new ServerInputsRes();
                serverSettings.Load(BinaryPath, "Data/server_inputs.xml");

                resource = new Res(BinaryPath);
                resource.Load();

                Election = new CommanderElection(this);
                raceStats = new RaceStatsService(this);

                friends = new FriendService(this);

                pvpStore = new PvpStoreManager(this);

                achievmentCache = new Achievments.AchievmentCache(this);

                Protocol.AllowRawCustomValues = true;
                this.PublicIpAddress = PublicIPAddressReader.ParsePublicIpAddress(SelectCharacterSettings.Default.PublicIPAddress);
                this.ConnectToMaster();

                this.executionFiber = new PoolFiber();
                this.executionFiber.Start();
                this.executionFiber.ScheduleOnInterval(this.Update,
                    SelectCharacterSettings.Default.DatabaseSaveInterval * 1000,
                    SelectCharacterSettings.Default.DatabaseSaveInterval * 1000);
            }catch(Exception ex) {
                log.Error(ex);
                log.Error(ex.StackTrace);
            }

        }
        protected override void TearDown() {
            log.InfoFormat("TearDown: serverId={0}", ServerId);
            if (this.MasterPeer != null) {
                this.MasterPeer.Disconnect();
            }

            if(executionFiber != null ) {
                executionFiber.Dispose();
                executionFiber = null;
            }

            Update();
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
            if (!IPAddress.TryParse(SelectCharacterSettings.Default.MasterIPAddress, out masterAddress)) {
                var hostEntry = Dns.GetHostEntry(SelectCharacterSettings.Default.MasterIPAddress);
                if (hostEntry.AddressList == null || hostEntry.AddressList.Length == 0) {
                    throw new ConfigurationException("MasterIPAddress setting is neither an IP nor an DNS entry: " +
                        SelectCharacterSettings.Default.MasterIPAddress);
                }
                masterAddress = hostEntry.AddressList.First(address => address.AddressFamily == AddressFamily.InterNetwork);
                if (masterAddress == null) {
                    throw new ConfigurationException(
                        "MasterIPAddress does not resolve to an IPv4 address! Found: "
                        + string.Join(", ", hostEntry.AddressList.Select(a => a.ToString()).ToArray())
                        );
                }
            }

            int masterPort = SelectCharacterSettings.Default.OutgoingMasterServerPeerPort;
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

        private void Update() {
            try {
                if(this.Players == null) {
                    return;
                }
                if(this.DB == null ) {
                    return;
                }

                Players.SaveModified(DB);
                Guilds.SaveModified(DB.Guilds);
                Notifications.SaveModified(DB.Notifications);
                Chat.DumpToDatabase(DB.Chat);
                Stores.SaveChanges();
                RaceCommands.Save();
                Election.Update();
                raceStats.Save();
                friends.Update();

            }catch(Exception ex) {
                log.Error(ex);
                log.Error(ex.StackTrace);
            }
        }

        public string GetStartLocation(Race race, Workshop workshop) {
            return StartLocations.GetWorld(race, workshop);
        }

        public void SendRaceStatusChanged(string gameRefID, string characterID, int raceStatus) {

            S2SRaceStatusChangedEvent evt = new S2SRaceStatusChangedEvent {
                gameRefID = gameRefID,
                characterID = characterID,
                raceStatus = raceStatus
            };

            EventData evtData = new EventData((byte)S2SEventCode.RaceStatusChanged, evt);

            MasterPeer.SendEvent(evtData, new SendParameters());
        }

        public void SendEventToClient(string gameRefID, EventData data) {
            SelectCharacterClientPeer peer;
            if(Clients.TryGetPeerForGameRefId(gameRefID, out peer)) {
                peer.SendEvent(data, new SendParameters());
            }
        }

        public void AddAchievmentVariable(string gameRef, string variable, int count ) {
            MasterPeer.CallS2SMethod(NebulaCommon.ServerType.Game, "AddAchievmentVariable", new object[] { gameRef, variable, count });
        }
    }
}
