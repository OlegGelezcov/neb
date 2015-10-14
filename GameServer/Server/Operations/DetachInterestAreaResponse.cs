// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DetachInterestAreaResponse.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   This operation is allowed AFTER having entered an <see cref="MmoWorld" /> with operation <see cref="EnterWorld" />.
//   See <see cref="MmoActor.OperationDetachInterestArea">MmoActor.OperationDetachInterestArea</see> for more information.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Space.Server.Operations
{
    using Common;
    using Photon.SocketServer.Rpc;

    /// <summary>
    /// This operation is allowed AFTER having entered an <see cref="MmoWorld"/> with operation <see cref="EnterWorld"/>.
    /// See <see cref="MmoActor.OperationDetachInterestArea">MmoActor.OperationDetachInterestArea</see> for more information.
    /// </summary>
    public class DetachInterestAreaResponse
    {
        /// <summary>
        /// Gets or sets the id of the <see cref="InterestArea"/> to be detached.
        /// Interest area #0 is seleted if none is submitted.
        /// </summary>
        [DataMember(Code = (byte)ParameterCode.InterestAreaId)]
        public byte InterestAreaId { get; set; }
    }
}