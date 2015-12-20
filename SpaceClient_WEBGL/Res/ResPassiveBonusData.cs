using Common;
#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif
namespace Nebula.Client.Res {
    public class ResPassiveBonusData {
        public PassiveBonusType type { get; private set; }
        public float timeToSingleTier { get; private set; }
        public float valueForTier { get; private set; }
        public int elementsForTier { get; private set; }
        public string nebulaElementID { get; private set; }

        public string name { get; private set; }
        public string description { get; private set; }
        public string icon { get; private set; }

        public int CountElementsForTier(int tier ) {
            return elementsForTier * tier;
        }

#if UP
        public ResPassiveBonusData(PassiveBonusType inType, UPXElement element) {
            type = inType;
            timeToSingleTier = element.GetFloat("time");
            valueForTier = element.GetFloat("value");
            elementsForTier = element.GetInt("elements");
            nebulaElementID = element.GetString("element_id");
            name = element.GetString("name");
            description = element.GetString("description");
            icon = element.GetString("icon");
        }
#else
        public ResPassiveBonusData(PassiveBonusType inType, XElement element) {
            type = inType;
            timeToSingleTier    = element.GetFloat  ("time");
            valueForTier        = element.GetFloat  ("value");
            elementsForTier     = element.GetInt    ("elements");
            nebulaElementID     = element.GetString ("element_id");
            name                = element.GetString ("name");
            description         = element.GetString ("description");
            icon                = element.GetString ("icon");
        }
#endif
    }
}
