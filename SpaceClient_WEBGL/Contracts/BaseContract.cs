using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;
using ServerClientCommon;

namespace Nebula.Client.Contracts {
    public class BaseContract : IInfoParser {
        public string id { get; private set; }
        public ContractState state { get; private set; }
        public int stage { get; private set; }
        public string sourceWorld { get; private set; }
        public ContractCategory category { get; private set; }

        public virtual void ParseInfo(Hashtable info) {
            id = info.GetValueString((int)SPC.Id);
            state = (ContractState)info.GetValueInt((int)SPC.ContractState);
            stage = info.GetValueInt((int)SPC.ContractStage);
            sourceWorld = info.GetValueString((int)SPC.SourceWorld);
            category = (ContractCategory)info.GetValueInt((int)SPC.ContractCategory);
        }

        public BaseContract(Hashtable hash) {
            ParseInfo(hash);
        }

        public virtual Hashtable Dump() {
            Hashtable dumpHash = new Hashtable {
                { "id", id },
                { "state", state.ToString() },
                { "stage", stage.ToString() },
                { "sourceWorld", sourceWorld },
                { "category", category.ToString() }
            };
            return dumpHash;
        }
    }

}
