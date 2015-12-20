// VkontakteId.cs
// Nebula
//
// Created by Oleg Zheleztsov on Thursday, December 17, 2015 11:09:56 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
namespace Nebula.Server.Login {

    /// <summary>
    /// Incapsulate operations with vkontakte id
    /// </summary>
    public class VkontakteId {
        private string m_Vkontakte;

        public VkontakteId(string vkid) {
            m_Vkontakte = vkid;
        }

        /// <summary>
        /// Return not null string for vkontakte id
        /// </summary>
        public string value {
            get {
                if(m_Vkontakte == null ) {
                    m_Vkontakte = string.Empty;
                }
                return m_Vkontakte;
            }
        }
    }
}
