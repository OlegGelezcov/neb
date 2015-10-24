using Common;
using Login.Events;
using Photon.SocketServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Login {
    public class LoggedInUser {       
        public readonly string login;
        public readonly string gameRef;
        public readonly LoginClientPeer peer;

        private LoggedInUser() { }

        public LoggedInUser(string login, string gameRef, LoginClientPeer peer) {
            this.login = login;
            this.gameRef = gameRef;
            this.peer = peer;
        }

        public void SendPassesUpdateEvent(DbUserLogin dbUser ) {
            if(login == dbUser.login && gameRef == dbUser.gameRef) {
                if(peer != null && peer.Connected ) {
                    PassesUpdateEvent eventObject = new PassesUpdateEvent {
                        passes = dbUser.passes,
                        currentTime = CommonUtils.SecondsFrom1970(),
                        expireTime = dbUser.expireTime
                    };
                    EventData eventData = new EventData((byte)LoginEventCode.PassesUpdate, eventObject);
                    peer.SendEvent(eventData, new SendParameters());
                }
            }
        }
    }
}
