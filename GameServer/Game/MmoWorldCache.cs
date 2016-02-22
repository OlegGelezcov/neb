namespace Space.Game {
    using ExitGames.Logging;
    using GameMath;
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    public sealed class MmoWorldCache : IDisposable
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private static MmoWorldCache s_Instance = null;

        public static  MmoWorldCache Instance(GameApplication app) {
            if(s_Instance == null ) {
                s_Instance = new MmoWorldCache(app);
            }
            return s_Instance;
        }

        private readonly ConcurrentDictionary<string, MmoWorld> dict;
        //private readonly ReaderWriterLockSlim readWriteLock;
        private readonly GameApplication m_App;


        public Hashtable GetStats() {
            Hashtable stats = new Hashtable();
            foreach (var pWorld in dict) {
                stats.Add(pWorld.Key, pWorld.Value.GetStats());
            }
            return stats;
        }

        public void Tick(float deltaTime) {
            foreach(var worldPair in dict) {
                worldPair.Value.Tick(deltaTime);
            }
        }

        private MmoWorldCache(GameApplication app)
        {
            m_App = app;
            this.dict = new ConcurrentDictionary<string, MmoWorld>();
            //this.readWriteLock = new ReaderWriterLockSlim();
        }

        ~MmoWorldCache()
        {
            this.Dispose(false);
        }

        public void Clear()
        {
            SaveState();

            //using(WriteLock.TryEnter(this.readWriteLock, Settings.MaxLockWaitTimeMilliseconds))
            //{
                foreach(var pWorld in dict) {
                    pWorld.Value.ClearResources();
                }

                this.dict.Clear();
            //}
        }

        public void SaveState() {
            log.InfoFormat("Save world cache state [dy]");
            //using(WriteLock.TryEnter(this.readWriteLock, Settings.MaxLockWaitTimeMilliseconds)) {
                foreach(var pWorld in dict) {
                    pWorld.Value.SaveWorldState();
                }
            //}
        }

        public void SendWorldRaceChanged(Hashtable info ) {
            //using (ReadLock.TryEnter(this.readWriteLock, Settings.MaxLockWaitTimeMilliseconds)) {
                foreach(var pWorld in dict ) {
                    pWorld.Value.SendWorldChangedToPlayers(info);
                }
            //}
        }

        public bool TryCreate(string name, Vector minCorner, Vector maxCorner, Vector tileDimensions, out MmoWorld world, Res resource)
        {
            //using(WriteLock.TryEnter(this.readWriteLock, Settings.MaxLockWaitTimeMilliseconds))
            //{
                if(this.dict.TryGetValue(name, out world))
                {
                    return false;
                }
                world = new MmoWorld(name, minCorner, maxCorner, tileDimensions, resource, m_App);
            //world.LoadXml();
            if (this.dict.TryAdd(name, world)) {
                return true;
            }
            return false;
            //}
        }

        public bool TryGet(string name, out MmoWorld world )
        {
            //using(ReadLock.TryEnter(this.readWriteLock, Settings.MaxLockWaitTimeMilliseconds))
            //{
                return this.dict.TryGetValue(name, out world);
            //}
        }

        public bool TryGetNames(out string[] names) {
            List<string> listNames = new List<string>();
            //using (ReadLock.TryEnter(this.readWriteLock, Settings.MaxLockWaitTimeMilliseconds)) {
                foreach (var key in dict.Keys) {
                    listNames.Add(key);
                }
                names = listNames.ToArray();
                return true;
            //}
        }


        public void Dispose()
        {
            this.Dispose(true);
            //GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if(disposing)
            {
                foreach(MmoWorld world in this.dict.Values)
                {
                    world.Dispose();
                }
            }
            //this.readWriteLock.Dispose();
        }
    }
}