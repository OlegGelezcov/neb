using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Client.Election {
    public class MyElectionStatus {
        public bool voted { get; private set; }
        public bool candidate { get; private set; }

        public void SetVoted(bool value) {
            voted = value;
        }

        public void SetCandidate(bool value) {
            candidate = value;
        }

        public void Clear() {
            voted = false;
            candidate = false;
        }
    }
}
