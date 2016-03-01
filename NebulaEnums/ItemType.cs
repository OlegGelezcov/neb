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
        Avatar = 0,

        /// <summary>
        /// The bot (fake avatar).
        /// </summary>
        Bot = 1,

        /// <summary>
        /// Some chest in world
        /// </summary>
        Chest = 2,

        /// <summary>
        /// Asteroid item
        /// </summary>
        Asteroid = 3,

        /// <summary>
        /// World event object
        /// </summary>
        Event = 4,

        SharedChest = 5,

        Teleport = 6,

        Station = 7,

        None = 8
    }
}