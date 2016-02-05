using Common;
using ExitGames.Logging;
using Nebula.Game.Components;
using Nebula.Game.Contracts;
using Nebula.Game.Contracts.Generators;
using ServerClientCommon;
using Space.Game;
using System.Collections;
using System.Linq;

namespace Nebula.Game {
    public class ContractOperations : BaseRPCOperations {
        private static readonly ILogger s_Log = LogManager.GetCurrentClassLogger();

        private MmoActor player { get; set; }

        public ContractOperations(MmoActor player) {
            this.player = player;
        }

        //public Hashtable AcceptTestContract() {
        //    ContractManager contractManager = player.GetComponent<ContractManager>();
        //    KillNPCContract contract = new KillNPCContract("ct1", Common.ContractState.accepted, 0, player.nebulaObject.mmoWorld().Name, contractManager);
        //    bool success = contractManager.AcceptContract(contract);
        //    Hashtable hash = CreateResponse(Common.RPCErrorCode.Ok);
        //    hash.Add(SPCKEY(SPC.Status), success);
        //    return hash;
        //}

        //public Hashtable AcceptContract(int icategory) {
        //    ContractManager contractManager = player.GetComponent<ContractManager>();
        //    ContractCategory category = (ContractCategory)icategory;
        //    if(contractManager.HasActiveContract(category)) {
        //        Hashtable resp = CreateResponse(RPCErrorCode.AlreadyHasAcceptedContractWithSuchCategory);
        //        resp.Add((int)SPC.ContractCategory, icategory);
        //        return resp;
        //    }

        //    var generator = ContractGenerator.Create(category);
        //    if(generator == null ) {
        //        Hashtable resp = CreateResponse(RPCErrorCode.ContractGeneratorNotFound);
        //        resp.Add((int)SPC.ContractCategory, icategory);
        //        return resp;
        //    }

        //    var playerRaceable = player.GetComponent<RaceableObject>();
        //    var playerCharacter = player.GetComponent<CharacterObject>();

        //    var contract = generator.Generate((Race)playerRaceable.race, playerCharacter.level, (player.World as MmoWorld).Name, contractManager, player.resource);
        //    if(contract == null ) {
        //        Hashtable resp = CreateResponse(RPCErrorCode.NoValidContract);
        //        return resp;
        //    }

        //    bool accepted = contractManager.AcceptContract(contract);
        //    Hashtable hash = CreateResponse(RPCErrorCode.Ok);
        //    hash.Add((int)SPC.Status, accepted);
        //    hash.Add((int)SPC.Contract, contract.GetInfo());
        //    return hash;
        //}

        public Hashtable GetContracts() {
            ContractManager contractManager = player.GetComponent<ContractManager>();
            return contractManager.GetInfo();
        }

        //public Hashtable CompleteContract(string id) {
        //    ContractManager contractManager = player.GetComponent<ContractManager>();
        //    bool success = contractManager.CompleteContract(id);
        //    Hashtable hash = CreateResponse(Common.RPCErrorCode.Ok);
        //    hash.Add(SPCKEY(SPC.Status), success);
        //    return hash;
        //}

        public Hashtable ProposeContract(int icategory) {
            ContractCategory category = (ContractCategory)icategory;
            ContractManager contractManager = player.GetComponent<ContractManager>();
            if(contractManager.HasActiveContract(category)) {
                Hashtable resp = CreateResponse(RPCErrorCode.AlreadyHasAcceptedContractWithSuchCategory);
                resp.Add((int)SPC.ContractCategory, icategory);
                return resp;
            }
            var generator = ContractGenerator.Create(category);
            if (generator == null) {
                Hashtable resp = CreateResponse(RPCErrorCode.ContractGeneratorNotFound);
                resp.Add((int)SPC.ContractCategory, icategory);
                return resp;
            }

            var playerRaceable = player.GetComponent<RaceableObject>();
            var playerCharacter = player.GetComponent<CharacterObject>();

            var contract = generator.Generate((Race)playerRaceable.race, playerCharacter.level, (player.World as MmoWorld).Name, contractManager, player.resource);
            if (contract == null) {
                Hashtable resp = CreateResponse(RPCErrorCode.NoValidContract);
                return resp;
            }

            if( false == contractManager.ProposeContract(contract) ) {
                return CreateResponse(RPCErrorCode.UnableProposeContract);
            }

            Hashtable result = CreateResponse(RPCErrorCode.Ok);
            result.Add((int)SPC.Contract, contract.GetInfo());

            return result;
        }


        /// <summary>
        /// Try accept proposed contract
        /// </summary>
        /// <param name="contractId">Contract id to will be accepted</param>
        /// <returns>If success return contract info or error code</returns>
        public Hashtable AcceptContract(string contractId) {
            ContractManager contractManager = player.GetComponent<ContractManager>();
            if(false == contractManager.HasProposedContract(contractId)) {
                Hashtable ret = CreateResponse(RPCErrorCode.ProposedContractNotFound);
            }
            BaseContract contract;
            if(contractManager.AcceptProposedContract(contractId, out contract)) {
                Hashtable ret = CreateResponse(RPCErrorCode.Ok);
                ret.Add((int)SPC.Contract, contract.GetInfo());
                return ret;
            }
            return CreateResponse(RPCErrorCode.ErrorOfAcceptContract);
        }

        public Hashtable DeclineContract(string contractId ) {
            ContractManager contractManager = player.GetComponent<ContractManager>();
            if((!contractManager.HasProposedContract(contractId)) && (!contractManager.HasActiveContract(contractId) )) {
                return CreateResponse(RPCErrorCode.NoValidContract);
            }

            BaseContract declinedContract;
            if(contractManager.DeclineContract(contractId, out declinedContract)) {
                Hashtable ret = CreateResponse(RPCErrorCode.Ok);
                ret.Add((int)SPC.Contract, declinedContract.GetInfo());
                return ret;
            }
            return CreateResponse(RPCErrorCode.NoValidContract);
        }

        public Hashtable CompleteContract(string contractId ) {
            ContractManager contractManager = player.GetComponent<ContractManager>();
            BaseContract completedContract;
            if(contractManager.CompleteContract(contractId, out completedContract)) {
                ContractRewardExecutor rewardGiver = new ContractRewardExecutor();
                var items = rewardGiver.GiveRewards(contractId, player);
                object[] itArr = new object[items.Count];
                for(int i = 0; i < items.Count; i++ ) {
                    itArr[i] = items[i].GetInfo();
                }

                Hashtable hash = CreateResponse(RPCErrorCode.Ok);
                hash.Add((int)SPC.Contract, completedContract.GetInfo());
                hash.Add((int)SPC.Items, itArr);
                return hash;
            }
            return CreateResponse(RPCErrorCode.UnknownError);
        }


    }
}
