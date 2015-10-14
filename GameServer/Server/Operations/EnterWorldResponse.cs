// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnterWorldResponse.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   This operation is allowed BEFORE having entered an <see cref="MmoWorld" /> with operation <see cref="EnterWorld" /> or AFTER having exited it with operaiton <see cref="OperationCode.ExitWorld" />.
//   See <see cref="MmoPeer.OperationEnterWorld">MmoPeer.OperationEnterWorld</see> for more information.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Space.Server.Operations
{
    using Common;
    using Photon.SocketServer.Rpc;
    using System.Collections;

    /// <summary>
    /// This operation is allowed BEFORE having entered an <see cref="MmoWorld"/> with operation <see cref="EnterWorld"/> or AFTER having exited it with operaiton <see cref="OperationCode.ExitWorld"/>.
    /// See <see cref="MmoPeer.OperationEnterWorld">MmoPeer.OperationEnterWorld</see> for more information.
    /// </summary>
    public class EnterWorldResponse
    {
        /// <summary>
        /// Gets or sets the <see cref="GridWorld"/>'s bottom right corner.
        /// This response parameter is submitted with error code <see cref="ReturnCode.Ok"/>.
        /// </summary>
        [DataMember(Code = (byte)ParameterCode.BottomRightCorner, IsOptional = true)]
        public float[] BottomRightCorner { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="GridWorld"/>'s tile dimensions.
        /// This response parameter is submitted with error code <see cref="ReturnCode.Ok"/>.
        /// </summary>
        [DataMember(Code = (byte)ParameterCode.TileDimensions, IsOptional = true)]
        public float[] TileDimensions { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="GridWorld"/>'s top left corner.
        /// This response parameter is submitted with error code <see cref="ReturnCode.Ok"/>.
        /// </summary>
        [DataMember(Code = (byte)ParameterCode.TopLeftCorner, IsOptional = true)]
        public float[] TopLeftCorner { get; set; }

        /// <summary>
        /// Gets or sets the name of the selected <see cref="MmoWorld"/>.
        /// </summary>
        [DataMember(Code = (byte)ParameterCode.WorldName)]
        public string WorldName { get; set; }

        [DataMember(Code = (byte)ParameterCode.LevelType, IsOptional = true)]
        public byte LevelType { get; set; }

        [DataMember(Code = (byte)ParameterCode.WorldContent, IsOptional = true)]
        public Hashtable Content { get; set; }

        [DataMember(Code = (byte)ParameterCode.ItemId, IsOptional = true )]
        public string ItemId { get; set; }

        [DataMember(Code =(byte)ParameterCode.Components, IsOptional =true)]
        public object[] components { get; set; }

        [DataMember(Code =(byte)ParameterCode.Size, IsOptional =true)]
        public float size { get; set; }

        [DataMember(Code =(byte)ParameterCode.SubZone, IsOptional =true)]
        public int subZone { get; set; }

        //[DataMember(Code =(byte)ParameterCode.EnterToStation, IsOptional =true)]
        //public bool startAtStation { get; set; }
    }
}