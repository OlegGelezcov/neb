using Common;
using System.Xml.Linq;

namespace Nebula.Server.Components {
    public abstract class ActivatorComponentData : MultiComponentData {

        public float cooldown { get; private set; }
        public float radius { get; private set; }
        public ActivatorType activatorType { get; private set; }

        public ActivatorComponentData(XElement e) {
            cooldown = e.GetFloat("cooldown");
            radius = e.GetFloat("radius");
            if(e.HasAttribute("activator_type")) {
                activatorType = (ActivatorType)System.Enum.Parse(typeof(ActivatorType), e.GetString("activator_type"));
            } else {
                activatorType = ActivatorType.PirateSpawn;
            }
        }

        public ActivatorComponentData(float inCooldown, float inRadius) {
            cooldown = inCooldown;
            radius = inRadius;
        }

        public override ComponentID componentID {
            get {
                return ComponentID.Activator;
            }
        }
    }

    public class VariableActivatorComponentData : ActivatorComponentData {

        
        public string variableName { get; private set; }
        public object variableValue { get; private set; }

        public VariableActivatorComponentData(XElement e) : base(e) {
            
            variableName = e.GetString("var_name");
            variableValue = null;
            switch(activatorType) {
                case ActivatorType.BoolVarSet: {
                        variableValue = bool.Parse(e.GetString("var_value"));
                    }
                    break;
                case ActivatorType.FloatVarSet: {
                        variableValue = float.Parse(e.GetString("var_value"));
                    }
                    break;
                case ActivatorType.IntVarIncr: {
                        variableValue = int.Parse(e.GetString("var_value"));
                    }
                    break;
                case ActivatorType.IntVarSet: {
                        variableValue = int.Parse(e.GetString("var_value"));
                    }
                    break;
            }
        }

        public override ComponentSubType subType {
            get {
                return ComponentSubType.var_activator;
            }
        }

    }
}
