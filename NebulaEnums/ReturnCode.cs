// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReturnCode.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   This enumeration contains all known error codes.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Common
{

    /// <summary>
    /// This enumeration contains all known error codes.
    /// </summary>
    public enum ReturnCode : short
    {
        /// <summary>
        /// The ok code.
        /// </summary>
        Ok = 0,

        /// <summary>
        /// The fatal.
        /// </summary>
        Fatal = 1,

        /// <summary>
        /// The parameter out of range.
        /// </summary>
        ParameterOutOfRange = 51,

        /// <summary>
        /// The operation not supported.
        /// </summary>
        OperationNotSupported,

        /// <summary>
        /// The invalid operation parameter.
        /// </summary>
        InvalidOperationParameter,

        /// <summary>
        /// The invalid operation.
        /// </summary>
        InvalidOperation,

        /// <summary>
        /// The avatar access denied.
        /// </summary>
        ItemAccessDenied,

        /// <summary>
        /// interest area not found.
        /// </summary>
        InterestAreaNotFound,

        /// <summary>
        /// The interest area already exists.
        /// </summary>
        InterestAreaAlreadyExists,

        /// <summary>
        /// The world already exists.
        /// </summary>
        WorldAlreadyExists = 101,

        /// <summary>
        /// The world not found.
        /// </summary>
        WorldNotFound,

        /// <summary>
        /// The item already exists.
        /// </summary>
        ItemAlreadyExists,

        /// <summary>
        /// The item not found.
        /// </summary>
        ItemNotFound,

        LOk = 111,
        LFatal = 112,
        LInvalidOperationParameter = 113,
        LUnsupportedOperation = 114,
        LRequestServerPropertiesInvalidParameters = 115,
        LItemNotFound = 116,
        LWorldNotFound = 117,
        LPlayerIdDontMatch = 118,

        SourcePlayerNotFound,
        TargetPlayerNotFound,
        LogicError,
        DontEnoughItemsInInventory,
        InventoryItemNotFound,
        StationHoldItemNotFound,
        PlayerStoreNotFounded,
        NotEnoughInventorySpace,
        ErrorAddingToInventory,
        NameAlreadyExists,
        ErrorOfAddingMail,
        UserNotFound,
        ResourceNotFound,
        NotEnoughCredits,
        InvalidInapType, 

        OperationDenied = -3,
        OperationInvalid = -2,
        InternalServerError = -1,


        InvalidAuthentication = 0x7FFF, // codes start at short.MaxValue 
        GameIdAlreadyExists = 0x7FFF - 1,
        GameFull = 0x7FFF - 2,
        GameClosed = 0x7FFF - 3,
        AlreadyMatched = 0x7FFF - 4,
        ServerFull = 0x7FFF - 5,
        UserBlocked = 0x7FFF - 6,
        NoMatchFound = 0x7FFF - 7,
        RedirectRepeat = 0x7FFF - 8,
        GameIdNotExists = 0x7FFF - 9,

        // for authenticate requests. Indicates that the max ccu limit has been reached
        MaxCcuReached = 0x7FFF - 10,

        // for authenticate requests. Indicates that the application is not subscribed to this region / private cloud. 
        InvalidRegion = 0x7FFF - 11,

        // for authenticate requests. Indicates that the call to the external authentication service failed.
        CustomAuthenticationFailed = short.MaxValue - 12,
        AccessTokenInvalid = short.MaxValue - 13,
        ErrorOfRetrievingUserLoginFromDatabase = short.MaxValue - 14
    }
}