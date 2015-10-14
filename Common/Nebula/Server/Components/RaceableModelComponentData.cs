using Common;
using System.Xml.Linq;

namespace Nebula.Server.Components {
    public class RaceableModelComponentData : ModelComponentData {

        public string humanModel { get; private set; }
        public string borguzandModel { get; private set; }
        public string criptizidModel { get; private set; }

        public RaceableModelComponentData(XElement componentElement) : 
            base(componentElement){
            if(componentElement.HasAttribute("h")) {
                humanModel = componentElement.GetString("h");
            }
            if(componentElement.HasAttribute("b")) {
                borguzandModel = componentElement.GetString("b");
            }
            if(componentElement.HasAttribute("c")) {
                criptizidModel = componentElement.GetString("c");
            }
        }

        public RaceableModelComponentData(string humamModel, string borguzandModel, string criptizidModel, string defaultModel)
            : base(defaultModel) {
            this.humanModel = humamModel;
            this.borguzandModel = borguzandModel;
            this.criptizidModel = criptizidModel;
        }

        public override ComponentSubType subType {
            get {
                return ComponentSubType.raceable_model;
            }
        }
    }
}
