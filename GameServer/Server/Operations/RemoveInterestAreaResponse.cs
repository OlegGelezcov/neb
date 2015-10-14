// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoveInterestArea.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   This operation is allowed AFTER having entered an <see cref="MmoWorld" /> with operation <see cref="EnterWorld" />.
//   See <see cref="MmoActor.OperationRemoveInterestArea">MmoActor.OperationRemoveInterestArea</see> for more information.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Space.Server.Operations
{
    using Common;
    using Photon.SocketServer.Rpc;

    /// <summary>
    /// This operation is allowed AFTER having entered an <see cref="MmoWorld"/> with operation <see cref="EnterWorld"/>.
    /// See <see cref="MmoActor.OperationRemoveInterestArea">MmoActor.OperationRemoveInterestArea</see> for more information.
    /// </summary>
    public class RemoveInterestAreaResponse
    {
        /// <summary>
        /// Gets or sets the id of the <see cref="InterestArea"/> to remove.
        /// </summary>
        [DataMember(Code = (byte)ParameterCode.InterestAreaId)]
        public byte InterestAreaId { get; set; }
    }
}