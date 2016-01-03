using System.Collections.Generic;

namespace Nebula.Client {
    public class ServerTechInfo {
        private bool m_Show;
        private string m_EnText;
        private string m_RuText;

        public bool show {
            get {
                return m_Show;
            }
        }

        public string GetText(string lang) {
            if(lang.Trim().ToLower() == "ru") {
                return m_RuText;
            }
            return m_EnText;
        }


        public ServerTechInfo(string json) {
            object obj = MiniJSON.Json.Deserialize(json);
            if(obj != null ) {
                Dictionary<string, object> dict = obj as Dictionary<string, object>;
                if(dict.ContainsKey("show")) {
                    int iShow = 0;
                    if(int.TryParse(dict["show"].ToString(), out iShow)) {
                        m_Show = (iShow != 0) ? true : false;
                    } else {
                        m_Show = false;
                    }
                }

                if(m_Show) {
                    if(dict.ContainsKey("en")) {
                        m_EnText = dict["en"].ToString();
                    } else {
                        m_EnText = string.Empty;
                    }

                    if(dict.ContainsKey("ru")) {
                        m_RuText = dict["ru"].ToString();
                    } else {
                        m_RuText = string.Empty;
                    }
                } else {
                    m_EnText = string.Empty;
                    m_RuText = string.Empty;
                }
            }
        }
    }
}
