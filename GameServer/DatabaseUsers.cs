using Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Nebula {
    public class DatabaseUsers {

        private ConcurrentDictionary<string, DatabaseUser> mUsers;

        public void Load(string basePath) {
            XDocument document = XDocument.Load(Path.Combine(basePath, "assets/database.xml"));
            mUsers = new ConcurrentDictionary<string, DatabaseUser>();
            var dumpList = document.Element("database").Elements("role").Select(roleElement => {
                string roleName = roleElement.GetString("name").ToLower().Trim();
                string user = roleElement.GetString("user");
                string password = roleElement.GetString("password");
                DatabaseUser userObj = new DatabaseUser(roleName, user, password);
                mUsers.TryAdd(roleName, userObj);
                return userObj;
            }).ToList();
        }

        public DatabaseUser GetUser(string role) {
            DatabaseUser user;
            if(mUsers.TryGetValue(role.ToLower().Trim(), out user)) {
                return user;
            }
            return null;
        }
    }

    public class DatabaseUser {
        public string role { get; }
        public string user { get; }
        public string password { get; }

        public DatabaseUser(string inRole, string inUser, string inPassword) {
            role = inRole;
            user = inUser;
            password = inPassword;
        }

        public string ConnectionString(string ip) {
            return "mongodb://" + user + ":" + password + "@" + ip + "/nebula";
        }
    }
}
