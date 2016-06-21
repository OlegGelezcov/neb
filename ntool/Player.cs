using Nebula.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ntool {
    public class Player {

        private string m_Login;
        private string m_GameRef;
        private ClientPlayerCharactersContainer m_characters;


        public void SetLogin(string login ) {
            m_Login = login;
        }

        public void SetGameRef(string gameRef) {
            m_GameRef = gameRef;
        }

        public void SetCharacters(ClientPlayerCharactersContainer characters) {
            m_characters = characters;
        }

        public string login {
            get {
                return m_Login.ToLower();
            }
        }

        public string gameRef {
            get {
                return m_GameRef;
            }
        }

        public ClientPlayerCharactersContainer characters {
            get {
                return m_characters;
            }
        }
    }
}
