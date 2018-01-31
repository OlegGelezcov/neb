using Common;
using ExitGames.Client.Photon;
using Nebula.Resources.Inaps;
using System.Linq;
#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif

namespace Nebula.Client.Inaps {

    /// <summary>
    /// Inap for nebula credits
    /// </summary>
    public class InapObject : IInapObject {

        private string m_Id;
        private string m_Name;
        private string m_Description;
        private int m_Price;
        private InapObjectType m_Type;
        private bool m_Consumable;
        private int m_Tag;
        private Hashtable m_Data;
        private string m_Icon;
        private string m_MailTitle;
        private string m_MailBody;
        private bool m_Available;
        private string m_Detail;
        private bool m_Visible;

        #region IInapObject
        public string Icon {
            get {
                return icon;
            }
        }

        public string Name {
            get {
                return name;
            }
        }

        public string Description {
            get {
                return description;
            }
        }

        public int Price {
            get {
                return price;
            }
        }

        public CoinType CoinType {
            get {
                return CoinType.nebula_credits;
            }
        } 
        #endregion

        public string detail {
            get {
                return m_Detail;
            }
        }

        public bool available {
            get {
                return m_Available;
            }
        }

        public string mailTitle {
            get {
                return m_MailTitle;
            }
        }

        public string mailBody {
            get {
                return m_MailBody;
            }
        }

        public string id {
            get {
                return m_Id;
            }
        }

        public string name {
            get {
                return m_Name;
            }
        }

        public string description {
            get {
                return m_Description;
            }
        }

        public int price {
            get {
                return m_Price;
            }
        }

        public InapObjectType type {
            get {
                return m_Type;
            }
        }

        public bool consumable {
            get {
                return m_Consumable;
            }
        }

        public int tag {
            get {
                return m_Tag;
            }
        }

        public Hashtable data {
            get {
                return m_Data;
            }
        }

        public string icon {
            get {
                return m_Icon;
            }
        }

        public bool visible {
            get {
                return m_Visible;
            }
        }

#if UP
        public InapObject(UPXElement element) {
            m_Id = element.GetString("id");
            m_Name = element.GetString("name");
            m_Description = element.GetString("description");
            m_Price = element.GetInt("price");
            m_Type = (InapObjectType)System.Enum.Parse(typeof(InapObjectType), element.GetString("type"));
            m_Consumable = element.GetBool("consumable");
            m_Tag = element.GetInt("tag");
            m_Icon = element.GetString("icon");
            m_MailTitle = element.GetString("mail_title");
            m_MailBody = element.GetString("mail_body");
            m_Available = element.GetBool("available");
            m_Detail = element.GetString("detail");
            m_Visible = element.GetBool("visible");

            m_Data = new Hashtable();
            var inputsElement = element.Element("inputs");
            if (inputsElement != null) {
                var dump = inputsElement.Elements("input").Select(ie => {
                    string key = ie.GetString("key");
                    object val = CommonUtils.ParseValue(ie.GetString("value"), ie.GetString("type"));
                    m_Data.Add(key, val);
                    return key;
                }).ToList();
            }
        }
#else
        public InapObject(XElement element) {
            m_Id = element.GetString("id");
            m_Name = element.GetString("name");
            m_Description = element.GetString("description");
            m_Price = element.GetInt("price");
            m_Type = (InapObjectType)System.Enum.Parse(typeof(InapObjectType), element.GetString("type"));
            m_Consumable = element.GetBool("consumable");
            m_Tag = element.GetInt("tag");
            m_Icon = element.GetString("icon");
            m_MailTitle = element.GetString("mail_title");
            m_MailBody = element.GetString("mail_body");
            m_Available = element.GetBool("available");
            m_Detail = element.GetString("detail");
            m_Visible = element.GetBool("visible");

            m_Data = new Hashtable();
            var inputsElement = element.Element("inputs");
            if(inputsElement != null ) {
                var dump = inputsElement.Elements("input").Select(ie => {
                    string key = ie.GetString("key");
                    object val = CommonUtils.ParseValue(ie.GetString("value"), ie.GetString("type"));
                    m_Data.Add(key, val);
                    return key;
                }).ToList();
            }
        }
#endif


    }
}
