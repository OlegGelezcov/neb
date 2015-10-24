using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Login {
    public struct LoggedUser {

        private string mLogin;
        private string mGameRef;
        
        public void SetLogin(string inLogin) {
            mLogin = inLogin;
        } 

        public void SetGameRef(string inGameRef) {
            mGameRef = inGameRef;
        }

        public string login {
            get {
                return mLogin;
            }
        } 

        public string gameRef {
            get {
                return mGameRef;
            }
        }
    }
}
