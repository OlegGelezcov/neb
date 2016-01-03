using Common;
using System.Collections.Concurrent;
using System.Linq;
using System.Xml.Linq;

namespace Nebula.Pets {
    public class PetSkillInfo {
        private int m_Id;
        private float m_Prob;
        private float m_Cooldown;
        private ConcurrentDictionary<string, object> m_Inputs;

        public int id {
            get {
                return m_Id;
            }
        }

        public float prob {
            get {
                return m_Prob;
            }
        }

        public float cooldown {
            get {
                return m_Cooldown;
            }
        }

        public ConcurrentDictionary<string, object> inputs {
            get {
                return m_Inputs;
            }
        }

        public PetSkillInfo(XElement element) {
            m_Id = element.GetInt("id");
            m_Prob = element.GetFloat("prob");
            m_Cooldown = element.GetFloat("cooldown");

            m_Inputs = new ConcurrentDictionary<string, object>();
            if(element.Element("inputs") != null ) {
                var dump = element.Element("inputs").Elements("input").Select(inputElement => {
                    string key = inputElement.GetString("key");
                    object val = CommonUtils.ParseValue(inputElement.GetString("value"), inputElement.GetString("type"));
                    m_Inputs.TryAdd(key, val);
                    return key;
                }).ToList();
            }
        }
    }
}
