using Common;
using Nebula.Engine;
using Nebula.Server.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Game.Components {
    public class SubZoneComponent : NebulaBehaviour {

        public float innerRadius { get; private set; }
        public float outerRadius { get; private set; }
        public int subZoneID { get; private set; }

        private void UpdateProperties() {
            if (nebulaObject.properties) {
                nebulaObject.properties.SetProperty((byte)PS.SubZoneID, subZoneID);
                nebulaObject.properties.SetProperty((byte)PS.InnerRadius, innerRadius);
                nebulaObject.properties.SetProperty((byte)PS.OuterRadius, outerRadius);
            }
        }

        public void Init(SubZoneComponentData data) {
            innerRadius = data.innerRadius;
            outerRadius = data.outerRadius;
            subZoneID = data.subZoneID;
        }

        public override void Start() {
            UpdateProperties();
        }

        public override int behaviourId {
            get {
                return (int)ComponentID.SubZone;
            }
        }
    }
}
