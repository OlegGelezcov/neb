using Common;
using System;
using ExitGames.Client.Photon;
using ServerClientCommon;
#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif

namespace Nebula.Server.Components {
    public class ModelComponentData : MultiComponentData, IDatabaseComponentData {

        public string model { get; private set; }
#if UP
        public ModelComponentData(UPXElement componentElement) {
            if (componentElement.HasAttribute("model")) {
                model = componentElement.GetString("model");
            } else {
                model = string.Empty;
            }
        }
#else
        public ModelComponentData(XElement componentElement) {
            if (componentElement.HasAttribute("model")) {
                model = componentElement.GetString("model");
            } else {
                model = string.Empty;
            }
        }
#endif

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
