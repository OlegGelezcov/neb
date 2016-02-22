namespace Common {
    /// <summary>
    /// World types 
    /// </summary>
    public enum WorldType : int {
        /// <summary>
        /// Source world for race, none from other race don't enter to this world
        /// </summary>
        source = 0,
        /// <summary>
        /// Space for race with race buildings ( owners of zone may be changed)
        /// </summary>
        populated = 1,
        /// <summary>
        /// Neutral zones without buildings initially ( owners may be changed when zone capture by race)
        /// </summary>
        neutral = 2,
        /// <summary>
        /// child zones
        /// </summary>
        child = 3,

        instance = 4
    }
}
