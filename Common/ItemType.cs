// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ItemType.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   This enum contains all know item types.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


namespace Common
{
    /// <summary>
    /// This enum contains all know item types.
    /// </summary>
    public enum ItemType
    {
        /// <summary>
        /// The avatar.
        /// </summary>
        Avatar,

        /// <summary>
        /// The bot (fake avatar).
        /// </summary>
        Bot,

        /// <summary>
        /// Some chest in world
        /// </summary>
        Chest,

        /// <summary>
        /// Asteroid item
        /// </summary>
        Asteroid,

        /// <summary>
        /// World event object
        /// </summary>
        Event,

        SharedChest,

        Teleport,

        Station
    }
}