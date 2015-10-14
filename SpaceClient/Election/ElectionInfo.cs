using Common;
using ServerClientCommon;
using System.Collections;

namespace Nebula.Client.Election {
    public class ElectionInfo : IInfoParser {

        public int registrationStartTime { get; private set; }
        public int voteStartTime { get; private set; }
        public int voteEndTime { get; private set; }
        public bool votingStarted { get; private set; }
        public bool registrationStarted { get; private set; }
        public int currentTime { get; private set; }

        public readonly MyElectionStatus myElectionStatus = new MyElectionStatus();

        public void ParseInfo(Hashtable info) {
            registrationStartTime = info.GetValue<int>((int)SPC.RegistrationStartTime, 0);
            voteStartTime = info.GetValue<int>((int)SPC.VoteStartTime, 0);
            voteEndTime = info.GetValue<int>((int)SPC.VoteEndTime, 0);
            votingStarted = info.GetValue<bool>((int)SPC.VotingStarted, false);
            registrationStarted = info.GetValue<bool>((int)SPC.RegistrationStarted, false);
            currentTime = info.GetValue<int>((int)SPC.CurrentTime, 0);
        }

        public ElectionInfo() { }

        public ElectionInfo(Hashtable info) {
            ParseInfo(info);
        }

        public void Clear() {
            myElectionStatus.Clear();
        }
    }
}
