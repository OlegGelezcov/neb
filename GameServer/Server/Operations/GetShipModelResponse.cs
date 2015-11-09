// GetShipModelResponse.cs
// Nebula
//
// Created by Oleg Zheleztsov on Tuesday, November 3, 2015 1:48:30 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using Common;
using Photon.SocketServer.Rpc;
using System.Collections;

namespace Nebula.Server.Operations {

    /// <summary>
    /// Response for GetShipModel
    /// </summary>
    public class GetShipModelResponse {

        /// <summary>
        /// Model in form slot type - module
        /// </summary>
        [DataMember(Code =(byte)ParameterCode.Info)]
        public Hashtable model { get; set; }
    }
}
