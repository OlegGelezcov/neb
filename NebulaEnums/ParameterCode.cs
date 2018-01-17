// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterCode.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   This enumeration contains the values used for event parameter, operation request parameter and operation response parameter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


namespace Common
{
    /// <summary>
    /// This enumeration contains the values used for event parameter, operation request parameter and operation response parameter.
    /// </summary>
    public enum ParameterCode : byte
    {
        ServerList = 1,

        /// <summary>
        /// Login id of user (may be facebook id)
        /// </summary>
        LoginId = 2,

        /// <summary>
        /// Access token for session
        /// </summary>
        AccessToken = 3,

        /// <summary>
        /// User display name
        /// </summary>
        DisplayName = 4,

        /// <summary>
        /// User characters
        /// </summary>
        Characters = 5,

        Model = 6,

        MailBox = 7,

        MessageId = 8,

        AttachmentId = 9,
        Notifications = 10,
        NotificationId = 11,
        Arguments = 12,
        Level = 13,
        Exp = 14,
        SourceCharacterId = 15,
        GuildId = 16,
        Page = 17,
        Email = 18,
        FacebookId = 19,
        VkontakteId = 20,
        //Passes = 19,
        //ExpireTime = 20,
        CurrentTime = 21,
        Language = 22,
        ServerId = 23,
        Icon = 24,
        /// <summary>
        /// The event code.
        /// </summary>
        EventCode = 60,

        /// <summary>
        /// The username.
        /// </summary>
        Username = 91,

        /// <summary>
        /// The old position.
        /// </summary>
        OldPosition = 92,

        /// <summary>
        /// The position.
        /// </summary>
        Position = 93,

        /// <summary>
        /// The properties.
        /// </summary>
        Properties = 94,

        /// <summary>
        /// The item id.
        /// </summary>
        ItemId = 95,

        /// <summary>
        /// The item type.
        /// </summary>
        ItemType = 96,

        /// <summary>
        /// The properties revision.
        /// </summary>
        PropertiesRevision = 97,

        /// <summary>
        /// The custom event code.
        /// </summary>
        CustomEventCode = 98,

        /// <summary>
        /// The event data.
        /// </summary>
        EventData = 99,

        /// <summary>
        /// The top left corner.
        /// </summary>
        TopLeftCorner = 100,

        /// <summary>
        /// The tile dimensions.
        /// </summary>
        TileDimensions = 101,

        /// <summary>
        /// The bottom right corner.
        /// </summary>
        BottomRightCorner = 102,

        /// <summary>
        /// The world name.
        /// </summary>
        WorldName = 103,

        /// <summary>
        /// The view distance.
        /// </summary>
        ViewDistanceEnter = 104,

        /// <summary>
        /// The properties set.
        /// </summary>
        PropertiesSet = 105,

        /// <summary>
        /// The properties unset.
        /// </summary>
        PropertiesUnset = 106,

        /// <summary>
        /// The event reliability.
        /// </summary>
        EventReliability = 107,

        /// <summary>
        /// The event receiver.
        /// </summary>
        EventReceiver = 108,

        /// <summary>
        /// The subscribe.
        /// </summary>
        Subscribe = 109,

        /// <summary>
        /// The view distance exit.
        /// </summary>
        ViewDistanceExit = 110,

        /// <summary>
        /// The interest area id.
        /// </summary>
        InterestAreaId = 111,

        /// <summary>
        /// The counter receive interval.
        /// </summary>
        CounterReceiveInterval = 112,

        /// <summary>
        /// The counter name.
        /// </summary>
        CounterName = 113,

        /// <summary>
        /// The counter time stamps.
        /// </summary>
        CounterTimeStamps = 114,

        /// <summary>
        /// The counter values.
        /// </summary>
        CounterValues = 115,

        /// <summary>
        /// The current rotation.
        /// </summary>
        Rotation = 116,

        /// <summary>
        /// The previous rotation.
        /// </summary>
        OldRotation = 117,



        Size = 118,
        Price = 119,
        LProperties = 120,
        
        UserId = 121,

        //description of entered world
        WorldContent = 122,


        Worlds = 123,

        InventoryType = 124,
        LItemId = 125,
        LItemType = 126,
        Count = 127,
        LInnerRadius = 128,
        LOuterRadius = 129,

        SourceLogin = 130,
        TargetLogin = 131,
        Title = 132,
        Body = 133,
        Attachments = 134,

        Method = 135,
        //EnterToStation = 135,
        
        Info = 136,         //different info types, typically hashtable
        
        Group = 137,
        
        SubType = 138,

        Action = 139,

        Parameters = 140,

        LevelType = 141,

        Groups = 142,

        Skill = 143,

        Login = 144,
        Password = 145,
        Result = 146,

        WorkshopId = 147,
        Status = 148,
        GameRefId = 149,
        CharacterId = 150,
        Race = 151,
        Speed = 152,
        Type = 153,
        Components = 154,
        SubZone,
        Platform = 155,
        DeviceId = 156,

        LTEST = 250
    }
}