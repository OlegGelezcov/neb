using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Nebula.Resources {
    public abstract class KeyValueTable<T, U> {

        private ConcurrentDictionary<T, U> m_Dict;

        public abstract void Load(XElement element);

        public U this[T key] {
            get {
                if(m_Dict == null ) {
                    m_Dict = new ConcurrentDictionary<T, U>();
                }
                U result;
                if(m_Dict.TryGetValue(key, out result)) {
                    return result;
                } else {
                    return (typeof(U).IsValueType) ? (U)Activator.CreateInstance(typeof(U)) : default(U);
                }
            }
            set {
                if(m_Dict == null) {
                    m_Dict = new ConcurrentDictionary<T, U>();
                }
                m_Dict.TryAdd(key, value);
            }
        }

        protected ConcurrentDictionary<T, U> dict {
            get {
                if(m_Dict == null ) {
                    m_Dict = new ConcurrentDictionary<T, U>();
                }
                return m_Dict;
            }
        }
    }
}
