using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Login {
    public class DbUserLogin {
        public ObjectId Id { get; set; }
        public string login { get; set; }
        public string password { get; set; }
        public string email { get; set; }
        public string gameRef { get; set; }
        public int creationTime { get; set; }
        public int passes { get; set; }
        public int expireTime { get; set; }

        public void MoveSinglePassToTime() {
            passes--;
            expireTime += DbReader.TIME_FOR_PASS;
        }

        public void IncrementPasses() {
            passes++;
        }

        public void DecrementPasses() {
            passes--;
        }
    }
}
