// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ItemGeneric.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Clients receive this event after executing operation <see cref="RaiseGenericEvent" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Space.Server.Events
{
    using Common;
    using Photon.SocketServer.Rpc;

    /// <summary>
    /// Clients receive this event after executing operation <see cref="RaiseGenericEvent"/>.
    /// </summary>
    public class ItemGeneric
    {
        /// <summary>
        /// Gets or sets the custom event code.
        /// </summary>
        [DataMember(Code = (byte)ParameterCode.CustomEventCode)]
        public byte CustomEventCode { get; set; }

        /// <summary>
        /// Gets or sets the contained data.
        /// </summary>
        [DataMember(Code = (byte)ParameterCode.EventData, IsOptional = true)]
        public object EventData { get; set; }

        /// <summary>
        /// Gets or sets the source <see cref="Item"/> Id.
        /// </summary>
        [DataMember(Code = (byte)ParameterCode.ItemId)]
        public string ItemId { get; set; }

        /// <summary>
        /// Gets or sets the source <see cref="Item"/> type.
        /// </summary>
        [DataMember(Code = (byte)ParameterCode.ItemType)]
        public byte ItemType { get; set; }

        [DataMember(Code = (byte)ParameterCode.GameRefId, IsOptional = true)]
        public string GameReferenceId { get; set; }

        [DataMember(Code = (byte)ParameterCode.CharacterId, IsOptional = true)]
        public string CharacterId { get; set; }
    }
}