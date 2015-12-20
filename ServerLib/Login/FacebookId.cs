// FacebookId.cs
// Nebula
//
// Created by Oleg Zheleztsov on Thursday, December 17, 2015 11:07:39 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
namespace Nebula.Server.Login {

    /// <summary>
    /// Incapsulate operations with facbook id
    /// </summary>
    public class FacebookId {

        private string m_Facebook;

        public FacebookId(string fbid) {
            m_Facebook = fbid;
        }

        public string value {
            get {
                if(m_Facebook == null ) {
                    m_Facebook = string.Empty;
                }
                return m_Facebook;
            }
        }
    }
}
