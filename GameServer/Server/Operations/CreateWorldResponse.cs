﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateWorldResponse.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   This operation is allowed BEFORE having entered an <see cref="MmoWorld" /> with operation <see cref="EnterWorld" />.
//   See <see cref="MmoPeer.OperationCreateWorld">MmoPeer.OperationCreateWorld</see> for more information.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Space.Server.Operations
{
    using Common;
    using Photon.SocketServer.Rpc;
using System.Collections;

    /// <summary>
    /// This operation is allowed BEFORE having entered an <see cref="MmoWorld"/> with operation <see cref="EnterWorld"/>.
    /// See <see cref="MmoPeer.OperationCreateWorld">MmoPeer.OperationCreateWorld</see> for more information.
    /// </summary>
    public class CreateWorldResponse
    {
        /// <summary>
        /// Gets or sets the <see cref="MmoWorld"/>'s name.
        /// </summary>
        [DataMember(Code = (byte)ParameterCode.WorldName)]
        public string WorldName { get; set; }
    }
}