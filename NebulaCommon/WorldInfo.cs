using System;
using System.Collections;
using Common;
using ServerClientCommon;

namespace NebulaCommon {

    /// <summary>
    /// World info database format common among server projects
    /// </summary>
    public class WorldInfo  : IInfoSource {
        

        /// <summary>
        /// ID of world
        /// </summary>
        public string worldID { get; set; }
        /// <summary>
        /// Start race of world
        /// </summary>
        public int startRace { get; set; }
        /// <summary>
        /// Current race of world
        /// </summary>
        public int currentRace { get; set; }
        /// <summary>
        /// Type of world
        /// </summary>
        public int worldType { get; set; }
        /// <summary>
        /// Under attack or not world currently
        /// </summary>
        public bool underAttack { get; set; }

        public int attackRace { get; set; }

        public int playerCount { get; set; }

        public Hashtable GetInfo() {
            return new Hashtable {
                {(int)SPC.WorldId, worldID },
                {(int)SPC.StartRace, startRace },
                {(int)SPC.CurrentRace, currentRace },
                {(int)SPC.WorldType, worldType },
                {(int)SPC.UnderAttack, underAttack },
                {(int)SPC.AttackRace, attackRace },
                {(int)SPC.PlayerCount, playerCount }
            };
        }
    }
}
