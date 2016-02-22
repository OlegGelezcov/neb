using Common;
using Space.Game;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Nebula.Contracts {
    public class BaseContractElementCollection<T> where T : ElementBaseData, new() {
        private List<T> m_Elements;

        public BaseContractElementCollection(XElement parent, string elementName ) {
            m_Elements = new List<T>();
            var dump = parent.Elements(elementName).Select(e => {
                T item = new T();
                item.Load(e);
                m_Elements.Add(item);
                return item;
            }).ToList();
        }

        public BaseContractElementCollection() {
            m_Elements = new List<T>();
        }

        public List<T> Available(Race race, int level) {
            List<T> filtered = new List<T>();
            foreach(var e in m_Elements) {
                if(e.IsValidRace(race) && e.IsValidLevel(level)) {
                    filtered.Add(e);
                }
            }
            return filtered;
        }

        public bool Has(Race race, int level) {
            return Available(race, level).Count > 0;
        }

        public T GetRandom(Race race, int level) {
            var filtered = Available(race, level);
            if(filtered.Count > 0 ) {
                return filtered.AnyElement();
            }
            return null;
        }

        public List<T> Get(Race race) {
            return Available(race, int.MaxValue);
        }

        public int GetCount(Race race) {
            return Get(race).Count;
        }
        public int GetCount(Race race, int level) {
            return Available(race, level).Count;
        }
    }
}
