using Common;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using ServerClientCommon;

namespace SelectCharacter.Commander {
    public class CommanderElectionInfo : IInfoSource {
        public ObjectId Id { get; set; }
        public int candidateRegistrationStartTime { get; set; }
        public int startTime { get; set; }
        public int endTime { get; set; }
        public bool started { get; set; }
        public bool registrationStarted { get; set; }

        //public bool newDateSetted { get; set; }

        public void SetElectionTime(int registrationTime, int start, int end) {
            candidateRegistrationStartTime = registrationTime;
            startTime = start;
            endTime = end;
            //newDateSetted = true;
            UpdateState();
        }



        public void UpdateState() {
            int nowTime = CommonUtils.SecondsFrom1970();

            if(nowTime > startTime && nowTime <= endTime) {
                started = true;
                registrationStarted = false;
            } else if(nowTime > candidateRegistrationStartTime && nowTime <= startTime) {
                started = false;
                registrationStarted = true;
            } else {
                started = false;
                registrationStarted = false;
            }
        }

        public Hashtable GetInfo() {
            return new Hashtable {
                { (int)SPC.RegistrationStartTime, candidateRegistrationStartTime },
                { (int)SPC.VoteStartTime, startTime },
                { (int)SPC.VoteEndTime, endTime },
                { (int)SPC.VotingStarted, started },
                { (int)SPC.RegistrationStarted, registrationStarted },
                { (int)SPC.CurrentTime, CommonUtils.SecondsFrom1970() }
            };
        }

        //public bool CheckRegistration() {
        //    int nowTime = CommonUtils.SecondsFrom1970();
        //    if(nowTime > candidateRegistrationStartTime && nowTime <= startTime ) {
        //        return true;
        //    } else {
        //        return false;
        //    }
        //}
    }
}
