using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Nebula.Client.Election {
    public class CommanderCandidates : IInfoParser {
        public Dictionary<string, CommanderCandidate> candidates { get; private set; }

        public CommanderCandidates() {
            candidates = new Dictionary<string, CommanderCandidate>();
        }

        public void ParseInfo(Hashtable info) {
            candidates.Clear();
            foreach(DictionaryEntry candidateEnrty in info) {
                string characterID = (string)candidateEnrty.Key;
                Hashtable candidateInfo = candidateEnrty.Value as Hashtable;
                candidates.Add(characterID, new CommanderCandidate(candidateInfo));
            }
        }
    }
}
