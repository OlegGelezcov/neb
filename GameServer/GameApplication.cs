using Common;
using ExitGames.Configuration;
using ExitGames.Diagnostics.Monitoring;
using ExitGames.Logging;
using ExitGames.Logging.Log4Net;
using log4net;
using log4net.Config;
using MongoDB.Driver;
using Nebula;
using Nebula.Game;
using Nebula.Game.Utils;
using NebulaCommon;
using NebulaCommon.SelectCharacter;
using NebulaCommon.ServerToServer.Operations;
using Photon.SocketServer;
using Photon.SocketServer.Diagnostics;
using Photon.SocketServer.ServerToServer;
using Space.Database;
using Space.Game;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using LogManager = ExitGames.Logging.LogManager;

public class GameApplication : ApplicationBase
{
    public static readonly Guid ServerId = Guid.NewGuid();
    public static readonly CounterSamplePublisher CounterPublisher;
    private static readonly ILogger log = LogManager.GetCurrentClassLogger();
    //private static GameApplication instance;
    private static OutgoingMasterServerPeer masterPeer;
    
    private byte isReconnecting;
    private Timer masterConnectRetryTimer;

    private readonly static object syncObject = new object();
    private static ResourcePool resourcePool = null;
    //public static CooperativeGroups PlayerGroups { get; private set; }

    public int? GamingTcpPort { get; protected set; }
    public int? GamingUdpPort { get; protected set; }
    public int? GamingWebSocketPort { get; protected set; }
    public IPEndPoint MasterEndPoint { get; protected set; }
    public IPAddress PublicIpAddress { get; protected set; }
    protected int ConnectRetryIntervalSeconds { get; set; }

    public GameUpdater updater { get; private set; }

    //private DatabaseUsers mDatabaseUsers;


    private DatabaseManager databaseManager;


    private string roleName = "";
    private ConnectionRole currentRole;
    private ConnectionConfigCollection rolesCollection;
    public ConcurrentDictionary<string, MmoActor> serverActors = new ConcurrentDictionary<string, MmoActor>();

    //private readonly PoolFiber loopFiber;
    
    public MmoActor GetServerActor(string gameRef) {
        MmoActor player;
        if(serverActors.TryGetValue(gameRef, out player)) {
            return player;
        }
        return null;
    }

    public string databaseConnectionString { get; private set; }

    private MongoClient m_MongoClient;
    private MongoServer m_MongoServer;
    private MongoDatabase m_DefaultDatabase;

    public string RoleName {
        get {
            if(currentRole != null ) {
                return currentRole.RoleName;
            }
            return string.Empty;
        }
    }

    public MongoDatabase defaultDatabase {
        get {
            return m_DefaultDatabase;
        }
    }

    /*
    public static new GameApplication Instance {
        get {
            return instance;
        }
        set {
            Interlocked.Exchange(ref instance, value);
        }
    }*/

    public OutgoingMasterServerPeer MasterPeer {
        get {
            return masterPeer;
        }
        set {
            Interlocked.Exchange(ref masterPeer, value);
        }
    }

    public DatabaseManager DatabaseManager {
        get {
            return this.databaseManager;
        }
    }


    private readonly LibLoggerObject m_LibLoggerObj = new LibLoggerObject();
    /// <summary>
    /// Initializes static members of the <see cref="PhotonApplication"/> class.
    /// </summary>
    static GameApplication()
    {
        CounterPublisher = new CounterSamplePublisher(1);
    }

    public GameApplication() {
        Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
        ReceiveRoleName();
        ReceiveRole();

        //mDatabaseUsers = new DatabaseUsers();
        //mDatabaseUsers.Load(BinaryPath);

        UpdateMasterEndPoint();
        this.GamingTcpPort = currentRole.GamingTcpPort;
        this.GamingUdpPort = currentRole.GamingUdpPort;
        this.GamingWebSocketPort = currentRole.GamingWebSocketPort;
        this.ConnectRetryIntervalSeconds = GameServerSettings.Default.ConnectRetryInterval;

    }

