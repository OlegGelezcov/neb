using Common;
using MongoDB.Bson;
using Nebula.Server.Login;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Login {

    public class StatCollection : ConcurrentDictionary<string, DbUserStat> {

        private LoginApplication m_Application;

        public StatCollection(LoginApplication app) {
            m_Application = app;
        }

        public void OnUserLoggedIn(FullUserAuth auth, string platform) {
            if(!ContainsKey(auth.login)) {
                var stat = m_Application.DbUserLogins.GetStat(auth.login);
                TryAdd(stat.login, stat);
            }

            DbUserStat existingStat = null;
            if(TryGetValue(auth.login, out existingStat)) {
                existingStat.platform = platform;
                existingStat.sessions.Add(CommonUtils.SecondsFrom1970());
            }
        }

        public void OnLogOut(LoginId login) {
            DbUserStat stat;
            if(TryGetValue(login.value, out stat)) {
                m_Application.DbUserLogins.SaveStat(stat);
                TryRemove(login.value, out stat);
            }
        }
    }

    public class DbUserStat { 
        public ObjectId Id { get; set; }
        public string login { get; set; }
        public string platform { get; set; }
        public List<int> sessions { get; set; }
    }

}
