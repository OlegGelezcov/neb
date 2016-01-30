﻿using ExitGames.Logging;
using Nebula.Game.Contracts;
using ServerClientCommon;
using Space.Game;
using System.Collections;

namespace Nebula.Game {
    public class ContractOperations : BaseRPCOperations {
        private static readonly ILogger s_Log = LogManager.GetCurrentClassLogger();

        private MmoActor player { get; set; }

        public ContractOperations(MmoActor player) {
            this.player = player;
        }

        public Hashtable AcceptTestContract() {
            ContractManager contractManager = player.GetComponent<ContractManager>();
            KillNPCContract contract = new KillNPCContract("ct1", Common.ContractState.accepted, 0, player.nebulaObject.mmoWorld().Name);
            bool success = contractManager.AcceptContract(contract);
            Hashtable hash = CreateResponse(Common.RPCErrorCode.Ok);
            hash.Add(SPCKEY(SPC.Status), success);
            return hash;
        }
    }
}
