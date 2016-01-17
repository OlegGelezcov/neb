//#define UP
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Nebula.Client {
    public abstract class KeyValueTable<T, U> {
        private Dictionary<T, U> m_Dict;

        public abstract void Load(object elementObj);

        public U this[T key] {
            get {
                if(m_Dict == null ) {
                    m_Dict = new Dictionary<T, U>();
                }
                U result;
                if(m_Dict.TryGetValue(key, out result)) {
                    return result;
                } else {
                    return (typeof(U).IsValueType) ? (U)Activator.CreateInstance(typeof(U)) : default(U);
                }
            }
            set {
                if(m_Dict == null ) {
                    m_Dict = new Dictionary<T, U>();
                }
                if(m_Dict.ContainsKey(key)) {
                    m_Dict.Remove(key);
                }
                m_Dict.Add(key, value);
            }
        }

        protected Dictionary<T, U> dict {
            get {
                if(m_Dict == null ) {
                    m_Dict = new Dictionary<T, U>();
                }
                return m_Dict;
            }
        }
    }
}
