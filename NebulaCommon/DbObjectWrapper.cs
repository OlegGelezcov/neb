using System.Threading;

namespace NebulaCommon {

    /// <summary>
    /// Object wrapper for DB saved objects. Hold information about state of object(modified or not) and object data 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DbObjectWrapper<T>  {

        private int mChanged = 0;

        public T Data { get; set; }
        public bool Changed {
            get {
                return mChanged != 0 ? true : false;
            }
            set {
                int val = (value) ? 1 : 0;
                Interlocked.Exchange(ref mChanged, val);
            }
        }
    }
}
