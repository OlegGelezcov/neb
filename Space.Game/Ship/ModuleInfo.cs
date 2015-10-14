
namespace Space.Game.Ship
{
    using System;
    using Common;
    using System.Collections.Generic;

    /// <summary>
    /// Module info from resources file
    /// </summary>
    public class ModuleInfo
    {
        /// <summary>
        /// Id of module template
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Module type
        /// </summary>
        public ShipModelSlotType Type { get; set; }

        /// <summary>
        /// Name of graphic prefab of module
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// Set which belong module(if null or empty no set)
        /// </summary>
        public List<string> allowedSets { get; set; }

        public string Name { get; set; }

        public bool hasSets
        {
            get
            {
                return allowedSets.Count > 0;
            }
        }


        public Workshop Workshop { get; set; }

    }
}
