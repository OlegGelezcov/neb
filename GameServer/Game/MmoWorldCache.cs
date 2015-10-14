namespace Space.Game
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using ExitGames.Threading;
    using Space.Server;
    using GameMath;
    using System.Collections;
    using ExitGames.Logging;

    public sealed class MmoWorldCache : IDisposable
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public static readonly MmoWorldCache Instance = new MmoWorldCache();

        private readonly Dictionary<string, MmoWorld> dict;
        private readonly ReaderWriterLockSlim readWriteLock;

        public Hashtable GetStats()
        {
            using(ReadLock.TryEnter(readWriteLock, Settings.MaxLockWaitTimeMilliseconds))
            {
                Hashtable stats = new Hashtable();
                foreach(var pWorld in dict)
                {
                    stats.Add(pWorld.Key, pWorld.Value.GetStats());
                }
                return stats;
            }
        }

        public void Tick(float deltaTime) {
            foreach(var worldPair in dict) {
                worldPair.Value.Tick(deltaTime);
            }
        }

        private MmoWorldCache()
        {
            this.dict = new Dictionary<string, MmoWorld>();
            this.readWriteLock = new ReaderWriterLockSlim();
        }

        ~MmoWorldCache()
        {
            this.Dispose(false);
        }

        public void Clear()
        {
            SaveState();

            using(WriteLock.TryEnter(this.readWriteLock, Settings.MaxLockWaitTimeMilliseconds))
            {
                foreach(var pWorld in dict) {
                    pWorld.Value.ClearResources();
                }

                this.dict.Clear();
            }
        }

        public void SaveState() {
            log.InfoFormat("Save world cache state [dy]");
            using(WriteLock.TryEnter(this.readWriteLock, Settings.MaxLockWaitTimeMilliseconds)) {
                foreach(var pWorld in dict) {
                    pWorld.Value.SaveWorldState();
                }
            }
        }

        public bool TryCreate(string name, Vector minCorner, Vector maxCorner, Vector tileDimensions, out MmoWorld world, Res resource)
        {
            using(WriteLock.TryEnter(this.readWriteLock, Settings.MaxLockWaitTimeMilliseconds))
            {
                if(this.dict.TryGetValue(name, out world))
                {
                    return false;
                }
                world = new MmoWorld(name, minCorner, maxCorner, tileDimensions, resource);
                //world.LoadXml();
                this.dict.Add(name, world);
                return true;
            }
        }

        public bool TryGet(string name, out MmoWorld world )
        {
            using(ReadLock.TryEnter(this.readWriteLock, Settings.MaxLockWaitTimeMilliseconds))
            {
                return this.dict.TryGetValue(name, out world);
            }
        }

        public bool TryGetNames(out string[] names) {
            List<string> listNames = new List<string>();
            using (ReadLock.TryEnter(this.readWriteLock, Settings.MaxLockWaitTimeMilliseconds)) {
                foreach (var key in dict.Keys) {
                    listNames.Add(key);
                }
                names = listNames.ToArray();
                return true;
            }
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
            this.readWriteLock.Dispose();
        }
    }
}