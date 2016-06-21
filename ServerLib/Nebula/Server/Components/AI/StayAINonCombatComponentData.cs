using Nebula.Server.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Collections;
using ServerClientCommon;

namespace Nebula.Server.Nebula.Server.Components.AI {
    public class StayAINonCombatComponentData : BaseAIComponentData, IDatabaseComponentData {

        public StayAINonCombatComponentData(XElement e) :
            base(e) {
        }

        public StayAINonCombatComponentData(bool inAlignWithForwardDirection, float rotationSpeed)
            : base(inAlignWithForwardDirection, rotationSpeed) {
        }

        public StayAINonCombatComponentData(Hashtable hash)
            : base(hash) {
        }

        public override ComponentSubType subType {
            get {
                return ComponentSubType.ai_stay_non_combat;
            }
        }

        public Hashtable AsHash() {
            return new Hashtable {
                { (int)SPC.AlignWithForwardDirection, alignWithForwardDirection },
                { (int)SPC.RotationSpeed, rotationSpeed },
                { (int)SPC.SubType, (int)subType }
            };
        }
    }
}
