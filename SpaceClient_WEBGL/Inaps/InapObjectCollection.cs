using Nebula.Resources.Inaps;
using System.Collections.Generic;
using System.Linq;

#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif

namespace Nebula.Client.Inaps {
    /// <summary>
    /// Holds inap objects
    /// </summary>
    public class InapObjectCollection {
        private Dictionary<string, InapObject> m_Items = new Dictionary<string, InapObject>();

        public void Load(string xml) {
#if UP
            UPXDocument document = new UPXDocument(xml);
#else
            XDocument document = XDocument.Parse(xml);
#endif

            m_Items = document.Element("game_inaps").Elements("game_inap").Select(inapElement => {
                InapObject inapObject = new InapObject(inapElement);
                return inapObject;
            }).ToDictionary(inapObject => inapObject.id, inapObject => inapObject);
        }

        public Dictionary<string, InapObject> inaps {
            get {
                return m_Items;
            }
        }

        public List<InapObject> Filtered(params InapObjectType[] types ) {
            List<InapObject> filteredList = new List<InapObject>();
            if(types == null ) {
                types = new InapObjectType[] { };
            }

            foreach(var kvp in inaps ) {
                if(types.Contains(kvp.Value.type)) {
                    filteredList.Add(kvp.Value);
                }
            }

            filteredList.Sort((first, second) => {
                return first.id.CompareTo(second.id);
            });

            return filteredList;
        }

        public List<InapObject> Filtered(List<InapObjectType> types) {
            return Filtered(types.ToArray());
        }

        public InapObject GetExpBoostObject(int tag) {
            foreach(var kvp in inaps ) {
                if(kvp.Value.type == InapObjectType.exp_boost) {
                    if(kvp.Value.tag == tag ) {
                        return kvp.Value;
                    }
                }
            }
            return null;
        }

        public InapObject GetInapObject(string id) {
            if(inaps.ContainsKey(id)) {
                return inaps[id];
            }
            return null;
        }

        public bool ContainsMailTitle(string title) {
            foreach(var inap in inaps ) {
                if(inap.Value.mailTitle == title ) {
                    return true;
                }
            }
            return false;
        }

        public bool ContainsMailBody(string body ) {
            foreach(var inap in inaps) {
                if(inap.Value.mailBody == body) {
                    return true;
                }
            }
            return false;
        }

        public InapObject GetInapWithMailTitle(string title) {
            foreach(var inap in inaps) {
                if(inap.Value.mailTitle == title ) {
                    return inap.Value;
                }
            }
            return null;
        }

        public InapObject GetInapWithMailBody(string body) {
            foreach(var inap in inaps) {
                if(inap.Value.mailBody == body ) {
                    return inap.Value;
                }
            }
            return null;
        }
    }

}
