using Common;
using System.Xml.Linq;

namespace Nebula.Resources {
    public class PassiveBonusData {
        public PassiveBonusType type { get; set; }
        public float timeToFirstTier { get; set; }
        public float valueForTier { get; set; }
        public int nebulaElementsForTier { get; set; }

        public string elementID { get; set; }

        public static PassiveBonusData CreateFromXml(PassiveBonusType type, XElement element ) {
            return new PassiveBonusData {
                type = type,
                timeToFirstTier = element.GetFloat("time"),
                valueForTier = element.GetFloat("value"),
                nebulaElementsForTier = element.GetInt("elements"),
                elementID = element.GetString("element_id")
            };
        }

    }
}
