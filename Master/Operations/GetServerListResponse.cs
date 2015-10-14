// GetServerListResponse.cs
// Nebula
//
// Created by Oleg Zheleztsov on Sunday, September 13, 2015 5:54:06 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
namespace Master.Operations {
    using Common;
    using Photon.SocketServer.Rpc;
    using System.Collections;
    public class GetServerListResponse {

        [DataMember(Code =(byte)ParameterCode.ServerList, IsOptional =false)]
        public Hashtable ServerList { get; set; }
    }
}
