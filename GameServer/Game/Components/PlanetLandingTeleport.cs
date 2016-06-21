using Common;
using ExitGames.Logging;
using Nebula.Engine;
using Nebula.Server.Nebula.Server.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Game.Components {
    public class PlanetLandingTeleport : NebulaBehaviour {

        private static readonly ILogger s_Log = LogManager.GetCurrentClassLogger();

        private PlanetLandingTeleportData m_Data;

        public void Init(PlanetLandingTeleportData data) {
            m_Data = data;
            if(data != null && props != null ) {
                props.SetProperty((byte)PS.EventWorldId, data.planetId);
            }
        }

        public override void Start() {
            if(m_Data != null && props != null ) {
                props.SetProperty((byte)PS.EventWorldId, m_Data.planetId);
            }
            s_Log.InfoFormat("Create planet landing object!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        }

        public override int behaviourId {
            get {
                return (int)ComponentID.PlanetLandingTeleport;
            }
        }
    }
}
