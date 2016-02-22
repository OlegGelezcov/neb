using Common;
using System.Xml.Linq;

namespace Nebula.Achievments {
    public class AchievmentTierData {
        public int id { get; private set; }
        public int count { get; private set; }
        public int points { get; private set; }

        public AchievmentTierData(XElement element) {
            id = element.GetInt("id");
            count = element.GetInt("count");
            points = element.GetInt("points");
        }

        public bool UnlockedFor(int counter ) {
            return (counter >= count);
        }

        public bool IsUnlockedByValues(int oldCount, int newCount ) {
            return (false == UnlockedFor(oldCount)) && (true == UnlockedFor(newCount));
        }

    }
}
