using Common;
using Space.Game;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Space
{
    public class ServerRuntimeStats
    {
        private static ServerRuntimeStats stats;

        public static ServerRuntimeStats Default(GameApplication app)
        {
            if (stats == null)
                stats = new ServerRuntimeStats(app);
            return stats;
        }

        private GameApplication m_App;

        private ServerRuntimeStats(GameApplication app) {
            m_App = app;
        }

        private Dictionary<ItemType, int> createdItems = new Dictionary<ItemType, int>();
        private object createdSync = new object();

        private Dictionary<ItemType, int> disposedItems = new Dictionary<ItemType, int>();
        private object disposedSync = new object();

        public void CreateItem(byte itemType)
        {
            lock(createdSync)
            {
                var type = itemType.toItemType();
                if (this.createdItems.ContainsKey(type))
                    this.createdItems[type]++;
                else
                    this.createdItems.Add(type, 1);
            }
        }

        public void DisposeItem(byte itemType)
        {
            lock(disposedSync)
            {
                var type = itemType.toItemType();
                if (this.disposedItems.ContainsKey(type))
                    this.disposedItems[type]++;
                else
                    this.disposedItems.Add(type, 1);
            }
        }

        public void OutStats(ILogContext context)
        {
            Hashtable createdItemsHash = new Hashtable();
            Hashtable disposedItemsHash = new Hashtable();

            lock (createdSync)
            {
                createdItemsHash = this.createdItems.toHash<ItemType, int>();
            }
            lock(disposedSync)
            {
                disposedItemsHash = this.disposedItems.toHash<ItemType, int>();
            }

            Hashtable resultInfo = new Hashtable
            {
                {"Created Items", createdItemsHash},
                {"Disposed Items", disposedItemsHash },
                {"Worlds", MmoWorldCache.Instance(m_App).GetStats() }
            };

            var builder = resultInfo.ToStringBuilder();
            context.Log(LogFilter.STATS, builder.ToString());
        }
    }
}
