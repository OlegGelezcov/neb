// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventCode.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   This enumeration contains all known event codes.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


namespace Common
{
    /// <summary>
    /// This enumeration contains all known event codes.
    /// </summary>
    public enum EventCode : byte
    {
        /// <summary>
        /// The actor leave.
        /// </summary>
        ItemDestroyed = 1,

        /// <summary>
        /// The actor move.
        /// </summary>
        ItemMoved = 2,

        /// <summary>
        /// The item properties set.
        /// </summary>
        ItemPropertiesSet = 3,

        /// <summary>
        /// The item generic.
        /// </summary>
        ItemGeneric = 4,

        /// <summary>
        /// The world exited.
        /// </summary>
        WorldExited = 5,

        /// <summary>
        /// The item subscribed.
        /// </summary>
        ItemSubscribed = 6,

        /// <summary>
        /// The item unsubscribed.
        /// </summary>
        ItemUnsubscribed = 7,

        /// <summary>
        /// The item properties.
        /// </summary>
        ItemProperties = 8,

        /// <summary>
        /// The radar update.
        /// </summary>
        RadarUpdate = 9,

        /// <summary>
        /// The counter data.
        /// </summary>
        CounterData = 10,

        LRemoveItemFromInterestArea = 11,
        LAddItemToInterestArea = 12,
        LItemMoved = 13,
        LItemRemoved = 14,
        LOwnerRemoved = 15,
        LFire = 16,
        LItemPropertiesChanged = 17,
        LItemNamedPropertyChanged = 18,
        UseSkill = 19,
        WorkshopEntered = 20,
        WorkshopExited = 21,
        ItemPropertyUpdate = 22,
        LTEST = 250
    }
}