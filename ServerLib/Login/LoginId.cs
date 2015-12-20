// LoginId.cs
// Nebula
//
// Created by Oleg Zheleztsov on Thursday, December 17, 2015 11:05:05 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
namespace Nebula.Server.Login {

    /// <summary>
    /// Incapsulate login of user for case independent manipulation with string
    /// </summary>
    public class LoginId {

        private string m_Login;

        public LoginId(string login) {
            m_Login = login;
        }

        public string value {
            get {
                if(m_Login == null ) {
                    m_Login = string.Empty;
                }
                return m_Login.ToLower();
            }
        }
    }
}
