using Common;
using System.Xml.Linq;
using System;
using System.Collections;
using ServerClientCommon;

namespace Nebula.Server.Components {
    public class ModelComponentData : MultiComponentData, IDatabaseComponentData {

        public string model { get; private set; }

        public ModelComponentData(XElement componentElement) {
            if (componentElement.HasAttribute("model")) {
                model = componentElement.GetString("model");
            } else {
                model = string.Empty;
            }
        }

        public ModelComponentData(string model) {
            this.model = model;
        }

        public ModelComponentData(Hashtable hash) {
            model = hash.GetValue<string>((int)SPC.Model, string.Empty);
        }

        public override ComponentID componentID {
            get {
                return ComponentID.Model;
            }
        }

        public override ComponentSubType subType {
            get {
                return ComponentSubType.simple_model;
            }
        }

        public Hashtable AsHash() {
            return new Hashtable {
                { (int)SPC.Model, model }
            };
        }

    }
}
