using Common;

using System.Linq;
using System.Xml.Linq;

namespace Nebula.Resources {
    public class PetParameters {
        public PetFFParameterCollection hpParameters { get; private set; }
        public PetFFParameterCollection damageParameters { get; private set; }
        public PetIFParameterCollection masteryParameters { get; private set; }

        public void Load(string file) {
            XDocument document = XDocument.Load(file);
            hpParameters = new PetFFParameterCollection();
            damageParameters = new PetFFParameterCollection();
            masteryParameters = new PetIFParameterCollection();

            var dump1 = document.Element("pets").Element("hp").Elements("color").Select(colorElement => {
                PetColor color = (PetColor)System.Enum.Parse(typeof(PetColor), colorElement.GetString("name"));
                float baseVal = colorElement.GetFloat("base");
                float valVal = colorElement.GetFloat("value");
                FloatFloatPetParameter parameter = new FloatFloatPetParameter(color, baseVal, valVal);
                hpParameters.AddParameter(parameter);
                return parameter;
            }).ToList();

            var dump2 = document.Element("pets").Element("damage").Elements("color").Select(colorElement => {
                PetColor color = (PetColor)System.Enum.Parse(typeof(PetColor), colorElement.GetString("name"));
                float baseVal = colorElement.GetFloat("base");
                float valVal = colorElement.GetFloat("value");
                FloatFloatPetParameter parameter = new FloatFloatPetParameter(color, baseVal, valVal);
                damageParameters.AddParameter(parameter);
                return parameter;
            }).ToList();

            var dump3 = document.Element("pets").Element("mastery").Elements("color").Select(colorElement => {
                PetColor color = (PetColor)System.Enum.Parse(typeof(PetColor), colorElement.GetString("name"));
                int index = colorElement.GetInt("index");
                float valVal = colorElement.GetFloat("value");
                IntFloatPetParameter parameter = new IntFloatPetParameter(color, index, valVal);
                masteryParameters.AddParameter(parameter);
                return parameter;
            }).ToList();
        }
    }
}
