using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Space.Game.Resources
{
    /// <summary>
    /// Realize all classes which load resources from disk
    /// </summary>
    public interface IResourceLoader
    {
        /// <summary>
        /// Load resources from disk and return true if success loading, false when fail loading
        /// </summary>
        bool Load(string basePath);

        bool Loaded { get; }
    }
}
