using Common;
#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif
namespace Nebula.Client.Pets {
    public class PetSkillData {
        private int m_Id;
        private float m_Cooldown;
        private string m_Name;
        private string m_Desc;
        private string m_Icon;

        public PetSkillData(object elementObj) {
#if UP
            var element = elementObj as UPXElement;
#else
            var element = elementObj as XElement;
#endif
            m_Id = element.GetInt("id");
            m_Cooldown = element.GetFloat("cooldown");
            m_Name = element.GetString("name");
            m_Desc = element.GetString("desc");
            m_Icon = element.GetString("icon");
        }

        public int id {
            get {
                return m_Id;
            }
        }

        public float cooldown {
            get {
                return m_Cooldown;
            }
        }

        public string name {
            get {
                return m_Name;
            }
        }

        public string description {
            get {
                return m_Desc;
            }
        }

        public string icon {
            get {
                return m_Icon;
            }
        }
    }
}
