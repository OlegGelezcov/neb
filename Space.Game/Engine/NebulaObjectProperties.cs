using Common;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Engine {
    public class NebulaObjectProperties : NebulaBehaviour {

        public ConcurrentDictionary<byte, object> properties { get; private set; }
        public int propertiesRevision { get; private set; }


        private void CheckProperties() {
            if(properties == null ) {
                properties = new ConcurrentDictionary<byte, object>();
                propertiesRevision = 0;
            }
        }

        public void SetProperties(Hashtable propertiesSet, ArrayList propertiesUnset) {
            CheckProperties();

            if(propertiesSet != null ) {
                var dictPropertiesSet = Convert(propertiesSet);
                foreach(var newProp in dictPropertiesSet) {
                    if(properties.ContainsKey(newProp.Key)) {
                        object val = null;
                        properties.TryRemove(newProp.Key, out val);
                    }
                    properties.TryAdd(newProp.Key, newProp.Value);
                }
            }

            if(propertiesUnset != null ) {
                foreach(object prop in propertiesUnset) {
                    object val = null;
                    properties.TryRemove((byte)prop, out val);
                }
            }

            propertiesRevision++;
        }

        public void SetProperty(byte pname, object pvalue) {
            CheckProperties();

            if(properties.ContainsKey(pname)) {
                object val = null;
                properties.TryRemove(pname, out val);
            }
            properties.TryAdd(pname, pvalue);
            propertiesRevision++;
        }



        public Hashtable raw {
            get {
                CheckProperties();

                Hashtable hash = new Hashtable();
                foreach(var prop in properties) {
                    hash.Add(prop.Key, prop.Value);
                }
                return hash;
            }
        }

        private ConcurrentDictionary<byte, object> Convert(Hashtable props) {
            var dict = new ConcurrentDictionary<byte, object>();
            foreach(DictionaryEntry entry in props) {
                dict.TryAdd((byte)entry.Key, entry.Value);
            }
            return dict;
        }

        public bool Contains(byte key) {
            CheckProperties();
            return properties.ContainsKey(key);
        }

        public object GetProperty(byte key) {
            CheckProperties();
            return properties[key];
        }

        public override int behaviourId {
            get {
                return (int)ComponentID.Properties;
            }
        }
    }
}
