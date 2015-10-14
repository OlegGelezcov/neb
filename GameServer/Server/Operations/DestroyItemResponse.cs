// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DestroyItemResponse.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   This operation is allowed AFTER having entered an <see cref="MmoWorld" /> with operation <see cref="EnterWorld" />.
//   See <see cref="MmoActor.OperationDestroyItem">MmoActor.OperationDestroyItem</see> for more information.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Space.Server.Operations
{
    using Common;
    using Photon.SocketServer;
    using Photon.SocketServer.Rpc;

    /// <summary>
    /// This operation is allowed AFTER having entered an <see cref="MmoWorld"/> with operation <see cref="EnterWorld"/>.
    /// See <see cref="MmoActor.OperationDestroyItem">MmoActor.OperationDestroyItem</see> for more information.
    /// </summary>
    public class DestroyItemResponse
    {
        /// <summary>
        /// Gets or sets the id of the <see cref="Item"/> to be destroyed.
        /// </summary>
        [DataMember(Code = (byte)ParameterCode.ItemId)]
        public string ItemId { get; set; }

        /// <summary>
        /// Gets or sets the type of the <see cref="Item"/> to be destroyed.
        /// </summary>
        [DataMember(Code = (byte)ParameterCode.ItemType)]
        public byte ItemType { get; set; }
    }
}