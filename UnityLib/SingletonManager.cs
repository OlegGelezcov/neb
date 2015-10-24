using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula {
    public static class SingletonManager {

        private static readonly Dictionary<string, SingletonManageData> sSingletons = new Dictionary<string, SingletonManageData>();

        public static void Register(IManagedSingleton sing) {
            if(!sSingletons.ContainsKey(sing.singletonName)) {
                sSingletons.Add(sing.singletonName, new SingletonManageData(sing));
            }
        }

        public static bool IsRegistered(IManagedSingleton sing) {
            return sSingletons.ContainsKey(sing.singletonName);
        }

        public static bool TryAwake(IManagedSingleton sing) {
            if(!IsRegistered(sing)) {
                Register(sing);
            }
            SingletonManageData data = null;
            if(sSingletons.TryGetValue(sing.singletonName, out data)) {
                if(data.awaked) {
                    return false;
                } else {
                    data.SetAwaked();
                    return true;
                }

            } else {
                throw new Exception("Singleton data not founded");
            }
        }

        private static void Clear() {
            sSingletons.Clear();
        }

        public static void ManagedDestroyAll() {
            foreach(var pSing in sSingletons) {
                pSing.Value.ManagedDestroy();
            }
            Clear();
        }
    }

    public class SingletonManageData {
        public readonly string singletonName;
        public bool awaked { get; private set; }

        public readonly IManagedSingleton singleton;

        public SingletonManageData(IManagedSingleton singleton) {
            singletonName = singleton.singletonName;
            awaked = false;
            this.singleton = singleton;
        }

        public void SetAwaked() {
            awaked = true;
        }

        public void UnsetAwaked() {
            awaked = false;
        }


        public void ManagedDestroy() {
            if(singleton != null ) {
                singleton.ManagedDestroy();
            }
        }
    }
}
