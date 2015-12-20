using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Server.Login {
    public class LoginGameRef {
        private LoginId m_Login;
        private GameRefId m_GameRef;

        public LoginGameRef(string login, string gameRef) {
            m_Login = new LoginId(login);
            m_GameRef = new GameRefId(gameRef);
        }

        public string login {
            get {
                return m_Login.value;
            }
        }

        public string gameRef {
            get {
                return m_GameRef.value;
            }
        }
    }
}
