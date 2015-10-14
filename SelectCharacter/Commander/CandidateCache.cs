using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectCharacter.Commander {
    public class CandidateCache {
        public readonly ConcurrentDictionary<string, CommanderCandidate> mCandidates = new ConcurrentDictionary<string, CommanderCandidate>();

        public bool TryAddCandidate(CommanderCandidate candidate) {
            return mCandidates.TryAdd(candidate.characterID, candidate);
        }

        public bool TryGetCandidate(string characterID, out CommanderCandidate candidate) {
            return mCandidates.TryGetValue(characterID, out candidate);
        }

        public void Clear() {
            mCandidates.Clear();
        }
    }
}
