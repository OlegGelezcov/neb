using Common;
using ServerClientCommon;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;

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
            registrationStartTime = info.GetValueInt((int)SPC.RegistrationStartTime);
            voteStartTime = info.GetValueInt((int)SPC.VoteStartTime);
            voteEndTime = info.GetValueInt((int)SPC.VoteEndTime);
            votingStarted = info.GetValueBool((int)SPC.VotingStarted);
            registrationStarted = info.GetValueBool((int)SPC.RegistrationStarted);
            currentTime = info.GetValueInt((int)SPC.CurrentTime);
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
