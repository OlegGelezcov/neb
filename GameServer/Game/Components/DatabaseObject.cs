using Common;
using ExitGames.Logging;
using Nebula.Engine;
using Nebula.Server.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Game.Components {
    /// <summary>
    /// Object will be saved death state into database when die
    /// </summary>
    public class DatabaseObject : NebulaBehaviour {

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public override int behaviourId {
            get {
                return (int)ComponentID.DatabaseObject;
            }
        }

        public string databaseID { get; private set; }

        public void Init(DatabaseObjectComponentData data) {
            databaseID = data.databaseID;
        }

        public void Death() {
            log.InfoFormat("Death() call on database object. Save it world state [dy]");
            nebulaObject.mmoWorld().AddDestroyedObjectToSave(databaseID);
        }
    }
}
