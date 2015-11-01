using MongoDB.Bson;

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
            expireTime += LoginApplication.Instance.serverSettings.timeForPass;
        }

        public void IncrementPasses() {
            passes++;
        }

        public void DecrementPasses() {
            passes--;
        }
    }
}
