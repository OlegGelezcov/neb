namespace Nebula.Server.Login {
    public class FullUserAuth {
        private LoginGameRef m_LoginGameRef;
        private FacebookId m_Facebook;
        private VkontakteId m_Vkontakte;

        public FullUserAuth(string login, string gameRef, string facebookId, string vkontakteId ) {
            m_LoginGameRef = new LoginGameRef(login, gameRef);
            m_Facebook = new FacebookId(facebookId);
            m_Vkontakte = new VkontakteId(vkontakteId);
        }

        public string login {
            get {
                return m_LoginGameRef.login;
            }
        }

        public string gameRef {
            get {
                return m_LoginGameRef.gameRef;
            }
        }

        public string facebookId {
            get {
                return m_Facebook.value;
            }
        }

        public string vkontakteId {
            get {
                return m_Vkontakte.value;
            }
        }
    }
}
