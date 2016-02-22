namespace Nebula.Client.Achievments {
    public class AchievmentTierData {
        public int id { get; private set; }
        //public string Name(string) {  }
        public int count { get; private set; }
        public int points { get; private set; }


        public AchievmentTierData(UniXMLElement element) {
            id              = element.GetInt("id");
            //name            = element.GetString("name");
            count = element.GetInt("count");
            points = element.GetInt("points");
        }

        public bool Unlocked(int cnt) {
            return cnt >= count;
        }

        public float Percent(int cnt) {
            float pc = (float)cnt / (float)count;
            if(pc > 1 ) {
                pc = 1.0f;
            }
            return pc;
        }
    }
}
