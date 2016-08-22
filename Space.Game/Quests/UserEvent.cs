using Common;

namespace Nebula.Quests {

    public class UserEvent {
        private UserEventName m_Name;
        private object m_Data;

        public UserEvent(UserEventName name, object data = null ) {
            m_Name = name;
            m_Data = data;
        }

        public UserEventName name {
            get {
                return m_Name;
            }
        }

        public object data {
            get {
                return m_Data;
            }
        }

        public string dataString {
            get {
                if(data != null ) {
                    if(data is string ) {
                        return (data as string);
                    }
                }
                return null;
            }
        }
    }
}
