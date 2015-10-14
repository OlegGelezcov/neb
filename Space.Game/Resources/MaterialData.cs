using Common;

namespace Space.Game.Resources
{
    /// <summary>
    /// Base class for materials
    /// </summary>
    public abstract class MaterialData
    {
        /// <summary>
        /// Type of material
        /// </summary>
        public abstract MaterialType Type { get; }

        /// <summary>
        /// Id of material
        /// </summary>
        public abstract string Id { get; set; }

        /// <summary>
        /// Name of material
        /// </summary>
        public abstract string Name { get; set; }

    }
}