    private void ReceiveRoleName() {
        try {
            roleName = File.ReadAllText(Path.Combine(BinaryPath, "assets/role.txt")).ToLower().Trim();
            log.InfoFormat("current role: {0}", roleName);
        } catch(Exception exception) {
            log.Error(exception);
        }
    }

    //public string ConnectionString() {
    //    var dbUser = mDatabaseUsers.GetUser(roleName);
    //    if(dbUser != null ) {
    //        return dbUser.ConnectionString(GameServerSettings.Default.DatabaseIP);
    //    }
    //    return string.Empty;
    //}

    private void ReceiveRole() {
        try {
            rolesCollection = new ConnectionConfigCollection();
#if LOCAL
            rolesCollection.LoadFromFile(Path.Combine(BinaryPath, "assets/connection_config_local.xml"));
#else
            rolesCollection.LoadFromFile(Path.Combine(BinaryPath, "assets/connection_config.xml"));
#endif

            if (!rolesCollection.TryGetRoleConnection(roleName.ToLower(), out currentRole)) {
                throw new Exception(string.Format("error of receiving role {0}", roleName.ToLower()));
            }
        } catch (Exception exception) {
            log.Error(exception);
            log.Error(exception.StackTrace);
        }
    }

    public ConnectionRole CurrentRole() {
        return currentRole;
    }



    protected override void Setup() {
        try {

            /*
            Instance = this;
            */

            m_LibLoggerObj.Setup();

            AppDomain.CurrentDomain.UnhandledException += AppDomain_OnUnhandledException;

            this.InitLogging();
            log.InfoFormat("GameApplication.Setup()");

            CounterPublisher.AddCounter(new CpuUsageCounterReader(), "Cpu");
            CounterPublisher.AddCounter(PhotonCounter.EventSentPerSec, "Events/sec");
            CounterPublisher.AddCounter(PhotonCounter.InitPerSec, "Connections/sec");
            CounterPublisher.AddCounter(PhotonCounter.OperationReceivePerSec, "Operations/sec");

            CounterPublisher.AddCounter(PhotonCounter.OperationsMiddle, "0ps > 50ms");
            CounterPublisher.AddCounter(PhotonCounter.OperationsSlow, "0ps > 200ms");
            CounterPublisher.AddCounter(OperationsMaxTimeCounter.Instance, "0ps max ms");

            CounterPublisher.Start();

            Protocol.AllowRawCustomValues = true;
            this.PublicIpAddress = PublicIPAddressReader.ParsePublicIpAddress(currentRole.PublicIPAddress);

            log.InfoFormat("current role = {0}, public IP = {1}, gaming TCP port = {2}, gaming UDP port = {3}, locations count = {4}", 
                currentRole.RoleName, currentRole.PublicIPAddress, currentRole.GamingTcpPort, currentRole.GamingUdpPort, currentRole.Locations.Count);
            log.InfoFormat("connect to master = {0}", currentRole.RoleName);

            databaseManager = new DatabaseManager(this);

#if LOCAL
            string databaseConnectionFile = System.IO.Path.Combine(BinaryPath, "assets/database_connection_local.txt");
#else
            string databaseConnectionFile = System.IO.Path.Combine(BinaryPath, "assets/database_connection.txt");
#endif
            databaseConnectionString = File.ReadAllText(databaseConnectionFile).Trim();


            MongoDefaults.MaxConnectionPoolSize = 500;
            
            m_MongoClient = new MongoClient(databaseConnectionString);
            log.InfoFormat("connected to: {0}".Color(LogColor.orange), databaseConnectionFile);

            m_MongoServer = m_MongoClient.GetServer();
            log.InfoFormat("connected to mongo server".Color(LogColor.orange));

            m_DefaultDatabase = m_MongoServer.GetDatabase(GameServerSettings.Default.DatabaseName);
            log.InfoFormat("received default database".Color(LogColor.orange));

           
            DatabaseManager.Setup();

            if(updater != null ) {
                updater.TearDown();
                updater = null;
            }
            updater = new GameUpdater(this);

            this.ConnectToMaster();

            
        } catch (Exception ex) {
            log.Error(ex);
            log.Error(ex.StackTrace);
        }

    }



