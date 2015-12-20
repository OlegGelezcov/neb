// LoginAuth.cs
// Nebula
//
// Created by Oleg Zheleztsov on Thursday, December 17, 2015 11:18:10 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//

namespace Nebula.Server.Login {

    /// <summary>
    /// Represent user permissions ( login and password )
    /// </summary>
    public class LoginAuth {
        private LoginId m_Login;
        private string m_Password;

        public LoginAuth(string login, string password) {
            m_Login = new LoginId(login);
            m_Password = password;
        }

        public string login {
            get {
                return m_Login.value;
            }
        }

        public string password {
            get {
                return m_Password;
            }
        }
    }
}
