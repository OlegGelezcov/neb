// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateWorld.cs" company="Exit Games GmbH">
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
    using Photon.SocketServer;
    using Photon.SocketServer.Rpc;
    using Space.Mmo.Server;
    using System.Collections;

    /// <summary>
    /// This operation is allowed BEFORE having entered an <see cref="MmoWorld"/> with operation <see cref="EnterWorld"/>.
    /// See <see cref="MmoPeer.OperationCreateWorld">MmoPeer.OperationCreateWorld</see> for more information.
    /// </summary>
    public class CreateWorld : Operation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateWorld"/> class.
        /// </summary>
        /// <param name="protocol">
        /// The protocol.
        /// </param>
        /// <param name="request">
        /// The request. 
        /// </param>
        public CreateWorld(IRpcProtocol protocol, OperationRequest request)
            : base(protocol, request)
        {
        }


        /// <summary>
        /// Gets or sets the <see cref="MmoWorld"/>'s name.
        /// </summary>
        [DataMember(Code = (byte)ParameterCode.WorldName)]
        public string WorldName { get; set; }

        /// <summary>
        /// Gets the operation response.
        /// </summary>
        /// <param name="errorCode">
        /// The error code.
        /// </param>
        /// <param name="debugMessage">
        /// The debug message.
        /// </param>
        /// <returns>
        /// A new operation response.
        /// </returns>
        public OperationResponse GetOperationResponse(short errorCode, string debugMessage)
        {
            var responseObject = new CreateWorldResponse { WorldName = this.WorldName };
            return new OperationResponse(this.OperationRequest.OperationCode, responseObject) { ReturnCode = errorCode, DebugMessage = debugMessage };
        }

        /// <summary>
        /// Gets the operation response.
        /// </summary>
        /// <param name="returnValue">
        /// The return value.
        /// </param>
        /// <returns>
        /// A new operation response.
        /// </returns>
        public OperationResponse GetOperationResponse(MethodReturnValue returnValue)
        {
            return this.GetOperationResponse(returnValue.Error, returnValue.Debug);
        }
    }
}