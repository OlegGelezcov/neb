using Common;
using Nebula.Pets;
using System.Linq;
using System.Xml.Linq;

namespace Nebula.Resources {
    public class PetParameters {
        //public PetFFParameterCollection hpParameters { get; private set; }
        //public PetFFParameterCollection damageParameters { get; private set; }
        //public PetIFParameterCollection masteryParameters { get; private set; }

        public IPetParamResource damage { get; private set; }
        public IPetParamResource hp { get; private set; }
        public IPetParamResource od { get; private set; }
        public IPetParamResource cooldown { get; private set; }
        public PetColorDropDataResource petColorDropResource { get; private set; }
        public PetActiveSkillCountTable activeSkillCountTable { get; private set; }
        public PetPassiveSkillCountTable passiveSkillCountTable { get; private set; }
        public PetMasteryTable masteryTable { get; private set; }
        public PetTypeTable typeTable { get; private set; }
        public PetUpgradeTable petUpgrades { get; private set; }
        public PetMasteryUpgradeTable masteryUpgrades { get; private set; }
        public PetDefaultModelTable defaultModels { get; private set; }

        public void Load(string file) {

            XDocument document = XDocument.Load(file);
            damage          = new MainPetParameterResource(document.Element("pets").Element("attack")   );
            hp              = new MainPetParameterResource(document.Element("pets").Element("hp")       );
            od              = new MainPetParameterResource(document.Element("pets").Element("od")       );
            cooldown        = new MainPetParameterResource(document.Element("pets").Element("cd")       );
            petColorDropResource = new PetColorDropDataResource(document.Element("pets").Element("colors"));
            activeSkillCountTable = new PetActiveSkillCountTable(document.Element("pets").Element("active_skills"));
            passiveSkillCountTable = new PetPassiveSkillCountTable(document.Element("pets").Element("passive_skills"));
            masteryTable = new PetMasteryTable(document.Element("pets").Element("mastery"));
            typeTable = new PetTypeTable(document.Element("pets").Element("types"));
            petUpgrades = new PetUpgradeTable(document.Element("pets").Element("upgrades"));
            masteryUpgrades = new PetMasteryUpgradeTable(document.Element("pets").Element("mastery_upgrades"));
            defaultModels = new PetDefaultModelTable(document.Element("pets").Element("default_model"));

            /*
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
            }).ToList();*/


        }
    }
}
