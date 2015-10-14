
namespace Space.Game.Resources.Zones
{
    /// <summary>
    /// Info about where in zone place asteroid
    /// </summary>
    public class ZoneAsteroidInfo
    {
        /// <summary>
        /// Position of asteroid
        /// </summary>
        public float[] Position { get; set; }

        /// <summary>
        /// Rotation of asteroid
        /// </summary>
        public float[] Rotation { get; set; }

        /// <summary>
        /// Respawn time
        /// </summary>
        public float Respawn { get; set; }

        /// <summary>
        /// Index of asteroid in world(analogue of id)
        /// </summary>
        public int Index { get; set; }

        public string DataId { get; set; }

        public bool ForceCreate { get; set; }

        public string model { get; set; }
    }
}

