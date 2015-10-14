// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnterWorld.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   This operation is allowed BEFORE having entered an <see cref="MmoWorld" /> with operation <see cref="EnterWorld" /> or AFTER having exited it with operaiton <see cref="OperationCode.ExitWorld" />.
//   See <see cref="MmoPeer.OperationEnterWorld">MmoPeer.OperationEnterWorld</see> for more information.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Space.Server.Operations
{
    using System.Collections;

    using Common;
    using Photon.SocketServer;
    using Photon.SocketServer.Rpc;
    using Space.Mmo.Server;

    /// <summary>
    /// This operation is allowed BEFORE having entered an <see cref="MmoWorld"/> with operation <see cref="EnterWorld"/> or AFTER having exited it with operaiton <see cref="OperationCode.ExitWorld"/>.
    /// See <see cref="MmoPeer.OperationEnterWorld">MmoPeer.OperationEnterWorld</see> for more information.
    /// </summary>
    public class EnterWorld : Operation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnterWorld"/> class.
        /// </summary>
        /// <param name="protocol">
        /// The protocol.
        /// </param>
        /// <param name="request">
        /// The request. 
        /// </param>
        public EnterWorld(IRpcProtocol protocol, OperationRequest request)
            : base(protocol, request)
        {
        }

        /// <summary>
        /// Gets or sets the id of the initial <see cref="InterestArea"/>.
        /// This request parameter is optional. Default: #0.
        /// </summary>
        [DataMember(Code = (byte)ParameterCode.InterestAreaId, IsOptional = true)]
        public byte InterestAreaId { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Actor.Avatar"/>'s initial position.
        /// This request parameter is mandatory.
        /// </summary>
        [DataMember(Code = (byte)ParameterCode.Position)]
        public float[] Position { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Actor.Avatar"/>'s initial properties.
        /// This request parameter is optional.
        /// </summary>
        [DataMember(Code = (byte)ParameterCode.Properties, IsOptional = true)]
        public Hashtable Properties { get; set; }

        /// <summary>
        /// Gets or sets the new rotation.
        /// This request parameter is optional.
        /// </summary>
        [DataMember(Code = (byte)ParameterCode.Rotation, IsOptional = true)]
        public float[] Rotation { get; set; }

        /// <summary>
        /// Gets or sets the client's username. This will be the <see cref="Actor.Avatar"/>'s <see cref="Item.Id">Item Id</see>.
        /// This request parameter is mandatory.
        /// </summary>
        //[DataMember(Code = (byte)ParameterCode.Username)]
        //public string Username { get; set; }

        /// <summary>
        /// Gets or sets the inner view distance (item subscribe threshold) of the initial <see cref="InterestArea"/>.
        /// This request parameter is mandatory.
        /// </summary>
        [DataMember(Code = (byte)ParameterCode.ViewDistanceEnter)]
        public float[] ViewDistanceEnter { get; set; }

        /// <summary>
        /// Gets or sets the outer view distance (item unsubscribe threshold) of the initial <see cref="InterestArea"/>.
        /// This request parameter is mandatory.
        /// </summary>
        [DataMember(Code = (byte)ParameterCode.ViewDistanceExit)]
        public float[] ViewDistanceExit { get; set; }

        /// <summary>
        /// Gets or sets the name of the selected <see cref="MmoWorld"/>.
        /// </summary>
        [DataMember(Code = (byte)ParameterCode.WorldName)]
        public string WorldName { get; set; }

        [DataMember(Code = (byte)ParameterCode.LevelType, IsOptional=true)]
        public byte LevelType { get; set; }

        [DataMember(Code = (byte)ParameterCode.GameRefId)]
        public string GameRefId { get; set; }

        [DataMember(Code =(byte)ParameterCode.CharacterId, IsOptional =false)]
        public string CharacterId { get; set; }

        [DataMember(Code =(byte)ParameterCode.WorkshopId, IsOptional =false)]
        public byte Workshop { get; set; }

        [DataMember(Code =(byte)ParameterCode.Race, IsOptional =false)]
        public byte Race { get; set; }

        [DataMember(Code =(byte)ParameterCode.Model, IsOptional =false)]
        public Hashtable Model { get; set; }

        [DataMember(Code =(byte)ParameterCode.DisplayName, IsOptional =false)]
        public string Name { get; set; }

        [DataMember(Code = (byte)ParameterCode.Login, IsOptional = false)]
        public string Login { get; set; }

        //[DataMember(Code =(byte)ParameterCode.EnterToStation, IsOptional = true)]
        //public bool EnterAtStation { get; set; }

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
        public OperationResponse GetOperationResponse(short errorCode, string debugMessage, Hashtable content )
        {
            var responseObject = new EnterWorldResponse { WorldName = this.WorldName, LevelType = this.LevelType, Content = content  };
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
        public OperationResponse GetOperationResponse(MethodReturnValue returnValue, Hashtable content )
        {
            return this.GetOperationResponse(returnValue.Error, returnValue.Debug, content );
        }
    }
}