    protected virtual void InitLogging() {
        LogManager.SetLoggerFactory(Log4NetLoggerFactory.Instance);
        GlobalContext.Properties["Photon:ApplicationLogPath"] = Path.Combine(this.ApplicationRootPath, "log");
        GlobalContext.Properties["LogFileName"] = "GS" + this.ApplicationName;
        XmlConfigurator.ConfigureAndWatch(new FileInfo(Path.Combine(this.BinaryPath, "log4net.config")));
    }

    public static void SetResourcePool(ResourcePool pool) {
        resourcePool = pool;
    }




    protected override PeerBase CreatePeer(InitRequest initRequest)
    {
        return new MmoPeer(initRequest.Protocol, initRequest.PhotonPeer, this);
    }

    protected override ServerPeerBase CreateServerPeer(InitResponse initResponse, object state) {
        Thread.VolatileWrite(ref this.isReconnecting, 0);
        this.MasterPeer = new OutgoingMasterServerPeer(initResponse.Protocol, initResponse.PhotonPeer, this);
        return this.MasterPeer;
    }

    protected override void TearDown()
    {
//#if LOCAL
//        if (roleName.ToLower().Contains("humans")) {
//            NetUtils.SendToSlack("", "LOCAL SERVER WAS RESTARTED");
//        }
//#endif
        //if (PlayerGroups != null) {
        //    PlayerGroups.Dispose();
        //    PlayerGroups = null;
        //}
        log.InfoFormat("TearDown: serverId={0}", ServerId);
        if (this.MasterPeer != null) {
            if (updater != null) {
                updater.Stop();
            }
            this.MasterPeer.Disconnect();
        }

        if(updater != null ) {
            updater.TearDown();
            updater = null;
        }
    }

    protected override void OnStopRequested() {

        MmoWorldCache.Instance(this).SaveState();

        log.InfoFormat("OnStopRequested: serverId={0}", ServerId);
        if (this.masterConnectRetryTimer != null) {
            this.masterConnectRetryTimer.Dispose();
        }
        if (this.MasterPeer != null) {
            this.MasterPeer.Disconnect();
        }
        if(updater != null ) {
            updater.TearDown();
            updater = null;
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
        log.InfoFormat("connect to master at ip = {0}", endPoint.Address.ToString());
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
        log.InfoFormat("ConnectToMaster()");
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
        log.InfoFormat("reconnect to master interval = {0} [dy]", ConnectRetryIntervalSeconds);

        this.masterConnectRetryTimer = new Timer(o => this.ConnectToMaster(), null, this.ConnectRetryIntervalSeconds, 0);
    }



    private static void AppDomain_OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        log.Error(e.ExceptionObject);
    }

    public static ResourcePool ResourcePool()
    {
        return resourcePool;
    }


#region Master operations
    public void MasterUpdateShipModule(string gameRefID, string characterID, ShipModelSlotType slotType, string templateID ) {
        UpdateShipModel op = new UpdateShipModel {
            CharacterId = characterID,
            GameRefId = gameRefID,
            SlotType = (byte)slotType,
            TemplateId = templateID
        };
        OperationRequest request = new OperationRequest((byte)ServerToServerOperationCode.UpdateShipModel, op);
        this.MasterPeer.SendOperationRequest(request, new SendParameters());
    }

    public void MasterUpdateCharacter(string gameRefId, string characterId, PlayerCharacter info, Hashtable model, string world, int exp) {
        UpdateCharacter op = new UpdateCharacter {
            Deleted = false,
            CharacterId = characterId,
            GameRefId = gameRefId,
            Model = model,
            Race = (byte)info.Race,
            Workshop = (byte)info.Workshop,
             Exp = exp,
              WorldId = world
        };
        OperationRequest request = new OperationRequest((byte)ServerToServerOperationCode.UpdateCharacter, op);
        this.MasterPeer.SendOperationRequest(request, new SendParameters());
    }
#endregion
}


