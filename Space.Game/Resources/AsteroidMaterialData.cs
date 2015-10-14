using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;


namespace Space.Game.Resources
{
    /// <summary>
    /// Describe material element, which contains in asteroid
    /// </summary>
    public class AsteroidMaterialData
    {
        public MaterialType Type { get; set; }
        public string Id { get; set; }
        public int MaxCount { get; set; }
        public float Prob { get; set; }
    }
}
