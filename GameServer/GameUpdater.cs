using Common;
using ExitGames.Concurrency.Fibers;
using ExitGames.Logging;
using Nebula.Game;
using Nebula.Game.Utils;
using NebulaCommon;
using NebulaCommon.ServerToServer.Events;
using Photon.SocketServer;
using Space.Database;
using Space.Game;
using Space.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula {
    public class GameUpdater {

        public GameApplication application { get; private set; }

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        private const int UPDATE_INTERVAL = 2000;

        private IDisposable mLoop;
        private static bool mStarted = false;
        private IFiber mFiber;

        private IFiber m_CheckFiber;
        private IDisposable m_CheckLoop;

        private readonly object fiberLock = new object();

        public GameUpdater(GameApplication app) {
            application = app;
            log.InfoFormat("Updater created!!");
            m_CheckFiber = new PoolFiber();
            m_CheckFiber.Start();
            m_CheckLoop = m_CheckFiber.ScheduleOnInterval(CheckUpdate, 15000, 15000);
        }

        public void Start() {
            if (!mStarted) {
                mStarted = true;
                log.InfoFormat("Updater start!");
                Init();               
            } else {
                log.InfoFormat("Loop already started! [red]");
            }
        }

        private void Init() {
            Res.SetLogger(new NebulaLogger());
            for (int i = 0; i < 10; i++) {
                log.Info("NEW SERVER++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
            }
            if (GameApplication.ResourcePool() == null) {
                GameApplication.SetResourcePool(new ResourcePool(GameApplication.Instance.BinaryPath));
            }
            var globalResourceCheck = GameApplication.ResourcePool().Resource("global");

            /*
#if LOCAL
            if(application.CurrentRole().RoleName.ToLower().Contains("human")) {
                log.InfoFormat("connect as: {0} [red]", "mongodb://human:87e898AA@localhost/nebula");
                GameApplication.Instance.DatabaseManager.Setup("mongodb://human:87e898AA@localhost:27017/nebula");
                
            } else if(application.CurrentRole().RoleName.ToLower().Contains("borg")) {
                log.InfoFormat("connect as: {0} [red]", "mongodb://borg:87e898AA@localhost/nebula");
                GameApplication.Instance.DatabaseManager.Setup("mongodb://borg:87e898AA@localhost:27017/nebula");             
            } else if(application.CurrentRole().RoleName.ToLower().Contains("crip")) {
                log.InfoFormat("connect as: {0} [red]", "mongodb://crip:87e898AA@localhost/nebula");
                GameApplication.Instance.DatabaseManager.Setup("mongodb://crip:87e898AA@localhost:27017/nebula");
            } else if(application.CurrentRole().RoleName.ToLower().Contains("neutral")) {
                log.InfoFormat("connect as: {0} [red]", "mongodb://neutral:87e898AA@localhost/nebula");
                GameApplication.Instance.DatabaseManager.Setup("mongodb://neutral:87e898AA@localhost:27017/nebula");
            } else {
                log.InfoFormat("unknown role = {0} [red]", application.CurrentRole().RoleName);
            }
#else

            GameApplication.Instance.DatabaseManager.Setup(GameServerSettings.Default.DatabaseConnectionString);
#endif
*/

            foreach(string loc in application.CurrentRole().Locations) {
                log.InfoFormat("Localtion: " + loc);
            }

            foreach (string locationID in application.CurrentRole().Locations) {
                MmoWorld world;
                Res resource = GameApplication.ResourcePool().Resource(locationID);
                if (!MmoWorldCache.Instance(application).TryCreate(locationID, Settings.CornerMin, Settings.CornerMax, Settings.TileDimensions, out world, resource)) {
                    log.ErrorFormat("error of creating world {0}", locationID);
                }
            }

            foreach (var locationID in application.CurrentRole().Locations) {
                MmoWorld world;
                if (MmoWorldCache.Instance(application).TryGet(locationID, out world)) {
                    world.Initialize();
                } else {
                    log.ErrorFormat("world not found = {0}", locationID);
                }
            }

            log.InfoFormat("before updater start");

            lock(fiberLock) {
                mFiber = new PoolFiber();
                mFiber.Start();
                mLoop = mFiber.ScheduleOnInterval(Update, UPDATE_INTERVAL, UPDATE_INTERVAL);
            }
            log.InfoFormat("after updater start");
        }

        /*
        public IFiber fiber {
            get {
                return mFiber;
            }
        }*/

        private float m_UpdateTime = 0;
        private bool m_UpdateOnceCalled = false;

        private void Update() {
            try {
                //if(GameApplication.Instance.CurrentRole().RoleName == "BorguzandsGame") {
                //    log.InfoFormat("borguzands tick");
                //} else {
                //    log.InfoFormat("invalid role name: {0}", GameApplication.Instance.CurrentRole().RoleName);
                //}
                //log.InfoFormat("server  = {0} tick", GameApplication.Instance.ApplicationName);
                Time.Tick();
                //log.InfoFormat("STARTED: tick at time: {0}, delta = {1}", Time.curtime(), Time.deltaTime());
                MmoWorldCache.Instance(application).Tick(Time.deltaTime());
                m_UpdateTime = CommonUtils.SecondsFrom1970();
                if(false == m_UpdateOnceCalled) {
                    m_UpdateOnceCalled = true;
                }
                //log.InfoFormat("Ticked: {0}", Time.deltaTime());
            } catch (Exception exception) {
                log.Error(exception);
                log.Error(exception.StackTrace);
            }
        }

        private void CheckUpdate() {
            try {
                if (m_UpdateOnceCalled) {
                    float curTime = CommonUtils.SecondsFrom1970();
                    float delta = curTime - m_UpdateTime;
                    if (delta > 300) {
                        
                        log.InfoFormat("server don't response: {0} seconds. Try restart".Color(LogColor.red), delta);
                        m_UpdateOnceCalled = false;
                        DeleteAllFromWorlds();
                        Stop();
                        Start();
                        log.InfoFormat("NO RESPONSE = {0} seconds!!!!".Color(LogColor.red), delta);
                    } else {
                        //log.InfoFormat("check passed success with interval: {0}".Color(LogColor.cyan), delta);
                    }
                }
            } catch(Exception exception ) {
                log.ErrorFormat(exception.Message);
                log.ErrorFormat(exception.StackTrace);
            }
        }

        public void Stop() {
            try {
                if (mStarted) {
                    log.InfoFormat("Updater stop!");
                    if (mLoop != null) {
                        mLoop.Dispose();
                        mLoop = null;
                    }
                    lock(fiberLock) {
                        if (mFiber != null) {
                            mFiber.Dispose();
                            mFiber = null;
                        }
                    }
                    mStarted = false;
                    m_UpdateOnceCalled = false;
                }
            } catch(Exception exception) {
                mLoop = null;
                mFiber = null;
                mStarted = false;
                m_UpdateOnceCalled = false;
                log.ErrorFormat(exception.Message);
                log.ErrorFormat(exception.StackTrace);
            }
        }

        private void DeleteAllFromWorlds() {
            try {
                foreach (var pActor in application.serverActors) {
                    pActor.Value.Peer.Disconnect();
                }
                application.serverActors.Clear();
                MmoWorldCache.Instance(application).Clear();
            } catch(Exception exception) {
                log.ErrorFormat(exception.Message);
                log.ErrorFormat(exception.StackTrace);
            }
        }

        public void Restart() {
            DeleteAllFromWorlds();
            Stop();
            Start();
        }


        public void TearDown() {
            Stop();
            if(m_CheckLoop != null ) {
                m_CheckLoop.Dispose();
                m_CheckLoop = null;
            }
            if(m_CheckFiber != null ) {
                m_CheckFiber.Dispose();
                m_CheckFiber = null;
            }
        }

        public void EnqueueAtUpdateLoop(Action action) {
            lock(fiberLock) {
                mFiber.Enqueue(action);
            }
        }

        public void CallS2SMethod(NebulaCommon.ServerType serverType, string method, object[] arguments) {
            lock(fiberLock) {
                mFiber.Enqueue(() => {
                    try {
                        S2SInvokeMethodStart start = new S2SInvokeMethodStart {
                            arguments = arguments,
                            method = method,
                            sourceServerID = GameApplication.ServerId.ToString(),
                            targetServerType = (byte)serverType
                        };
                        EventData evt = new EventData((byte)S2SEventCode.InvokeMethodStart, start);
                        application.MasterPeer.SendEvent(evt, new SendParameters());
                    } catch (Exception ex) {
                        log.Info("exception");
                        log.Info(ex.Message);
                        log.Info(ex.StackTrace);
                    }
                });
            }
        }

        public void GivePvpPoints(string login, string gameRef, string character, string guild, byte race, int pvpPoints) {
            CallS2SMethod(ServerType.SelectCharacter, "AddPvpPoints", new object[] { login, gameRef, character, guild, race,  pvpPoints });
        }

        public void RequestGuildInfo(string gameRef, string character) {
            CallS2SMethod(ServerType.SelectCharacter, "RequestGuildInfo", new object[] { gameRef, character });
        }

        public void SetCreditsBonus(string characterId, float bonus ) {
            CallS2SMethod(ServerType.SelectCharacter, "SetCreditsBonus", new object[] { characterId, bonus }); 
        }

        //public void SetCreditsBonus(string )

        public void SendS2SWorldRaceChanged(string worldID, byte previousRace, byte currentRace ) {
            lock(fiberLock) {
                mFiber.Enqueue(() => {
                    try {
                        WorldRaceChanged eventInstance = new WorldRaceChanged {
                            worldID = worldID,
                            previousRace = previousRace,
                            currentRace = currentRace
                        };
                        EventData eventData = new EventData((byte)S2SEventCode.WorldRaceChanged, eventInstance);
                        application.MasterPeer.SendEvent(eventData, new SendParameters());
                    } catch (Exception exception) {
                        log.InfoFormat(exception.Message + " [red]");
                        log.InfoFormat(exception.StackTrace + " [red]");
                    }
                });
            }
        }

        public void EnqueueFiberAction(Action action) {
            lock(fiberLock) {
                mFiber.Enqueue(action);
            }
        }
    }
}
