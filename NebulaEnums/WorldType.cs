namespace Common {
    /// <summary>
    /// World types 
    /// </summary>
    public enum WorldType : int {
        /// <summary>
        /// Source world for race, none from other race don't enter to this world
        /// </summary>
        source,
        /// <summary>
        /// Space for race with race buildings ( owners of zone may be changed)
        /// </summary>
        populated,
        /// <summary>
        /// Neutral zones without buildings initially ( owners may be changed when zone capture by race)
        /// </summary>
        neutral,
        /// <summary>
        /// child zones
        /// </summary>
        child
    }
}
